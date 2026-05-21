using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]
public class Concentrate() : Spire1RevampedCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Shiv>(),EnergyHoverTip];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new EnergyVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Concentrate source = this;
        var heldcards = new List<CardModel>(2);
        foreach (CardModel original in (await CardSelectCmd.FromHand(choiceContext, source.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt,2, 2), (Func<CardModel, bool>) null, (AbstractModel) source)).ToList<CardModel>())
        {//dump cards to a list
            heldcards.Add(original);
        }//discard cards and gain energy by manually extracting the discard function and rearranging it
        List<CardModel> discardCards;
        ICombatState combatState;
        List<CardModel> slyCards;
        CardPile discardPile;
        if (CombatManager.Instance.IsOverOrEnding)
        {
            discardCards = (List<CardModel>) null;
            combatState = (ICombatState) null;
            slyCards = (List<CardModel>) null;
            discardPile = (CardPile) null;
        }
        else
        {
            discardCards = heldcards.ToList<CardModel>();
            combatState = discardCards[0].CombatState ?? discardCards[0].Owner.Creature.CombatState;
            slyCards = new List<CardModel>();
            discardPile = PileType.Discard.GetPile(discardCards[0].Owner);
            foreach (CardModel card in discardCards)
            {
                var cardCost = 0;
                int energyAtTimeOfPlay = source.Owner.PlayerCombatState.Energy;
                if (card.EnergyCost.CostsX)
                    cardCost = energyAtTimeOfPlay;
                else if (card.EnergyCost.GetResolved() == -1)
                    cardCost = 0;
                else
                    cardCost = card.EnergyCost.GetResolved();
                if (card.IsSlyThisTurn)
                    slyCards.Add(card);
                CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, discardPile);
                CombatManager.Instance.History.CardDiscarded(combatState, card);
                await Hook.AfterCardDiscarded(combatState, choiceContext, card);
                await PlayerCmd.GainEnergy(cardCost, source.Owner);
            }
            discardPile.InvokeContentsChanged();
            foreach (CardModel card in slyCards)
                await CardCmd.AutoPlay(choiceContext, card, (Creature) null, AutoPlayType.SlyDiscard);
            discardCards = (List<CardModel>) null;
            combatState = (ICombatState) null;
            slyCards = (List<CardModel>) null;
            discardPile = (CardPile) null;
        }
        foreach (CardModel original in heldcards)
        {//transform the discarded cards
            if (original.Pile != null)
            {
                CardModel card = (CardModel) source.CombatState.CreateCard<Shiv>(source.Owner);
                CardPileAddResult? nullable = await CardCmd.Transform(original, card);
            }
        }
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}