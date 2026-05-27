using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class BleedingAnvil : Spire1RevampedRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy((CustomRelicModel) this),
        HoverTipFactory.Static(StaticHoverTip.SummonStatic)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new EnergyVar(1)];
    
    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        BleedingAnvil bleedingAnvil = this;
        if (card.Owner.Creature.Player != bleedingAnvil.Owner)
            return;
        SummonResult summonResult = await OstyCmd.Summon((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), card.Owner, amount, (AbstractModel) bleedingAnvil);
    }
}