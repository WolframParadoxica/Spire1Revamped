using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

#nullable enable
namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class OmenPower : Spire1RevampedPower
{
  private CardModel? _triggeringCard;
  private List<PowerModel>? _doubledPowers;
//  private bool _isFinishedTriggering;

  public override PowerType Type => PowerType.Debuff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BoolVar("EffectUsed", false)
  ];

  private CardModel? TriggeringCard
  {
    get => this._triggeringCard;
    set
    {
      this.AssertMutable();
      this._triggeringCard = value;
    }
  }

  private List<PowerModel> DoubledPowers
  {
    get
    {
      this.AssertMutable();
      if (this._doubledPowers == null)
        this._doubledPowers = new List<PowerModel>();
      return this._doubledPowers;
    }
  }

//  private bool IsFinishedTriggering
//  {
//    get => this._isFinishedTriggering;
//    set
//    {
//      this.AssertMutable();
//      this._isFinishedTriggering = value;
//    }
//  }

//  public void SetOmenUsed(bool value){
//((BoolVar) DynamicVars["EffectUsed"]).BoolVal = value;
//  }

  public override Task BeforePowerAmountChanged(
    PowerModel power,
    Decimal amount,
    Creature target,
    Creature? applier,
    CardModel? cardSource)
  {
    if (this.TriggeringCard != null || cardSource == null || target.Side != this.Owner.Side || target != this.Owner || !power.IsVisible || power.GetTypeForAmount(amount) != PowerType.Debuff)
      return Task.CompletedTask;
    this.TriggeringCard = cardSource;
    this.DoubledPowers.Add(power);
//    ((BoolVar)DynamicVars["EffectUsed"]).BoolVal = true;
    return Task.CompletedTask;
  }

  public override Decimal ModifyPowerAmountGivenMultiplicative(
    PowerModel power,
    Creature giver,
    Decimal amount,
    Creature? target,
    CardModel? cardSource)
  {
    return this.TriggeringCard == null || cardSource != this.TriggeringCard || target != this.Owner || this.HasDoubledTemporaryPowerSource(power) || power.GetTypeForAmount(amount) != PowerType.Debuff ? 1M : 2M;
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (cardPlay.Card != this.TriggeringCard)// || this.IsFinishedTriggering)
      return;
    this.Flash();
//    this.IsFinishedTriggering = true;
    await PowerCmd.Decrement((PowerModel) this);
//    ((BoolVar)DynamicVars["EffectUsed"]).BoolVal = false;
    this.TriggeringCard = null;
  }

  private bool HasDoubledTemporaryPowerSource(PowerModel power)
  {
    return this.DoubledPowers.OfType<ITemporaryPower>().Any<ITemporaryPower>((Func<ITemporaryPower, bool>) (p => p.InternallyAppliedPower.GetType() == power.GetType()));
  }
}