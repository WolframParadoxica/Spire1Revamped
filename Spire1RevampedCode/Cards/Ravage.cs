using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]

public class Ravage() : Spire1RevampedCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip, HoverTipFactory.FromKeyword(CardKeyword.Sly)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new (nameof (RavagePower), 1M), new (nameof (AttackCostUpPower), 1M), new EnergyVar("ExtraCost", 1), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<RavagePower>(choiceContext, Owner.Creature, DynamicVars[nameof (RavagePower)].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<AttackCostUpPower>(choiceContext, Owner.Creature, DynamicVars[nameof (AttackCostUpPower)].BaseValue, Owner.Creature, this);
        await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars.Cards.IntValue), null, this));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<AttackCostUpPower>().UpgradeValueBy(1M);
        DynamicVars.Power<RavagePower>().UpgradeValueBy(1M);
        DynamicVars["ExtraCost"].UpgradeValueBy(1M);
        DynamicVars.Cards.UpgradeValueBy(1M);
    }
}