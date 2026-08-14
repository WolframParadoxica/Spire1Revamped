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

  protected override IEnumerable<DynamicVar> CanonicalVars => [new BoolVar("ShuffleNegate", false)];

  public void SetShuffleNegate(bool value)
  {
    ((BoolVar) DynamicVars["ShuffleNegate"]).BoolVal = value;
  }

  public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
  {
    var drawPile = PileType.Draw.GetPile(Owner.Player!).Cards;
    if (drawPile.Count > 0 && drawPile[0] == card)
    {
        if (((BoolVar)DynamicVars["ShuffleNegate"]).BoolVal) return;
        Flash();
        await PowerCmd.Apply<DrawCardsNextTurnPower>(new BlockingPlayerChoiceContext(), Owner, Amount, Owner, null);
    }
  }
}

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Shuffle), typeof(PlayerChoiceContext), typeof(Player))]
class CardPileCmd_Shuffle_Patch
{
  [HarmonyPrefix]
  private static void PreShuffleHook(Player player)
  {
    if (!player.HasPower<AmbivalencePower>()) return;
    var power = player.Creature.GetPower<AmbivalencePower>();
    power?.SetShuffleNegate(true);
  }

  [HarmonyPostfix]
  private static void PostShuffleHook(Player player)
  {
    if (!player.HasPower<AmbivalencePower>()) return;
    var power = player.Creature.GetPower<AmbivalencePower>();
    power?.SetShuffleNegate(false);
  }
}