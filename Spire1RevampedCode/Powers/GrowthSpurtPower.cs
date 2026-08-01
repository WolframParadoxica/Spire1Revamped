using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class GrowthSpurtPower : Spire1RevampedPower
{
  private CardModel? _currentlyPlayingCard;

  private CardModel? _triggeringCard;

  public void SetTriggeringCard(CardModel value){
    this._triggeringCard = value;
  }

  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  private CardModel? TriggeringCard
  {
    get => this._triggeringCard;
    set
    {
      this.AssertMutable();
      this._triggeringCard = value;
    }
  }

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.SummonStatic),HoverTipFactory.Static(StaticHoverTip.Block)];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BoolVar("WasDoubled", false)
  ];
  
  public void SetWasDoubled(bool value){
    ((BoolVar) DynamicVars["WasDoubled"]).BoolVal = value;
  }

  public override Task BeforeCardPlayed(CardPlay cardPlay)
  {
    this._currentlyPlayingCard = cardPlay.Card;
    return Task.CompletedTask;
  }

  public override decimal ModifyPowerAmountGivenMultiplicative(
    PowerModel power,
    Creature giver,
    Decimal amount,
    Creature? target,
    CardModel? cardSource)
  {
    return power is not SummonNextTurnPower || giver != Owner || cardSource == null || amount <= 0M ? 1M : 2M;
  }

  public override Task AfterModifyingPowerAmountGiven(PowerModel power)
  {
    if (power is not SummonNextTurnPower)
      return Task.CompletedTask;
    this.TriggeringCard = _currentlyPlayingCard;
    this.SetWasDoubled(true);
    return Task.CompletedTask;
  }

  public override Decimal ModifyBlockMultiplicative(
    Creature target,
    Decimal block,
    ValueProp props,
    CardModel? cardSource,
    CardPlay? cardPlay)
  {
    return !props.IsCardOrMonsterMove() || cardSource == null || this.TriggeringCard != null && this.TriggeringCard != cardSource || cardSource.Owner.Creature != this.Owner ? 1M : 2M;
  }

  public override Task AfterModifyingBlockAmount(
    Decimal modifiedAmount,
    CardModel? cardSource,
    CardPlay? cardPlay)
  {
    if (modifiedAmount <= 0M || cardSource == null)
      return Task.CompletedTask;
    this.TriggeringCard = cardSource;
    this.SetWasDoubled(true);
    return Task.CompletedTask;
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (cardPlay.Card != this.TriggeringCard)
      this.TriggeringCard = null;
    if (((BoolVar)DynamicVars["WasDoubled"]).BoolVal)
    {
      this.Flash();
      this.SetWasDoubled(false);
      await PowerCmd.Decrement(this);
      this.TriggeringCard = null;
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
    __result *= 2;
  }
}

[HarmonyPatch(typeof(OstyCmd), nameof(OstyCmd.Summon), 
  typeof(PlayerChoiceContext), typeof(Player), typeof(decimal), typeof(AbstractModel))]
class PreSummonPatch
{
  [HarmonyPrefix]
  static void PreSummonHook(PlayerChoiceContext choiceContext, Player summoner, decimal amount, AbstractModel source)
  {
    if (amount <= 0M) 
      return;
    if (source is not CardModel cardSource)
      return;
    GrowthSpurtPower? power = summoner.Creature.GetPower<GrowthSpurtPower>();
    power?.SetTriggeringCard(cardSource);
    power?.SetWasDoubled(true);
  }
}