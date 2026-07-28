using BaseLib.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Warcry() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new CardsVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Warcry warcry = this;
        IEnumerable<CardModel> cardModels = await CardPileCmd.Draw(choiceContext, 1M, warcry.Owner);
        CardSelectorPrefs prefs = new CardSelectorPrefs(warcry.SelectionScreenPrompt, 1);
        CardModel card = (await CardSelectCmd.FromHand(choiceContext, warcry.Owner, prefs, (Func<CardModel, bool>) null, (AbstractModel) warcry)).FirstOrDefault<CardModel>();
        if (card != null)
        {
            CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
        }
        DrawCardsNextTurnPower? cardsNextTurnPower = await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, this.Owner.Creature, warcry.DynamicVars.Cards.BaseValue, this.Owner.Creature, (CardModel) this);
    }

    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(1M);
}