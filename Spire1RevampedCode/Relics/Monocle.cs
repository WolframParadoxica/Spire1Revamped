using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class Monocle : Spire1RevampedRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    private readonly SpireField<CardModel, bool> _hasCostSwapped = new(() => false);

    private static bool _isBlockingCostSwap;

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (_isBlockingCostSwap) return false;
        var isBlockingCostSwapPreviousValue = _isBlockingCostSwap;
        _isBlockingCostSwap = true;
        if (card.Owner != Owner)
        {
            _isBlockingCostSwap = false;
            return false;
        }
        if (originalCost <= 0M || card.EnergyCost.GetWithModifiers(CostModifiers.All) <= 0)
        {
            if (_hasCostSwapped.Get(card)) _hasCostSwapped[card] = false;
            _isBlockingCostSwap = false;
            return false;
        }
        modifiedCost = originalCost - 1M;
        if (modifiedCost < 0M)
        {
            modifiedCost = 0M;
            if (_hasCostSwapped.Get(card)) _hasCostSwapped[card] = false;
        }
        else if (!_hasCostSwapped.Get(card)) _hasCostSwapped[card] = true;
        _isBlockingCostSwap = isBlockingCostSwapPreviousValue;
        return true;
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner != Owner || card.HasStarCostX || !_hasCostSwapped.Get(card)) return false;
        modifiedCost = originalCost < 0M ? originalCost + 2M : originalCost + 1M;
        return true;
    }
}