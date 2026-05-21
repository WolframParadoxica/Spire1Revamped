using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Preignite() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new CardsVar(3)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Preignite card1 = this;
        var cards = new List<CardModel>(card1.DynamicVars.Cards.IntValue);
        CardPile drawPile = PileType.Draw.GetPile(card1.Owner);
        for (int i = 0; i < card1.DynamicVars.Cards.IntValue; ++i)
        {
            await CardPileCmd.ShuffleIfNecessary(choiceContext, card1.Owner);
            CardModel card = drawPile.Cards.FirstOrDefault<CardModel>();
            if (card != null)
            {
                cards.Add(card);
                card.RemoveFromCurrentPile();
            }
            else
                break;
        }
        
        CardModel card2 = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, card1.Owner, new CardSelectorPrefs(card1.SelectionScreenPrompt, 1))).FirstOrDefault<CardModel>();
        if (card2 == null)
            return;
        await CardCmd.AutoPlay(choiceContext, card2, null);
        foreach (CardModel original in cards)
            if (original != card2)
            {
                CardPileAddResult cardPileAddResult = await CardPileCmd.Add(original, PileType.Draw);
                await CardCmd.Exhaust(choiceContext, original);
            }
    }

    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(2M);
}