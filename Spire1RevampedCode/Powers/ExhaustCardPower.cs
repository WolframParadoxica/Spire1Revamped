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

    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.IsAutoPlay) return Task.CompletedTask;
        GetInternalData<Data>().AmountsForPlayedCards[cardPlay.Card] = Amount;
        return Task.CompletedTask;
    }
  
    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay, ResourceInfo resources, CardLocation location)
    {
        if (card.Owner.Creature != Owner || isAutoPlay || card.Keywords.Contains(BaseLibKeywords.Purge) || card.Type is CardType.Power || location.pileType is PileType.None) return location;
        location.pileType = PileType.Exhaust;
        return location;
    }

    public override Task AfterModifyingCardPlayResultLocation(CardModel card, CardLocation cardLocation)
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.IsAutoPlay || !cardPlay.IsFirstInSeries || !GetInternalData<Data>().AmountsForPlayedCards.Remove(cardPlay.Card, out var amount) || amount <= 0) return;
        await PowerCmd.Decrement(this);
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> AmountsForPlayedCards = new();
    }
}