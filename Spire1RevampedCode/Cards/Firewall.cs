using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Firewall() : Spire1RevampedCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public const string _calculatedBlocksKey = "CalculatedBlocks";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new BlockVar(6M, ValueProp.Move),
        (DynamicVar) new CalculationBaseVar(1M),
        (DynamicVar) new CalculationExtraVar(1M),
        (DynamicVar) new CalculatedVar("CalculatedBlocks").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => (Decimal) (CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Count<CardPlayFinishedEntry>((Func<CardPlayFinishedEntry, bool>) (e => e.CardPlay.Card.Owner == card.Owner && e.CardPlay.Card.Type == CardType.Power)))))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Firewall firewall = this;
        await CreatureCmd.TriggerAnim(firewall.Owner.Creature, "Cast", firewall.Owner.Character.CastAnimDelay);
        int blockGains = (int) ((CalculatedVar) DynamicVars["CalculatedBlocks"]).Calculate(firewall.Owner.Creature);
        for (int i = 0; i < blockGains; ++i)
        {
            Decimal num = await CreatureCmd.GainBlock(firewall.Owner.Creature, firewall.DynamicVars.Block, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(1M);
    }
}