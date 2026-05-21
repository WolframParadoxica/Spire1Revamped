using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Spire1Revamped.Spire1RevampedCode.Extensions;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class InvigoratePower : Spire1RevampedPower
{
    //Loads from Spire1Revamped/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
  {
    InvigoratePower invigoratePower = this;
    if (side != invigoratePower.Owner.Side)
      return;
    IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(invigoratePower.Owner.Player).Cards;
    if (cards.Count == 0)
      return;
    decimal amount = cards.Count * this.Amount;
    invigoratePower.Flash();
    VigorPower vigorPower = await PowerCmd.Apply<VigorPower>(choiceContext, invigoratePower.Owner, amount, invigoratePower.Owner, (CardModel) null);
    //VigorPower vigorPower = PowerCmd.Apply<VigorPower>(choiceContext, invigoratePower.Owner.Player, amount, invigoratePower.Owner.Player, (CardModel) cardSource);
  }
  
}