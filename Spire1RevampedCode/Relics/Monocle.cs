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

    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new EnergyVar(1)];
    
    public readonly SpireField<CardModel, bool> HasCostSwapped = new(() => false);

    public override Task BeforeCombatStartLate()
    {
        Monocle monocle = this;
        foreach (CardModel allCard in monocle.Owner.PlayerCombatState.AllCards)
            if (allCard.Owner == monocle.Owner)
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
        Monocle monocle = this;
        if (card.Owner == monocle.Owner)
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
    
    private static bool __isBlockingCostSwap;
    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        Monocle monocle = this;
        modifiedCost = originalCost;
        if (__isBlockingCostSwap)
            return false;
        bool isBlockingCostSwapPreviousValue = __isBlockingCostSwap;
        __isBlockingCostSwap = true;
        if (card.Owner != this.Owner)
        {
            __isBlockingCostSwap = false;
            return false;
        }
        if (originalCost <= 0M || card.EnergyCost.GetWithModifiers(CostModifiers.All)<=0)
        {
            if (this.HasCostSwapped.Get(card))
            {
                //card.UpgradeStarCostBy(-1);
                HasCostSwapped[card] = false;
            }
            __isBlockingCostSwap = false;
            return false;
        }
        modifiedCost = originalCost - 1M;
        if (modifiedCost < 0M)
        {
            modifiedCost = 0M;
            if (this.HasCostSwapped.Get(card))
            {
                //card.UpgradeStarCostBy(-1);
                HasCostSwapped[card] = false;
            }
        }
        else if (!this.HasCostSwapped.Get(card))
        {
            //card.UpgradeStarCostBy(1);
            HasCostSwapped[card] = true;
        }
        __isBlockingCostSwap = isBlockingCostSwapPreviousValue;
        return true;
    }
    public override bool TryModifyStarCost(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        Monocle monocle = this;
        modifiedCost = originalCost;
        if (card.Owner != this.Owner || originalCost < 0M || card.HasStarCostX || !this.HasCostSwapped.Get(card))
            return false;
        modifiedCost = originalCost + 1M;
        return true;
    }
}