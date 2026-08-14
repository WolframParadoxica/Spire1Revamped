using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class OmenPower : Spire1RevampedPower
{
    private CardModel? _triggeringCard;
    private List<PowerModel>? _doubledPowers;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private CardModel? TriggeringCard { get => _triggeringCard; set { AssertMutable(); _triggeringCard = value; } }
    private List<PowerModel> DoubledPowers { get { AssertMutable(); _doubledPowers ??= []; return _doubledPowers; } }

    public override Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier, CardModel? cardSource)
    {
        if ((TriggeringCard is not null && TriggeringCard != cardSource) || cardSource is null || target != Owner || !power.IsVisible || power.GetTypeForAmount(amount) != PowerType.Debuff) return Task.CompletedTask;
        TriggeringCard = cardSource;
        DoubledPowers.Add(power);
        return Task.CompletedTask;
    }

    public override decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (target != Owner || cardSource is null || (TriggeringCard is not null && TriggeringCard != cardSource) || HasDoubledTemporaryPowerSource(power) || power.GetTypeForAmount(amount) != PowerType.Debuff) return 1M;
        return 2M;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != TriggeringCard) return;
        Flash();
        await PowerCmd.Decrement(this);
        TriggeringCard = null;
        DoubledPowers.Clear();
    }

    private bool HasDoubledTemporaryPowerSource(PowerModel power)
    {
        return DoubledPowers.OfType<ITemporaryPower>().Any(p => p.InternallyAppliedPower.GetType() == power.GetType());
    }
}