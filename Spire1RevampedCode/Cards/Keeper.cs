using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;

#nullable enable
namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class Keeper() : Spire1RevampedCard(1,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.SummonDynamic, (DynamicVar) this.DynamicVars.Summon),HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new SummonVar(7M),
        new DynamicVar("BlockThreshold", 3M),
        (DynamicVar) new CalculationBaseVar(7M),
        (DynamicVar) new CalculationExtraVar(1M),
        (DynamicVar) new CalculatedVar("CalculatedSummon").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, target) => Math.Floor((card.Owner.Creature.Block) / card.DynamicVars["BlockThreshold"].BaseValue))),
        (DynamicVar) new CalculatedVar("CalculatedDisplaySummon").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, target) => Hook.ModifySummonAmount(card.CombatState,card.Owner, Math.Floor((card.Owner.Creature.Block) / card.DynamicVars["BlockThreshold"].BaseValue),card)))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Keeper keeper = this;
        await CreatureCmd.TriggerAnim(this.Owner.Creature, Necrobinder.GetSummonAnimIfApplicable(this.Owner.Character), Necrobinder.GetSummonDelayIfApplicable(this.Owner.Character));
        SummonResult summonResult = await OstyCmd.Summon(choiceContext, this.Owner, ((CalculatedVar) this.DynamicVars["CalculatedSummon"]).Calculate(cardPlay.Target), (AbstractModel) this);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Summon.UpgradeValueBy(3M);
        this.DynamicVars.CalculationBase.UpgradeValueBy(3M);
        this.DynamicVars["BlockThreshold"].UpgradeValueBy(-1M);
    }
}