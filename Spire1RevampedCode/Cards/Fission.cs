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
public class Fission() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    
public override OrbEvokeType OrbEvokeType => OrbEvokeType.All;

protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new CardsVar(1),
        (DynamicVar) new CalculationBaseVar(0M),
        (DynamicVar) new CalculationExtraVar(1M),
        (DynamicVar) new CalculatedVar("CalculatedOrbs").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => (Decimal) card.Owner.PlayerCombatState.OrbQueue.Orbs.GroupBy<OrbModel, ModelId>((Func<OrbModel, ModelId>) (orb => orb.Id)).Count<IGrouping<ModelId, OrbModel>>()))
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Fission fission = this;
        int evokeCount = (int) ((CalculatedVar) fission.DynamicVars["CalculatedOrbs"]).Calculate(cardPlay.Target) * (int) fission.DynamicVars.Cards.BaseValue;
        int orbCount = fission.Owner.PlayerCombatState.OrbQueue.Orbs.Count;
        for (int i = 0; i < orbCount; ++i)
        {
            await OrbCmd.EvokeNext(choiceContext, fission.Owner);
        }
        IEnumerable<CardModel> cardModels = await CardPileCmd.Draw(choiceContext, evokeCount, fission.Owner);
        await CreatureCmd.TriggerAnim(fission.Owner.Creature, "Cast", fission.Owner.Character.CastAnimDelay);
        for (int i = 0; i < evokeCount; ++i)
            await OrbCmd.Channel(choiceContext, OrbModel.GetRandomOrb(fission.Owner.RunState.Rng.CombatOrbGeneration).ToMutable(), fission.Owner);
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(1M);
}