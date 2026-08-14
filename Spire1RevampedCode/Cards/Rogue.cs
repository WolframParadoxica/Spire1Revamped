using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]
public class Rogue() : Spire1RevampedCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
  protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15M, ValueProp.Move)];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Sly)];

  private static readonly SpireField<CardModel, bool> TemporaryReplay = new(() => false);

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    var card = (await CardSelectCmd.FromHandForDiscard(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
    if (card is null) return;
    if (card._hasSingleTurnSly || card.Keywords.Contains(CardKeyword.Sly)) TemporaryReplay[card] = true;
    await CardCmd.Discard(choiceContext, card);
  }

  public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
  {
    return card.Owner != Owner || !TemporaryReplay.Get(card) ? playCount : playCount + 1;
  }

  public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (TemporaryReplay.Get(cardPlay.Card)) TemporaryReplay[cardPlay.Card] = false;
    return Task.CompletedTask;
  }

  protected override void OnUpgrade()
  {
    DynamicVars.Block.UpgradeValueBy(5M);
  }
}