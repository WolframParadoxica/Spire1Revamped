using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class FrozenBattery : Spire1RevampedRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var orbs = Owner.PlayerCombatState!.OrbQueue.Orbs;
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type is not CardType.Power || orbs.Count <= 0)
            return;
        Flash();
        await OrbCmd.Passive(choiceContext, orbs[0], null);
    }
}