using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]

public class Massacre() : Spire1RevampedCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new DamageVar(4M, ValueProp.Move),
        (DynamicVar) new PowerVar<MassacrePower>(50M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [this.EnergyHoverTip];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Massacre massacre = this;
        AttackCommand attackCommand = await DamageCmd.Attack(massacre.DynamicVars.Damage.BaseValue).FromCard((CardModel) massacre, cardPlay).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        MassacrePower? massacrePower = await PowerCmd.Apply<MassacrePower>(choiceContext, cardPlay.Target, massacre.DynamicVars["MassacrePower"].BaseValue, massacre.Owner.Creature, (CardModel) massacre);
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
        this.DynamicVars.Power<MassacrePower>().UpgradeValueBy(25);
    }
}