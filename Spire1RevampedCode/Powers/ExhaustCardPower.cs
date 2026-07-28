using BaseLib.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class ExhaustCardPower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;
  
  public override PowerStackType StackType => PowerStackType.Counter;

  protected override object InitInternalData() => (object) new ExhaustCardPower.Data();

  public override Task BeforeCardPlayed(CardPlay cardPlay)
  {
    if (cardPlay.Card.Owner.Creature != this.Owner)
      return Task.CompletedTask;
    this.GetInternalData<ExhaustCardPower.Data>().amountsForPlayedCards.Add(cardPlay.Card, this.Amount);
    return Task.CompletedTask;
  }
  public override CardLocation ModifyCardPlayResultLocation(
    CardModel card,
    bool isAutoPlay,
    ResourceInfo resources,
    CardLocation location)
  {
    if (card.Owner.Creature != this.Owner || isAutoPlay || card.Keywords.Contains(BaseLibKeywords.Purge) || card.Type == CardType.Power || location.pileType == PileType.None)
      return location;
    location.pileType = PileType.Exhaust;
    return location;
  }

  public override Task AfterModifyingCardPlayResultLocation(
    CardModel card,
    CardLocation cardLocation)
  {
    this.Flash();
    return Task.CompletedTask;
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    ExhaustCardPower exhaustCardPower = this;
    int amount;
    if (cardPlay.Card.Owner.Creature != this.Owner || cardPlay.IsAutoPlay || !this.GetInternalData<ExhaustCardPower.Data>().amountsForPlayedCards.Remove(cardPlay.Card, out amount) || amount <= 0)
      return;
    await PowerCmd.Decrement((PowerModel) this);
    return;
  }

  public class Data
  {
    public readonly Dictionary<CardModel, int> amountsForPlayedCards = new Dictionary<CardModel, int>();
  }
}