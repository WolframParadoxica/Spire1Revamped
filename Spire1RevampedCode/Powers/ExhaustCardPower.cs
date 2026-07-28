using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class ExhaustCardPower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;
  
  public override PowerStackType StackType => PowerStackType.Counter;
  
  public override CardLocation ModifyCardPlayResultLocation(
    CardModel card,
    bool isAutoPlay,
    ResourceInfo resources,
    CardLocation location)
  {
    if (card.Owner.Creature != this.Owner || isAutoPlay)
      return location;
    location.pileType = PileType.Exhaust;
    return location;
  }

  public override async Task AfterModifyingCardPlayResultLocation(
    CardModel card,
    CardLocation cardLocation)
  {
    this.Flash();
    await PowerCmd.Decrement((PowerModel) this);
  }
}