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

    public override Task BeforeCombatStartLate()
    {
        foreach (var allCard in Owner.PlayerCombatState!.AllCards)
            if (allCard.Owner == Owner)
                switch (allCard.CanonicalStarCost)
                {
                    case > 0:
                        break;
                    default:
                        //skip unplayable curses & statuses, quest cards, X energy cost cards, and stardust
                        if (allCard is { EnergyCost: { Canonical: > -1, CostsX: false }, HasStarCostX: false } && !allCard.Keywords.Contains(CardKeyword.Unplayable))
                            allCard.UpgradeStarCostBy(1);
                        break;
                }
        return base.BeforeCombatStartLate();
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner != Owner)
            return Task.CompletedTask;
        switch (card.CanonicalStarCost)
        {
            case > 0:
                break;
            default:
                //skip unplayable curses & statuses, quest cards, X energy cost cards, and stardust
                if (card is { EnergyCost: { Canonical: > -1, CostsX: false }, HasStarCostX: false } && !card.Keywords.Contains(CardKeyword.Unplayable))
                    card.UpgradeStarCostBy(1);
                break;
        }
        return Task.CompletedTask;
    }
    
    private static bool _isBlockingCostSwap;
    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (_isBlockingCostSwap)
            return false;
        bool isBlockingCostSwapPreviousValue = _isBlockingCostSwap;
        _isBlockingCostSwap = true;
        if (card.Owner != this.Owner)
        {
            _isBlockingCostSwap = false;
            return false;
        }
        if (originalCost <= 0M || card.EnergyCost.GetWithModifiers(CostModifiers.All)<=0)
        {
            if (this._hasCostSwapped.Get(card))
            {
                _hasCostSwapped[card] = false;
            }
            _isBlockingCostSwap = false;
            return false;
        }
        modifiedCost = originalCost - 1M;
        if (modifiedCost < 0M)
        {
            modifiedCost = 0M;
            if (this._hasCostSwapped.Get(card))
            {
                _hasCostSwapped[card] = false;
            }
        }
        else if (!this._hasCostSwapped.Get(card))
        {
            _hasCostSwapped[card] = true;
        }
        _isBlockingCostSwap = isBlockingCostSwapPreviousValue;
        return true;
    }
    public override bool TryModifyStarCost(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner != Owner || originalCost < 0M || card.HasStarCostX || !_hasCostSwapped.Get(card))
            return false;
        modifiedCost = originalCost + 1M;
        return true;
    }
}