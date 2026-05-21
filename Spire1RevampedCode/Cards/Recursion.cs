using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Recursion() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    
public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;

protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke),HoverTipFactory.Static(StaticHoverTip.Channeling)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Recursion recursion = this;
        if (recursion.Owner.PlayerCombatState.OrbQueue.Orbs.Count <= 0)
            return;
        var recurOrb = recursion.Owner.PlayerCombatState!.OrbQueue.Orbs.FirstOrDefault();
        await CreatureCmd.TriggerAnim(recursion.Owner.Creature, "Cast", recursion.Owner.Character.CastAnimDelay);
        await OrbCmd.EvokeNext(choiceContext, recursion.Owner);
        await Cmd.CustomScaledWait(0.1f, 0.25f);
        await OrbCmd.Channel(choiceContext, recurOrb, recursion.Owner);
    }

    protected override void OnUpgrade() => this.RemoveKeyword(CardKeyword.Exhaust);
}