using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Fission() : Spire1RevampedCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override OrbEvokeType OrbEvokeType => OrbEvokeType.All;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1), new CalculationBaseVar(0M), new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedOrbs").WithMultiplier(((Func<CardModel, Creature, decimal>) ((card, _) => card.Owner.PlayerCombatState!.OrbQueue.Orbs.GroupBy((Func<OrbModel, ModelId>) (orb => orb.Id)).Count()))!)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var evokeCount = ((CalculatedVar) DynamicVars["CalculatedOrbs"]).Calculate(cardPlay.Target) * DynamicVars.Cards.BaseValue;
        var orbCount = Owner.PlayerCombatState!.OrbQueue.Orbs.Count;
        for (var i = 0; i < orbCount; ++i) await OrbCmd.EvokeNext(choiceContext, Owner);
        await CardPileCmd.Draw(choiceContext, evokeCount, Owner);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        for (var i = 0; i < evokeCount; ++i) await OrbCmd.Channel(choiceContext, OrbModel.GetRandomOrb(Owner.RunState.Rng.CombatOrbGeneration).ToMutable(), Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1M);
}