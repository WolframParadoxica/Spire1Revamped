using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(RegentCardPool))]
public class Ascend() : Spire1RevampedCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
  protected override IEnumerable<DynamicVar> CanonicalVars => [new StarsVar(1), new ("StarMultiplier", 1M)];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
    await PlayerCmd.GainStars(DynamicVars["StarMultiplier"].BaseValue*Owner.PlayerCombatState!.Stars, Owner);
  }

  protected override void OnUpgrade() => DynamicVars["StarMultiplier"].UpgradeValueBy(1M);
}