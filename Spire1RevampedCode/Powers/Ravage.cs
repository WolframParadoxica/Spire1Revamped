using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class RavagePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;
  
  public override PowerStackType StackType => PowerStackType.Counter;
  
  public override Task AfterApplied(Creature? applier, CardModel? cardSource)
  {
    RavagePower ravagePower = this;
    foreach (CardModel allCard in ravagePower.Owner.Player.PlayerCombatState.AllCards)
      if (allCard.Type == CardType.Attack)
        CardCmd.ApplySingleTurnSly(allCard);
    return Task.CompletedTask;
  }
  
  public override Task AfterCardEnteredCombat(CardModel card)
  {
    RavagePower ravagePower = this;
    if (card.Owner == ravagePower.Owner.Player)
      if (card.Type == CardType.Attack)
        CardCmd.ApplySingleTurnSly(card);
    return Task.CompletedTask;
  }

  public override Task AfterEnergyReset(Player player)
  {
    RavagePower ravagePower = this;
    if (player != ravagePower.Owner.Player)
      return base.AfterEnergyReset(player);
    PowerCmd.Decrement((PowerModel) ravagePower);
    if (this.Amount <= 0) return base.AfterEnergyReset(player);
    foreach (CardModel allCard in ravagePower.Owner.Player.PlayerCombatState.AllCards)
      if (allCard.Type == CardType.Attack)
        CardCmd.ApplySingleTurnSly(allCard);
    return base.AfterEnergyReset(player);
  }
}