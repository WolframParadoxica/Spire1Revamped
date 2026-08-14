using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class GrowthSpurtPower : Spire1RevampedPower
{
    private CardModel? _currentlyPlayingCard;
    private CardModel? _triggeringCard;
    private CardModel? _pendingPowerSource;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.SummonStatic), HoverTipFactory.Static(StaticHoverTip.Block)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BoolVar("WasDoubled", false)];

    internal CardModel? TriggeringCard { get => _triggeringCard; set { AssertMutable(); _triggeringCard = value; } }

    internal bool WasDoubled { get => ((BoolVar)DynamicVars["WasDoubled"]).BoolVal; set { AssertMutable(); ((BoolVar)DynamicVars["WasDoubled"]).BoolVal = value; } }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        _currentlyPlayingCard = cardPlay.Card;
        return Task.CompletedTask;
    }

    public override decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (power is not SummonNextTurnPower || giver != Owner || cardSource is null || amount <= 0M || (TriggeringCard is not null && TriggeringCard != cardSource)) return 1M;
        _pendingPowerSource = cardSource;
        return 2M;
    }

    public override Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        if (power is not SummonNextTurnPower) return Task.CompletedTask;
        TriggeringCard = _pendingPowerSource ?? _currentlyPlayingCard;
        WasDoubled = true;
        _pendingPowerSource = null; 
        return Task.CompletedTask;
    }

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        return !props.IsCardOrMonsterMove() || cardSource is null || (TriggeringCard is not null && TriggeringCard != cardSource) || cardSource.Owner.Creature != Owner ? 1M : 2M;
    }

    public override Task AfterModifyingBlockAmount(decimal modifiedAmount, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (modifiedAmount <= 0M || cardSource is null) return Task.CompletedTask;
        TriggeringCard = cardSource;
        WasDoubled = true;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == TriggeringCard)
        {
            if (WasDoubled)
            {
                Flash();
                WasDoubled = false;
                await PowerCmd.Decrement(this);
            }
            TriggeringCard = null;
        }
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifySummonAmount), typeof(ICombatState), typeof(Player), typeof(decimal), typeof(AbstractModel))]
class Hook_ModifySummonAmount_Patch
{
    [HarmonyPostfix]
    static void PostModifySummonAmountHook(Player summoner, ref decimal __result, AbstractModel source)
    {
        if (source is not CardModel cardSource || summoner.Creature.GetPower<GrowthSpurtPower>() is not { } power) return;
        if (power.TriggeringCard is not null && power.TriggeringCard != cardSource) return;
        if (cardSource.Owner.Creature != summoner.Creature) return;
        __result *= 2;
    }
}

[HarmonyPatch(typeof(OstyCmd), nameof(OstyCmd.Summon), typeof(PlayerChoiceContext), typeof(Player), typeof(decimal), typeof(AbstractModel))]
class OstyCmd_Summon_Patch
{
    [HarmonyPrefix]
    static void PreSummonHook(Player summoner, decimal amount, AbstractModel source)
    {
        if (amount <= 0M || source is not CardModel cardSource || summoner.Creature.GetPower<GrowthSpurtPower>() is not { } power) return;
        if (power.TriggeringCard is not null && power.TriggeringCard != cardSource) return;
        if (cardSource.Owner.Creature != summoner.Creature) return;
        power.TriggeringCard = cardSource;
        power.WasDoubled = true;
    }
}