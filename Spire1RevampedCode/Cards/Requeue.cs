using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Requeue() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    
public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;

protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke),HoverTipFactory.Static(StaticHoverTip.Channeling)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Requeue requeue = this;
        if (requeue.Owner.PlayerCombatState.OrbQueue.Orbs.Count <= 0)
            return;
        var recurOrb = requeue.Owner.PlayerCombatState!.OrbQueue.Orbs.FirstOrDefault();
        await CreatureCmd.TriggerAnim(requeue.Owner.Creature, "Cast", requeue.Owner.Character.CastAnimDelay);
        await OrbCmd.EvokeNext(choiceContext, requeue.Owner);
        await Cmd.CustomScaledWait(0.1f, 0.25f);
        await OrbCmd.Channel(choiceContext, recurOrb, requeue.Owner);
        CardModel slimy = requeue.CombatState.CreateCard<Slimed>(requeue.Owner);
        if (requeue.IsUpgraded)
            slimy.EnergyCost.SetThisCombat(0);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(slimy, PileType.Discard, requeue.Owner));
        await Cmd.Wait(0.5f);
    }
}