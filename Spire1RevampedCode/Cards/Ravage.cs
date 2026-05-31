using BaseLib.Utils;
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

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> hoverTips = [];
            if (!this.IsUpgraded)
                hoverTips.Add(HoverTipFactory.FromPower<VulnerablePower>());
            hoverTips.Add(HoverTipFactory.Static(StaticHoverTip.Block));
            hoverTips.Add(HoverTipFactory.FromKeyword(CardKeyword.Sly));
            return hoverTips;
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>(1M), new DynamicVar(nameof (Ravage), 1M)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Ravage ravage = this;
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);
        await CreatureCmd.LoseBlock(this.Owner.Creature, this.Owner.Creature.Block);
        if (!this.IsUpgraded)
        {
            VulnerablePower vulnerablePower = await PowerCmd.Apply<VulnerablePower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature,
                (CardModel)this);
        }
        RavagePower ravagePower = await PowerCmd.Apply<RavagePower>(choiceContext, this.Owner.Creature, this.DynamicVars[nameof (Ravage)].BaseValue, this.Owner.Creature, (CardModel) this);
    }
}