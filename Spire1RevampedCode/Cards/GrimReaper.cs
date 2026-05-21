using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(NecrobinderCardPool))]

public class GrimReaper() : Spire1RevampedCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new OstyDamageVar(6M, ValueProp.Move)];
    
    protected override bool ShouldGlowRedInternal => this.Owner.IsOstyMissing;

    protected override HashSet<CardTag> CanonicalTags => [CardTag.OstyAttack];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        GrimReaper grimReaper = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        if (!Osty.CheckMissingWithAnim(grimReaper.Owner))
        {
            AttackCommand attackCommand = await DamageCmd.Attack(grimReaper.DynamicVars.OstyDamage.BaseValue).FromOsty(grimReaper.Owner.Osty, (CardModel) grimReaper).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
            await CreatureCmd.Heal(cardPlay.Card.Owner.Osty, (Decimal) attackCommand.Results.SelectMany<List<DamageResult>, DamageResult>((Func<List<DamageResult>, IEnumerable<DamageResult>>) (r => (IEnumerable<DamageResult>) r)).Sum<DamageResult>((Func<DamageResult, int>) (r => r.TotalDamage + r.OverkillDamage)));
        }
    }

    protected override void OnUpgrade() => this.DynamicVars.OstyDamage.UpgradeValueBy(3M);
}