using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class GalvanizePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;
  
  public override PowerStackType StackType => PowerStackType.Counter;
  
  public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
  {
    GalvanizePower galvanizePower = this;
    if (player != galvanizePower.Owner.Player)
      return;
    await PowerCmd.Decrement((PowerModel) galvanizePower);
  }
}

[HarmonyPatch(typeof(LightningOrb), "Passive", MethodType.Async)]
public static class LightningOrb_Passive_Patch
{
  [HarmonyTranspiler]
  private static IEnumerable<CodeInstruction> Transpiler(
    IEnumerable<CodeInstruction> instructions,
    ILGenerator generator,
    MethodBase original)
  {
    return AsyncMethodCall.Create(
      generator,
      instructions,
      original,
      AccessTools.Method(typeof(LightningOrb_Passive_Patch), nameof(SecondHit)),
      beforeState: original
    );
  }

  public static async Task SecondHit(LightningOrb __instance, PlayerChoiceContext choiceContext, Creature? target)
  {
    if (!__instance.Owner.Creature.HasPower<GalvanizePower>())
      return;

    await __instance.ApplyLightningDamage(__instance.PassiveVal, target, choiceContext);
  }
}

[HarmonyPatch(typeof(LightningOrb), "Evoke", MethodType.Async)]
public static class LightningOrb_Evoke_Patch
{
  [HarmonyTranspiler]
  private static IEnumerable<CodeInstruction> Transpiler(
    IEnumerable<CodeInstruction> instructions,
    ILGenerator generator,
    MethodBase original)
  {
    return AsyncMethodCall.Create(
      generator,
      instructions,
      original,
      AccessTools.Method(typeof(LightningOrb_Evoke_Patch), nameof(SecondEvoke)),
      beforeState: original
    );
  }

  public static async Task SecondEvoke(LightningOrb __instance, PlayerChoiceContext playerChoiceContext)
  {
    if (!__instance.Owner.Creature.HasPower<GalvanizePower>())
      return;

    var targets = await __instance.ApplyLightningDamage(__instance.EvokeVal, null, playerChoiceContext);
    if (__instance.Owner.Creature.CombatState == null)
      return;
    await Hook.AfterOrbEvoked(playerChoiceContext, __instance.Owner.Creature.CombatState, __instance, targets);
  }
}