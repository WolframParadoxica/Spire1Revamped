using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]

public class Aggregate() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> hoverTips =
            [
                this.EnergyHoverTip
            ];
            switch (this.IsUpgraded)
            {
                case false:
                    hoverTips.Add(HoverTipFactory.FromCard<Void>());
                    break;
                case true:
                    hoverTips.Add(HoverTipFactory.FromCard<Dazed>());
                    break;
            }
            return hoverTips;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new EnergyVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Retain];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Aggregate aggregate = this;
        int energy = 0;
        List<CardModel> list = Aggregate.GetStatuses(aggregate.Owner).ToList<CardModel>();
        await CreatureCmd.TriggerAnim(aggregate.Owner.Creature, "Cast", aggregate.Owner.Character.CastAnimDelay);
        foreach (CardModel card2 in list)
        {
            CardModel card = (CardModel) null;
            switch (this.IsUpgraded)
            {
                case false:
                    card = (CardModel) aggregate.CombatState.CreateCard<Void>(aggregate.Owner);
                    break;
                case true:
                    card = (CardModel) aggregate.CombatState.CreateCard<Dazed>(aggregate.Owner);
                    break;
            }
            CardPileAddResult? nullable = await CardCmd.Transform(card2, card);
            energy++;
        }
        await PlayerCmd.GainEnergy(energy, aggregate.Owner);
    }

    public static IEnumerable<CardModel> GetStatuses(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where<CardModel>((Func<CardModel, bool>) (c => c.Type == CardType.Status && c.Pile.Type != PileType.Exhaust));
    }
}