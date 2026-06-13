using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class FrozenBattery : Spire1RevampedRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];
//        HoverTipFactory.Static(StaticHoverTip.Channeling),
//        HoverTipFactory.FromOrb<FrostOrb>()
//    ];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        FrozenBattery frozenBattery = this;
        if (cardPlay.Card.Owner != this.Owner || cardPlay.Card.Type != CardType.Power || this.Owner.PlayerCombatState.OrbQueue.Orbs.Count <= 0)
            return;
        this.Flash();
        await OrbCmd.Passive(choiceContext, this.Owner.PlayerCombatState!.OrbQueue.Orbs.FirstOrDefault(), (Creature) null);
    }
}//"Whenever you play a Power, [gold]Channel[/gold] 1 [gold]Frost[/gold]."