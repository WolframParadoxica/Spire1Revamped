using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

#nullable enable
namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Cripple() : Spire1RevampedCard(2,
  CardType.Attack, CardRarity.Ancient,
  TargetType.AnyEnemy)
{

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(),HoverTipFactory.FromPower<WeakPower>()];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    (DynamicVar) new DamageVar(15M, ValueProp.Move),
    (DynamicVar) new PowerVar<VulnerablePower>(2M),
    (DynamicVar) new PowerVar<WeakPower>(1M)
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    Cripple @cripple = this;
    ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
    AttackCommand attackCommand = await DamageCmd.Attack(@cripple.DynamicVars.Damage.BaseValue).FromCard((CardModel) @cripple).Targeting(cardPlay.Target).WithAttackerAnim(Ironclad.GetHeavyAnimIfApplicable(@cripple.Owner.Character), Ironclad.GetHeavyAttackDelayIfApplicable(@cripple.Owner.Character)).WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "heavy_attack.mp3").WithHitVfxSpawnedAtBase().Execute(choiceContext);
    VulnerablePower vulnerablePower = await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, @cripple.DynamicVars.Vulnerable.BaseValue, @cripple.Owner.Creature, (CardModel) @cripple);
    WeakPower weakPower = await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, @cripple.DynamicVars.Weak.BaseValue*cardPlay.Target.GetPowerAmount<VulnerablePower>(), @cripple.Owner.Creature, (CardModel) @cripple);
  }

  protected override void OnUpgrade()
  {
    this.DynamicVars.Damage.UpgradeValueBy(5M);
    this.DynamicVars.Weak.UpgradeValueBy(1M);
  }
}