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

#nullable enable
namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]
public class Rogue() : Spire1RevampedCard(1,
  CardType.Skill, CardRarity.Ancient,
  TargetType.Self)
{
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    (DynamicVar) new BlockVar(15M, ValueProp.Move)
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Sly)];
  
  public static readonly SpireField<CardModel, bool> TemporaryReplay = new(() => false);

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    Rogue rogue = this;
    Decimal num = await CreatureCmd.GainBlock(rogue.Owner.Creature, rogue.DynamicVars.Block, cardPlay);
    CardModel card = (await CardSelectCmd.FromHandForDiscard(choiceContext, rogue.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), (Func<CardModel, bool>) null, (AbstractModel) rogue)).FirstOrDefault<CardModel>();
    if (card == null)
      return;
    if (card._hasSingleTurnSly || card.Keywords.Contains(CardKeyword.Sly))
      TemporaryReplay[card] = true;
    await CardCmd.Discard(choiceContext, card);
  }

  public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
  {
    return card.Owner != this.Owner || !TemporaryReplay.Get(card) ? playCount : playCount + 1;
  }
  
  public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (TemporaryReplay.Get(cardPlay.Card))
    {
      TemporaryReplay[cardPlay.Card] = false;
    }
    return Task.CompletedTask;
  }
  
  protected override void OnUpgrade()
  {
    this.DynamicVars.Block.UpgradeValueBy(5M);
  }
}