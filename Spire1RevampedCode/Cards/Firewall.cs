using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]

public class Firewall() : Spire1RevampedCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public const string _blockRepeatsKey = "BlockRepeats";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new BlockVar(7M, ValueProp.Move),
        (DynamicVar) new ("BlockRepeats", 1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Firewall firewall = this;
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);
        int blockGains = (int) DynamicVars["BlockRepeats"].BaseValue;
        for (int i = 0; i < blockGains; ++i)
        {
            Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, cardPlay);
        }

        if (cardPlay.IsLastInSeries)
        {
            DynamicVars["BlockRepeats"].BaseValue = 1;
        }
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner || !CombatManager.Instance.IsInProgress || cardPlay.Card.Type != CardType.Power)
            return Task.CompletedTask;
        ++DynamicVars["BlockRepeats"].BaseValue
        ;
        return Task.CompletedTask;
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(2M);
    }
}