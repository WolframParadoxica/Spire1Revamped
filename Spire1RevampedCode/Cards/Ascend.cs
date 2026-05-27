using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

#nullable enable
namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(RegentCardPool))]
public class Ascend() : Spire1RevampedCard(1,
  CardType.Skill, CardRarity.Ancient,
  TargetType.Self)
{

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    (DynamicVar) new StarsVar(1),
    new DynamicVar("StarMultiplier", 1M)
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    Ascend ascend = this;
    await CreatureCmd.TriggerAnim(ascend.Owner.Creature, "Cast", ascend.Owner.Character.CastAnimDelay);
    await PlayerCmd.GainStars(ascend.DynamicVars["StarMultiplier"].BaseValue*this.Owner.PlayerCombatState.Stars, ascend.Owner);
  }

  protected override void OnUpgrade() => this.DynamicVars["StarMultiplier"].UpgradeValueBy(1M);
}