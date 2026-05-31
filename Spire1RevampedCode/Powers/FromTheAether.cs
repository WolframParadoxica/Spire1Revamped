using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class FromTheAetherPower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
  {
    FromTheAetherPower fromTheAetherPower = this;
    if (creator == null || creator.Creature != fromTheAetherPower.Owner)
      return;
    fromTheAetherPower.Flash();
    IEnumerable<CardModel> cardModels = await CardPileCmd.Draw(
      (PlayerChoiceContext) new BlockingPlayerChoiceContext(),
      (Decimal) fromTheAetherPower.Amount,
      fromTheAetherPower.Owner.Player);
  }
}