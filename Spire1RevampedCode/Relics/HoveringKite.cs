using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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
public class HoveringKite : Spire1RevampedRelic
{
    private bool _wasUsedThisTurn;
    
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy((CustomRelicModel) this)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new EnergyVar(1)];

    public bool WasUsedThisTurn
    {
        get => this._wasUsedThisTurn;
        set
        {
            this.AssertMutable();
            this._wasUsedThisTurn = value;
        }
    }

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        HoveringKite hoveringKite = this;
        if (card.Owner != hoveringKite.Owner || hoveringKite.WasUsedThisTurn)
            return;
        hoveringKite.Flash();
        await PlayerCmd.GainEnergy(1, hoveringKite.Owner);
        hoveringKite.Status = RelicStatus.Normal;
        hoveringKite.WasUsedThisTurn = true;
    }

    public override Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        HoveringKite hoveringKite = this;
        if (!participants.Contains<Creature>(hoveringKite.Owner.Creature))
            return Task.CompletedTask;
        this.WasUsedThisTurn = false;
        this.Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
}