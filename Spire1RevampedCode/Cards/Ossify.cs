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

[Pool(typeof(NecrobinderCardPool))]
public class Ossify() : Spire1RevampedCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new PowerVar<DexterityPower>(1M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>(),HoverTipFactory.Static(StaticHoverTip.SummonStatic)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Ossify ossify = this;
        await CreatureCmd.TriggerAnim(ossify.Owner.Creature, "Cast", ossify.Owner.Character.CastAnimDelay);
        DexterityPower dexterityPower = await PowerCmd.Apply<DexterityPower>(choiceContext, ossify.Owner.Creature, ossify.DynamicVars.Dexterity.BaseValue, ossify.Owner.Creature, (CardModel) ossify);
        OssifyPower ossifyPower = await PowerCmd.Apply<OssifyPower>(choiceContext, ossify.Owner.Creature, 1M, ossify.Owner.Creature, (CardModel) ossify);
    }

    protected override void OnUpgrade() => this.DynamicVars.Dexterity.UpgradeValueBy(1M);
}