using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class DischargePower : Spire1RevampedPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromOrb<LightningOrb>()];

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Decrement(this);
    }
}

[HarmonyPatch(typeof(LightningOrb), "Passive", MethodType.Async)]
public static class LightningOrb_Passive_Patch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
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
        if (CombatManager.Instance.IsOverOrEnding) return;
        if (__instance.Owner.Creature.GetPower<DischargePower>() is not { } power) return;
        power.Flash();
        await __instance.ApplyLightningDamage(__instance.PassiveVal, target, choiceContext, false);
    }
}

[HarmonyPatch(typeof(LightningOrb), "Evoke", MethodType.Async)]
public static class LightningOrb_Evoke_Patch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        return AsyncMethodCall.Create(generator, instructions, original, AccessTools.Method(typeof(LightningOrb_Evoke_Patch), nameof(SecondEvoke)), beforeState: original);
    }

    public static async Task SecondEvoke(LightningOrb __instance, PlayerChoiceContext playerChoiceContext)
    {
        if (CombatManager.Instance.IsOverOrEnding) return;
        var combatState = __instance.Owner.Creature.CombatState;
        if (combatState is null || __instance.Owner.Creature.GetPower<DischargePower>() is not { } power) return;
        var targets = await __instance.ApplyLightningDamage(__instance.EvokeVal, null, playerChoiceContext, true);
        power.Flash();
        await Hook.AfterOrbEvoked(playerChoiceContext, combatState, __instance, targets);
    }
}