using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class InvigoratePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
  {
    if (side != Owner.Side) return;
    var cards = PileType.Hand.GetPile(Owner.Player!).Cards;
    if (cards.Count is 0) return;
    Flash();
    await PowerCmd.Apply<VigorPower>(choiceContext, Owner, cards.Count * Amount, Owner, null);
  }
}
