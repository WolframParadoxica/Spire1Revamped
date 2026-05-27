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

  protected override object InitInternalData() => (object) new PulseOfLifePower.Data();

  public override Task BeforeCardPlayed(CardPlay cardPlay)
  {
    if (cardPlay.Card.Owner.Creature != this.Owner)
      return Task.CompletedTask;
    this.GetInternalData<PulseOfLifePower.Data>().amountsForPlayedCards.Add(cardPlay.Card, this.Amount);
    return Task.CompletedTask;
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    PulseOfLifePower pulseOfLifePower = this;
    int amount;
    if (cardPlay.Card.Owner.Creature != pulseOfLifePower.Owner || !pulseOfLifePower.GetInternalData<PulseOfLifePower.Data>().amountsForPlayedCards.Remove(cardPlay.Card, out amount) || amount <= 0)
      return;
    if (cardPlay.Card.Owner.Creature != pulseOfLifePower.Owner || !cardPlay.Card.Owner.IsOstyAlive)
      return;
    await CreatureCmd.Heal(cardPlay.Card.Owner.Osty, amount);
  }

  public class Data
  {
    public readonly Dictionary<CardModel, int> amountsForPlayedCards = new Dictionary<CardModel, int>();
  }
  
}