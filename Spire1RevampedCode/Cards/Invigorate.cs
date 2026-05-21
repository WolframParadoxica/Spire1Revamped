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
public class Invigorate() : Spire1RevampedCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new PowerVar<InvigoratePower>(1M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VigorPower>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Invigorate invigorate = this;
        await CreatureCmd.TriggerAnim(invigorate.Owner.Creature, "Cast", invigorate.Owner.Character.CastAnimDelay);
        InvigoratePower invigoratePower = await PowerCmd.Apply<InvigoratePower>(choiceContext, invigorate.Owner.Creature, DynamicVars.Power<InvigoratePower>().BaseValue, invigorate.Owner.Creature, (CardModel) invigorate);
    }

    protected override void OnUpgrade() => this.DynamicVars.Power<InvigoratePower>().UpgradeValueBy(1);
}