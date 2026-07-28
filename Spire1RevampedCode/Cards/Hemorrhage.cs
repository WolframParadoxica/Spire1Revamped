using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Hemorrhage() : Spire1RevampedCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new DamageVar(21M, ValueProp.Move),
        (DynamicVar) new HpLossVar(1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Hemorrhage hemorrhage = this;
        VfxCmd.PlayOnCreatureCenter(hemorrhage.Owner.Creature, "vfx/vfx_bloody_impact");
        int count = 0;
        foreach (Creature hittableEnemy in (IEnumerable<Creature>)this.CombatState.HittableEnemies)
        {
            count++;
        }
        for (int i = 0; i < count; ++i)
        {
            IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, hemorrhage.Owner.Creature, hemorrhage.DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, (CardModel) hemorrhage,cardPlay);
        }
        AttackCommand attackCommand = await DamageCmd.Attack(hemorrhage.DynamicVars.Damage.BaseValue).FromCard((CardModel) hemorrhage,cardPlay).TargetingAllOpponents(hemorrhage.CombatState).WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(7M);
}