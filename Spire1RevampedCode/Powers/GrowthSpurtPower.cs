using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class GrowthSpurtPower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.SummonStatic)];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BoolVar("SummonDoubled", false)
  ];
  
  public void SetSummonDoubled(bool value){
    ((BoolVar) DynamicVars["SummonDoubled"]).BoolVal = value;
  }

  public override decimal ModifyPowerAmountGivenMultiplicative(
    PowerModel power,
    Creature giver,
    Decimal amount,
    Creature? target,
    CardModel? cardSource)
  {
    return power is not SummonNextTurnPower ? 1M : 2M;
  }

  public override async Task AfterModifyingPowerAmountGiven(PowerModel power)
  {
    await PowerCmd.Decrement((PowerModel) this);
  }

  public override async Task AfterSummon(PlayerChoiceContext choiceContext, Player summoner, decimal amount)
  {
    if (((BoolVar)DynamicVars["SummonDoubled"]).BoolVal)
    {
      this.SetSummonDoubled(false);
      await PowerCmd.Decrement((PowerModel)this);
    }
  }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifySummonAmount), 
  typeof(ICombatState), typeof(Player), typeof(decimal), typeof(AbstractModel))]
class PostModifySummonPatch
{
  [HarmonyPostfix]
  static void PostModifySummonHook(ICombatState combatState, Player summoner, ref decimal __result, AbstractModel source)
  {
    if (!summoner.HasPower<GrowthSpurtPower>() || source is not CardModel)
      return;
    GrowthSpurtPower? power = summoner.Creature.GetPower<GrowthSpurtPower>();
    power.SetSummonDoubled(true);
    __result *= 2;
  }
}