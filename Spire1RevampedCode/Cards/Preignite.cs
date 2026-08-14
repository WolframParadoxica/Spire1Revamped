using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Preignite() : Spire1RevampedCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards = new List<CardModel>(DynamicVars.Cards.IntValue);
        var drawPile = PileType.Draw.GetPile(Owner).Cards;
        while (cards.Count < DynamicVars.Cards.IntValue)
        {//grab the 3(4) cards
            await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner);
            if (drawPile.Count is 0) break;
            cards.Add(drawPile[0]);
            drawPile[0].RemoveFromCurrentPile();
        }//exhaust them in sequence
        foreach (var card in cards)
        {
            await CardPileCmd.Add(card, PileType.Draw);
            await CardCmd.Exhaust(choiceContext, card);
        }//open up ui to play one of them
        if ((await CardSelectCmd.FromSimpleGrid(choiceContext, cards, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault() is { } selectedCard) await CardCmd.AutoPlay(choiceContext, selectedCard, null);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1M);
}