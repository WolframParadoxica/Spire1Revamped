using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class MarkOfPain : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy((CustomRelicModel) this),
        HoverTipFactory.FromCard<Pain>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new EnergyVar(1)];

    public override async Task AfterObtained()
    {
        CardModel deck = await CardPileCmd.AddCurseToDeck<Pain>(this.Owner);
    }

    public override Decimal ModifyMaxEnergy(Player player, Decimal amount)
    {
        return player != this.Owner ? amount : amount + this.DynamicVars.Energy.BaseValue;
    }
}