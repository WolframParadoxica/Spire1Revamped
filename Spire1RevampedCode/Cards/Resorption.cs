using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class Resorption() : Spire1RevampedCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new PowerVar<ResorptionPower>(1M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.SummonStatic),HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Resorption resorption = this;
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);
        if (resorption.IsUpgraded)
        {
            GrowthSpurtPower? growthSpurtPower = await PowerCmd.Apply<GrowthSpurtPower>(choiceContext, this.Owner.Creature, DynamicVars.Power<ResorptionPower>().BaseValue, this.Owner.Creature, (CardModel) this);
        }
        ResorptionPower? resorptionPower = await PowerCmd.Apply<ResorptionPower>(choiceContext, this.Owner.Creature, DynamicVars.Power<ResorptionPower>().BaseValue, this.Owner.Creature, (CardModel) this);
    }
}