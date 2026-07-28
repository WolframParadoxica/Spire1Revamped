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

  protected override IEnumerable<IHoverTip> ExtraHoverTips
  {
    get
    {
      List<IHoverTip> hoverTips = [];
      hoverTips.AddRange(HoverTipFactory.FromCardWithCardHoverTips<SovereignBlade>());
      return hoverTips;
    }
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    EnGardePower enGardePower = this;
    if (!(cardPlay.Card is SovereignBlade) || cardPlay.Card.Owner.Creature != enGardePower.Owner)
      return;
    FreeCardPower? freeCardPower = await PowerCmd.Apply<FreeCardPower>(choiceContext, this.Owner, enGardePower.Amount, this.Owner, (CardModel) ModelDb.Card<EnGarde>());
    ExhaustCardPower? exhaustCardPower = await PowerCmd.Apply<ExhaustCardPower>(choiceContext, this.Owner, enGardePower.Amount, this.Owner, (CardModel) ModelDb.Card<EnGarde>());
  }
}