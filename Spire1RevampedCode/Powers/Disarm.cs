using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class DisarmPower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

  public override async Task AfterDamageReceived(
    PlayerChoiceContext choiceContext,
    Creature target,
    DamageResult result,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource)
  {
    DisarmPower disarmPower = this;
    if (target != disarmPower.Owner || result.UnblockedDamage <= 0 || disarmPower.Owner.CombatState.CurrentSide != disarmPower.Owner.Side)
      return;
    IReadOnlyList<DisarmedPower> disarmedPowerList = await PowerCmd.Apply<DisarmedPower>(choiceContext, (IEnumerable<Creature>) CombatState.HittableEnemies, disarmPower.Amount, disarmPower.Owner, (CardModel) null);
  }
}