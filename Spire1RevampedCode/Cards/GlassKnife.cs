using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]

public class GlassKnife() : Spire1RevampedCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private decimal _extraDamageFromPlays;
    private decimal ExtraDamageFromPlays { get => _extraDamageFromPlays; set { AssertMutable(); _extraDamageFromPlays = value; } }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10M, ValueProp.Move), new ("Decrease", 2M)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard(this, cardPlay).Targeting(cardPlay.Target!).WithHitFx("vfx/vfx_attack_slash", tmpSfx: "event:/sfx/characters/silent/silent_dagger_spray").Execute(choiceContext);
        var damage = DynamicVars.Damage;
        damage.BaseValue -= DynamicVars["Decrease"].BaseValue;
        ExtraDamageFromPlays -= DynamicVars["Decrease"].BaseValue;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        var damage = DynamicVars.Damage;
        damage.BaseValue -= ExtraDamageFromPlays;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["Decrease"].UpgradeValueBy(-1M);
    }
}