using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]

public class GlassKnife() : Spire1RevampedCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    public const string _decreaseKey = "Decrease";

    public Decimal _extraDamageFromPlays;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new DamageVar(10M, ValueProp.Move),
        new DynamicVar("Decrease", 2M)
    ];

    public Decimal ExtraDamageFromPlays
    {
        get => this._extraDamageFromPlays;
        set
        {
            this.AssertMutable();
            this._extraDamageFromPlays = value;
        }
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        GlassKnife card = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard((CardModel) card).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash", tmpSfx: "event:/sfx/characters/silent/silent_dagger_spray").Execute(choiceContext);
        DamageVar damage = card.DynamicVars.Damage;
        damage.BaseValue = damage.BaseValue - card.DynamicVars["Decrease"].BaseValue;
        card.ExtraDamageFromPlays -= card.DynamicVars["Decrease"].BaseValue;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DamageVar damage = this.DynamicVars.Damage;
        damage.BaseValue = damage.BaseValue - this.ExtraDamageFromPlays;
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
        this.DynamicVars["Decrease"].UpgradeValueBy(-1M);
    }
}