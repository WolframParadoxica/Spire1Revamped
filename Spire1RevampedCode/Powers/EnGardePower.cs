using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class EnGardePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<SovereignBlade>();

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (cardPlay.Card is not SovereignBlade || cardPlay.Card.Owner.Creature != Owner) return;
    await PowerCmd.Apply<FreeCardPower>(choiceContext, Owner, Amount, Owner, ModelDb.Card<EnGarde>());
    await PowerCmd.Apply<ExhaustCardPower>(choiceContext, Owner, Amount, Owner, ModelDb.Card<EnGarde>());
  }
}