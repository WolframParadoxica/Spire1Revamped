using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Discharge() : Spire1RevampedCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling), HoverTipFactory.FromOrb<LightningOrb>()];

  protected override IEnumerable<DynamicVar> CanonicalVars => [new (nameof (Discharge), 1M)];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
    await OrbCmd.Channel<LightningOrb>(choiceContext, Owner);
    await OrbCmd.Channel<LightningOrb>(choiceContext, Owner);
    await PowerCmd.Apply<DischargePower>(choiceContext, Owner.Creature, DynamicVars[nameof (Discharge)].BaseValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}