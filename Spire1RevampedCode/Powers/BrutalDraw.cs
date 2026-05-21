using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Spire1Revamped.Spire1RevampedCode.Extensions;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class BrutalDrawPower : Spire1RevampedPower
{
    //Loads from Spire1Revamped/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    
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