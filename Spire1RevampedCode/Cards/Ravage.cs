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
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]

public class Ravage() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    public const string _powerVarName = "Ravage";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<FrailPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Sly)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar(nameof (Ravage), 1M),
        new PowerVar<FrailPower>(1M),
        new PowerVar<VulnerablePower>(1M),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Ravage ravage = this;
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);
        FrailPower frailPower = await PowerCmd.Apply<FrailPower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature, (CardModel)this);
        VulnerablePower vulnerablePower = await PowerCmd.Apply<VulnerablePower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature, (CardModel)this);
        RavagePower ravagePower = await PowerCmd.Apply<RavagePower>(choiceContext, this.Owner.Creature, this.DynamicVars[nameof (Ravage)].BaseValue, this.Owner.Creature, (CardModel) this);
        await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, this.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, this.DynamicVars.Cards.IntValue), (Func<CardModel, bool>) null, (AbstractModel) this));
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(1M);
}