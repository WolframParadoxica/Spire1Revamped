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

public class GrimReaper() : Spire1RevampedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new OstyDamageVar(6M, ValueProp.Move)];

    protected override bool ShouldGlowRedInternal => Owner.IsOstyMissing;

    protected override HashSet<CardTag> CanonicalTags => [CardTag.OstyAttack];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!Osty.CheckMissingWithAnim(Owner))
        {
            var attackCommand = await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue).FromOsty(Owner.Osty!, this,cardPlay).Targeting(cardPlay.Target!).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
            await CreatureCmd.Heal(Owner.Osty!, attackCommand.Results.SelectMany(r => r).Sum((Func<DamageResult, int>) (r => r.TotalDamage + r.OverkillDamage)));
        }
    }

    protected override void OnUpgrade() => DynamicVars.OstyDamage.UpgradeValueBy(3M);
}