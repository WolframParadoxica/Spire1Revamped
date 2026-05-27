using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class Monocle : Spire1RevampedRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new CardsVar(1)];
    
    public override async Task AfterStarsSpent(int amount, Player spender)
    {
        Monocle monocle = this;
        if (amount <= 0 || spender != monocle.Owner)
            return;
        monocle.Flash();
        for (int i = 0; (Decimal) i < amount; ++i)
        {
            CardModel cardModel = await CardPileCmd.Draw((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), monocle.Owner);
        }
    }
}