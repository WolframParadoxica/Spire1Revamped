using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using Spire1Revamped.Spire1RevampedCode.Powers;

#nullable enable
namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Discharge() : Spire1RevampedCard(1,
  CardType.Skill, CardRarity.Ancient,
  TargetType.Self)
{
  public const string _powerVarName = "Discharge";
  
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling), HoverTipFactory.FromOrb<LightningOrb>()];

  protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(nameof (Discharge), 1M)];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    Discharge discharge = this;
    await CreatureCmd.TriggerAnim(discharge.Owner.Creature, "Cast", discharge.Owner.Character.CastAnimDelay);
    await OrbCmd.Channel<LightningOrb>(choiceContext, discharge.Owner);
    await OrbCmd.Channel<LightningOrb>(choiceContext, discharge.Owner);
    DischargePower? dischargePower = await PowerCmd.Apply<DischargePower>(choiceContext, discharge.Owner.Creature, discharge.DynamicVars[nameof (Discharge)].BaseValue, discharge.Owner.Creature, (CardModel) discharge);
  }

  protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}