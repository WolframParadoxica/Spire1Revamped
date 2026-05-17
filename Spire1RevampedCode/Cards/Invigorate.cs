using BaseLib.Extensions;
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

[Pool(typeof(RegentCardPool))]
public class Impotence() : Spire1RevampedCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new PowerVar<ImpotencePower>(1M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VigorPower>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Impotence impotence = this;
        await CreatureCmd.TriggerAnim(impotence.Owner.Creature, "Cast", impotence.Owner.Character.CastAnimDelay);
        ImpotencePower impotencePower = await PowerCmd.Apply<ImpotencePower>(choiceContext, impotence.Owner.Creature, DynamicVars.Power<ImpotencePower>().BaseValue, impotence.Owner.Creature, (CardModel) impotence);
    }

    protected override void OnUpgrade() => this.DynamicVars.Power<ImpotencePower>().UpgradeValueBy(1);
}
