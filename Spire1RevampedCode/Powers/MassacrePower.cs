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

public sealed class MassacrePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Debuff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BoolVar("EffectUsed", false)
  ];
  
  public override Decimal ModifyDamageMultiplicative(
    Creature? target,
    Decimal amount,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource,
    CardPlay? cardPlay)
  {
    return target != this.Owner || !props.IsPoweredAttack() || cardSource == null || cardPlay != null && cardPlay.Resources.EnergySpent != 0 || cardPlay == null && cardSource.EnergyCost.GetWithModifiers(CostModifiers.All) != 0 ? 1M : 1M + (Decimal) this.Amount / 100M;
  }
  
  public override Task AfterModifyingDamageAmount(CardModel? cardSource)
  {
    ((BoolVar)DynamicVars["EffectUsed"]).BoolVal = true;
    return Task.CompletedTask;
  }
  
  public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
  {
    if (((BoolVar)DynamicVars["EffectUsed"]).BoolVal)
      await PowerCmd.Remove((PowerModel) this);
  }
}