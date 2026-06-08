using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

#nullable enable
namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]
public class Rogue() : Spire1RevampedCard(1,
  CardType.Skill, CardRarity.Ancient,
  TargetType.Self)
{
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(),HoverTipFactory.FromPower<WeakPower>()];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    (DynamicVar) new BlockVar(15M, ValueProp.Move),
    (DynamicVar) new CardsVar(3)
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    Rogue rogue = this;
    Decimal num = await CreatureCmd.GainBlock(rogue.Owner.Creature, rogue.DynamicVars.Block, cardPlay);
    CardModel card = (await CardSelectCmd.FromHandForDiscard(choiceContext, rogue.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), (Func<CardModel, bool>) null, (AbstractModel) rogue)).FirstOrDefault<CardModel>();
    if (card == null)
      return;
    await CardCmd.Discard(choiceContext, card);
    DrawCardsNextTurnPower cardsNextTurnPower = await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, rogue.Owner.Creature, rogue.DynamicVars.Cards.BaseValue, rogue.Owner.Creature, (CardModel) rogue);
  }

  protected override void OnUpgrade()
  {
    this.DynamicVars.Block.UpgradeValueBy(5M);
    this.DynamicVars.Cards.UpgradeValueBy(2M);
  }
}