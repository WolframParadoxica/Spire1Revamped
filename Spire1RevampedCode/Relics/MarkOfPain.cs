using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class MarkOfPain : Spire1RevampedRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this),
        ..HoverTipFactory.FromCardWithCardHoverTips<Pain>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override async Task AfterObtained()
    {
        List<CardPileAddResult> results = [];
        for (var i = 0; i < 2; ++i)
            results.Add(await CardPileCmd.Add(Owner.RunState.CreateCard<Pain>(Owner), PileType.Deck));
        CardCmd.PreviewCardPileAdd(results, 2f);
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.BaseValue;
    }
}