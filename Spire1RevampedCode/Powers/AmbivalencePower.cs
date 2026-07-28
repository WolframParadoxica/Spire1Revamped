using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class AmbivalencePower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BoolVar("ShuffleNegate", false)
  ];
  
  public void SetShuffleNegate(bool value){
    ((BoolVar) DynamicVars["ShuffleNegate"]).BoolVal = value;
  }
  
  public override Task AfterShuffle(PlayerChoiceContext choiceContext, Player player)
  {
    AmbivalencePower ambivalencePower = this;
    if (player == this.Owner.Player)
      ((BoolVar)DynamicVars["ShuffleNegate"]).BoolVal = true;
    return Task.CompletedTask;
  }
  
  public override async Task AfterCardChangedPiles(
    CardModel card,
    PileType oldPileType,
    AbstractModel? clonedBy)
  {
    if (PileType.Draw.GetPile(this.Owner.Player).Cards.FirstOrDefault<CardModel>() == card && !((BoolVar)DynamicVars["ShuffleNegate"]).BoolVal)
    {
      this.Flash();
      DrawCardsNextTurnPower cardsNextTurnPower = await PowerCmd.Apply<DrawCardsNextTurnPower>(new BlockingPlayerChoiceContext(), this.Owner, this.Amount, this.Owner,  (CardModel) null);
    }
    else
    {
      if (((BoolVar)DynamicVars["ShuffleNegate"]).BoolVal)
        ((BoolVar)DynamicVars["ShuffleNegate"]).BoolVal = false;
    }
  }
}

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Shuffle), 
  typeof(PlayerChoiceContext), typeof(Player))]
class PreShufflePatch
{
  [HarmonyPrefix]
  static void PreShuffleHook(PlayerChoiceContext choiceContext,
    Player player)
  {
    if (player.HasPower<AmbivalencePower>())
    {
      AmbivalencePower? power = player.Creature.GetPower<AmbivalencePower>();
      power.SetShuffleNegate(true);
    }
  }
}