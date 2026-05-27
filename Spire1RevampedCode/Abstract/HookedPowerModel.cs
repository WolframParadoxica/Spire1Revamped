using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Spire1Revamped.Spire1RevampedCode.Abstract;

public abstract class HookedPowerModel : CustomPowerModel
{
    private async Task ExecuteWithContext(Func<PlayerChoiceContext, Task> action)
    {
        if (LocalContext.NetId == null)
            throw new InvalidOperationException(
                $"Cannot execute power hook '{GetType().Name}': LocalContext.NetId is null.");
        if (Owner.IsDead) return;
        var ctx = new HookPlayerChoiceContext(
            this,
            LocalContext.NetId.Value,
            CombatState,
            GameActionType.Combat);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(action(ctx));
    }
    
    public sealed override Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        return ExecuteWithContext(ctx => AfterCardGeneratedForCombat(ctx, card, player));
    }

    protected virtual Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? player)
    {
        return Task.CompletedTask;
    }
}