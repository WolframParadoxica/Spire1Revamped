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
public class Ambivalence() : Spire1RevampedCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new CardsVar(1),(DynamicVar) new PowerVar<AmbivalencePower>(1M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VigorPower>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Ambivalence ambivalence = this;
        await CreatureCmd.TriggerAnim(ambivalence.Owner.Creature, "Cast", ambivalence.Owner.Character.CastAnimDelay);
        AmbivalencePower? ambivalencePower = await PowerCmd.Apply<AmbivalencePower>(choiceContext, ambivalence.Owner.Creature, DynamicVars.Power<AmbivalencePower>().BaseValue, ambivalence.Owner.Creature, (CardModel) ambivalence);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(1M);
        this.DynamicVars.Power<AmbivalencePower>().UpgradeValueBy(1);
    }
}