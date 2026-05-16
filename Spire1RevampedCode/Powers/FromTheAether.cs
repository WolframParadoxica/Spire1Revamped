using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Spire1Revamped.Spire1RevampedCode.Extensions;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class FromTheAetherPower : Spire1RevampedPower
{
    //Loads from Spire1Revamped/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
  {
    FromTheAetherPower fromTheAetherPower = this;
    if (creator == null || creator.Creature != fromTheAetherPower.Owner)
      return;
    fromTheAetherPower.Flash();
    IEnumerable<CardModel> cardModels = await CardPileCmd.Draw((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), (Decimal) fromTheAetherPower.Amount, fromTheAetherPower.Owner.Player);
//    DrawCardsNextTurnPower cardsNextTurnPower = await PowerCmd.Apply<DrawCardsNextTurnPower>((PlayerChoiceContext) new ThrowingPlayerChoiceContext(),fromTheAetherPower.Owner, this.Amount, fromTheAetherPower.Owner,  (CardModel) null);
  }
}