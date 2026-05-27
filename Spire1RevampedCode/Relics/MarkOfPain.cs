using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class MarkOfPain : Spire1RevampedRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy((CustomRelicModel) this),
        ..HoverTipFactory.FromCardWithCardHoverTips<Pain>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new EnergyVar(1)];

    public override async Task AfterObtained()
    {
        MarkOfPain markOfPain = this;
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        for (int i = 0; i < 2; ++i)
            results.Add(await CardPileCmd.Add((CardModel) markOfPain.Owner.RunState.CreateCard<Pain>(markOfPain.Owner), PileType.Deck));
        CardCmd.PreviewCardPileAdd((IReadOnlyList<CardPileAddResult>) results, 2f);
        results = (List<CardPileAddResult>) null;
    }

    public override Decimal ModifyMaxEnergy(Player player, Decimal amount)
    {
        return player != this.Owner ? amount : amount + this.DynamicVars.Energy.BaseValue;
    }
}