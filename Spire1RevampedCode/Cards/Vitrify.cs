using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Vitrify() : Spire1RevampedCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{

protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromOrb<GlassOrb>(),
    HoverTipFactory.Static(StaticHoverTip.Channeling),
    HoverTipFactory.FromCard<Burn>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Vitrify vitrify = this;
        int count = vitrify.Owner.PlayerCombatState.OrbQueue.Orbs.Count;
        if (vitrify.IsUpgraded)
            count = vitrify.Owner.PlayerCombatState.OrbQueue.Capacity;
        await CreatureCmd.TriggerAnim(vitrify.Owner.Creature, "Cast", vitrify.Owner.Character.CastAnimDelay);
        for (int i = 0; i < count; ++i)
            await OrbCmd.Channel<GlassOrb>(choiceContext, vitrify.Owner);
        for (int i = 0; i < vitrify.DynamicVars.Cards.IntValue; ++i)
            await OrbCmd.Channel<GlassOrb>(choiceContext, vitrify.Owner);CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel) vitrify.CombatState.CreateCard<Burn>(vitrify.Owner), PileType.Discard, vitrify.Owner));
        await Cmd.Wait(0.5f);
    }
}