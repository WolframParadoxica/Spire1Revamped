using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class RestlessHandPower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;
  
  public override PowerStackType StackType => PowerStackType.Counter;
  
  public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
  {
    return card.Owner.Creature != this.Owner || card.Type != CardType.Attack || !card.Tags.Contains<CardTag>(CardTag.OstyAttack) ? playCount : playCount + 1;
  }
  
  public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
  {
    RestlessHandPower restlessHandPower = this;
    if (player != restlessHandPower.Owner.Player)
      return;
    await PowerCmd.Decrement((PowerModel) restlessHandPower);
  }
}