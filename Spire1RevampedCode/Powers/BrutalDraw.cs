using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class BrutalDrawPower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  public override Decimal ModifyHandDraw(Player player, Decimal count)
  {
      return player != this.Owner.Player ? count : count + (Decimal) this.Amount;
  }

  public override Task AfterModifyingHandDraw()
  {
      this.Flash();
      PowerCmd.Remove((PowerModel) this);
      return Task.CompletedTask;
  }
}