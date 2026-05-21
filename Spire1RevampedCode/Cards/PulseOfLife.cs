using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class PulseOfLife() : Spire1RevampedCard(0,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new PowerVar<PulseOfLifePower>(1M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        PulseOfLife pulseOfLife = this;
        await CreatureCmd.TriggerAnim(pulseOfLife.Owner.Creature, "Cast", pulseOfLife.Owner.Character.CastAnimDelay);
        PulseOfLifePower pulseOfLifePower = await PowerCmd.Apply<PulseOfLifePower>(choiceContext, pulseOfLife.Owner.Creature, DynamicVars.Power<PulseOfLifePower>().BaseValue, pulseOfLife.Owner.Creature, (CardModel) pulseOfLife);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}