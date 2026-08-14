using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Recursion() : Spire1RevampedCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke),HoverTipFactory.Static(StaticHoverTip.Channeling)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.PlayerCombatState!.OrbQueue.Orbs is not [var recurOrb, ..]) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await OrbCmd.Passive(choiceContext, recurOrb, null);
        await Cmd.CustomScaledWait(0.1f, 0.25f);
        if (IsUpgraded) { await OrbCmd.Passive(choiceContext, recurOrb, null); await Cmd.CustomScaledWait(0.1f, 0.25f); }
        var recurOrbCopy = (OrbModel)recurOrb.MutableClone();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (IsUpgraded) { await OrbCmd.EvokeNext(choiceContext, Owner, false); await Cmd.CustomScaledWait(0.1f, 0.25f); }
        await OrbCmd.EvokeNext(choiceContext, Owner);
        await Cmd.CustomScaledWait(0.1f, 0.25f);
        await OrbCmd.Channel(choiceContext, recurOrb, Owner);
        if (IsUpgraded) await OrbCmd.Channel(choiceContext, recurOrbCopy, Owner);
    }
}