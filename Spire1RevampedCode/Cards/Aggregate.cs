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

public class Aggregate() : Spire1RevampedCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip, IsUpgraded ? HoverTipFactory.FromCard<Dazed>() : HoverTipFactory.FromCard<Void>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var energy = 0;
        var list = GetStatuses(Owner).ToList();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (var card2 in list)
        {
            CardModel card = IsUpgraded ? CombatState!.CreateCard<Dazed>(Owner) : CombatState!.CreateCard<Void>(Owner);
            await CardCmd.Transform(card2, card);
            energy++;
        }
        await PlayerCmd.GainEnergy(energy, Owner);
        await Cmd.Wait(0.5f);
    }

    private static IEnumerable<CardModel> GetStatuses(Player owner)
    {
        return owner.PlayerCombatState!.AllCards.Where(c => c.Type is CardType.Status && c.Pile!.Type is not PileType.Exhaust);
    }
}