using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Cripple() : Spire1RevampedCard(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<WeakPower>()];

  protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(15M, ValueProp.Move), new PowerVar<VulnerablePower>(2M), new PowerVar<WeakPower>(1M)];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target!).WithAttackerAnim(Ironclad.GetHeavyAnimIfApplicable(Owner.Character), Ironclad.GetHeavyAttackDelayIfApplicable(Owner.Character)).WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "heavy_attack.mp3").WithHitVfxSpawnedAtBase().Execute(choiceContext);
    await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target!, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target!, DynamicVars.Weak.BaseValue*cardPlay.Target!.GetPowerAmount<VulnerablePower>(), Owner.Creature, this);
  }

  protected override void OnUpgrade()
  {
    DynamicVars.Damage.UpgradeValueBy(5M);
    DynamicVars.Weak.UpgradeValueBy(1M);
  }
}