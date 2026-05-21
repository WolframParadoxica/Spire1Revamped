using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Reduplicate() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    
public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;

protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke),HoverTipFactory.Static(StaticHoverTip.Channeling)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Reduplicate reduplicate = this;
        if (reduplicate.Owner.PlayerCombatState.OrbQueue.Orbs.Count <= 0)
            return;
        var recurOrb = reduplicate.Owner.PlayerCombatState!.OrbQueue.Orbs.FirstOrDefault();
        
        await OrbCmd.Passive(choiceContext, recurOrb, (Creature) null);
        await Cmd.CustomScaledWait(0.1f, 0.25f);
        if (reduplicate.IsUpgraded)
        {
            await OrbCmd.Passive(choiceContext, recurOrb, (Creature) null);
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
        
        var recurOrbCopy = ModelDb.GetById<OrbModel>(ModelDb.GetId(recurOrb.GetType())).ToMutable();
        FieldInfo? evokeValField = AccessTools.Field(recurOrbCopy.GetType(), "_evokeVal");
        if(evokeValField != null)  evokeValField.SetValue(recurOrbCopy, evokeValField.GetValue(recurOrb));
        FieldInfo? passiveValField = AccessTools.Field(recurOrbCopy.GetType(), "_passiveVal");
        if (passiveValField != null) passiveValField.SetValue(recurOrbCopy, passiveValField.GetValue(recurOrb));
        
        await CreatureCmd.TriggerAnim(reduplicate.Owner.Creature, "Cast", reduplicate.Owner.Character.CastAnimDelay);
        if (reduplicate.IsUpgraded)
        {
            await OrbCmd.EvokeNext(choiceContext, reduplicate.Owner, false);
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        };
        await OrbCmd.EvokeNext(choiceContext, reduplicate.Owner);
        await Cmd.CustomScaledWait(0.1f, 0.25f);
        await OrbCmd.Channel(choiceContext, recurOrb, reduplicate.Owner);
        
        if (reduplicate.IsUpgraded)
            await OrbCmd.Channel(choiceContext, recurOrbCopy, Owner);
    }

    protected override void OnUpgrade()
    {

    }
    
}