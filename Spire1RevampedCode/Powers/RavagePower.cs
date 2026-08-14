using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class RavagePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Sly)];

  public override Task AfterApplied(Creature? applier, CardModel? cardSource)
  {
    foreach (var allCard in Owner.Player!.PlayerCombatState!.AllCards)
      if (allCard.Type is CardType.Attack) CardCmd.ApplySingleTurnSly(allCard);
    return Task.CompletedTask;
  }

  public override Task AfterCardEnteredCombat(CardModel card)
  {
    if (card.Owner != Owner.Player) return Task.CompletedTask;
    if (card.Type is CardType.Attack) CardCmd.ApplySingleTurnSly(card);
    return Task.CompletedTask;
  }

  public override Task AfterEnergyReset(Player player)
  {
    if (player.Creature != Owner) return base.AfterEnergyReset(player);
    PowerCmd.Decrement(this);
    if (Amount <= 0) return base.AfterEnergyReset(player);
    foreach (var allCard in Owner.Player!.PlayerCombatState!.AllCards)
      if (allCard.Type is CardType.Attack) CardCmd.ApplySingleTurnSly(allCard);
    return base.AfterEnergyReset(player);
  }
}