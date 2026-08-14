using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class MassacrePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Debuff;
  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<DynamicVar> CanonicalVars => [new BoolVar("EffectUsed", false)];

  public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
  {
    if (target != Owner || !props.IsPoweredAttack() || cardSource is null) return 1M;
    var calculatedCost = cardSource.EnergyCost.CostsX ? cardSource.Owner.PlayerCombatState?.Energy : cardSource.EnergyCost.GetWithModifiers(CostModifiers.All);
    if (!(cardPlay is not null ? cardPlay.Resources.EnergySpent is 0 : calculatedCost is 0)) return 1M;
    if (cardPlay is not null) { ((BoolVar)DynamicVars["EffectUsed"]).BoolVal = true; }
    return 1M + Amount / 100M;
  }

  public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
  {
    if (((BoolVar)DynamicVars["EffectUsed"]).BoolVal) await PowerCmd.Remove(this);
  }
}