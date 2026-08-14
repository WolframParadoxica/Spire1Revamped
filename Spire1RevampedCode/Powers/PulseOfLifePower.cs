using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class PulseOfLifePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override object InitInternalData() => new Data();

  public override Task BeforeCardPlayed(CardPlay cardPlay)
  {
    if (cardPlay.Card.Owner.Creature != Owner) return Task.CompletedTask;
    GetInternalData<Data>().AmountsForPlayedCards[cardPlay.Card] = Amount;
    return Task.CompletedTask;
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (cardPlay.Card.Owner.Creature != Owner || !GetInternalData<Data>().AmountsForPlayedCards.Remove(cardPlay.Card, out var amount) || amount <= 0) return;
    if (!cardPlay.Card.Owner.IsOstyAlive) return;
    Flash();
    await CreatureCmd.Heal(cardPlay.Card.Owner.Osty!, amount);
  }

  private class Data
  {
    public readonly Dictionary<CardModel, int> AmountsForPlayedCards = new();
  }
}