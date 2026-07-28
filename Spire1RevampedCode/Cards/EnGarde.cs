using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(RegentCardPool))]
public class EnGarde() : Spire1RevampedCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new ForgeVar(3),(DynamicVar) new PowerVar<EnGardePower>(1M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> hoverTips = [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
            hoverTips.AddRange(HoverTipFactory.FromForge());
            return hoverTips;
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        EnGarde enGarde = this;
        await CreatureCmd.TriggerAnim(enGarde.Owner.Creature, "Cast", enGarde.Owner.Character.CastAnimDelay);
        EnGardePower? enGardePower = await PowerCmd.Apply<EnGardePower>(choiceContext, enGarde.Owner.Creature, DynamicVars.Power<EnGardePower>().BaseValue, enGarde.Owner.Creature, (CardModel) enGarde);
        IEnumerable<SovereignBlade> sovereignBlades = await ForgeCmd.Forge((Decimal) enGarde.DynamicVars.Forge.IntValue, enGarde.Owner, (AbstractModel) enGarde);
    }

    protected override void OnUpgrade() => this.DynamicVars.Forge.UpgradeValueBy(4M);
}