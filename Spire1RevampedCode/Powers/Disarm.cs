using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Spire1Revamped.Spire1RevampedCode.Extensions;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class DisarmPower : Spire1RevampedPower
{
    //Loads from Spire1Revamped/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    
  public override PowerType Type => PowerType.Debuff;

  public override PowerStackType StackType => PowerStackType.Counter;

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
    IReadOnlyList<DisarmedPower> disarmedPowerList = await PowerCmd.Apply<DisarmedPower>(choiceContext, (IEnumerable<Creature>) CombatState.HittableEnemies, this.Amount, disarmPower.Owner, (CardModel) null);
  }
}