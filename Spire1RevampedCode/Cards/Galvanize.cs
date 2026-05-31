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
public class Galvanize() : Spire1RevampedCard(1,
  CardType.Skill, CardRarity.Ancient,
  TargetType.Self)
{
  public const string _powerVarName = "Galvanize";
  
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling), HoverTipFactory.FromOrb<LightningOrb>()];

  protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(nameof (Galvanize), 1M)];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    Galvanize galvanize = this;
    await CreatureCmd.TriggerAnim(galvanize.Owner.Creature, "Cast", galvanize.Owner.Character.CastAnimDelay);
    await OrbCmd.Channel<LightningOrb>(choiceContext, galvanize.Owner);
    await OrbCmd.Channel<LightningOrb>(choiceContext, galvanize.Owner);
    GalvanizePower galvanizePower = await PowerCmd.Apply<GalvanizePower>(choiceContext, galvanize.Owner.Creature, galvanize.DynamicVars[nameof (Galvanize)].BaseValue, galvanize.Owner.Creature, (CardModel) galvanize);
  }

  protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}