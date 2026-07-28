using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(NecrobinderCardPool))]

public class Vitality() : Spire1RevampedCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new DamageVar(2M, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain,CardKeyword.Exhaust];

    protected override bool ShouldGlowRedInternal => this.Owner.IsOstyMissing;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Vitality vitality = this;
        if (Osty.CheckMissingWithAnim(vitality.Owner) || this.Owner.Osty.MaxHp == this.Owner.Osty.CurrentHp)
            return;
        var healAmount = this.Owner.Osty.MaxHp - this.Owner.Osty.CurrentHp;
        await CreatureCmd.Heal(cardPlay.Card.Owner.Osty, healAmount);
        IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext,
            (IEnumerable<Creature>) this.CombatState.HittableEnemies,
            (Decimal) healAmount * this.DynamicVars.Damage.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            this.Owner.Creature);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(1M);
}