using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Events;
using Spire1Revamped.Spire1RevampedCode.Relics;

namespace Spire1Revamped.Spire1RevampedCode.Patches;

[HarmonyPatch(typeof(AncientEventModel), "GenerateInitialOptionsWrapper")]
public class ModifyDarvOptionsPatch
{
    public static void Postfix(AncientEventModel __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (__instance is not Darv darv)
            return;
            
        if (darv.Owner.RunState.Modifiers.Count > 0)
            return;

        List<EventOption> options = __result.ToList();
        if (options[2].TextKey != "DARV.pages.INITIAL.options.DUSTY_TOME")
        {
            if (__instance.Owner.Character is Ironclad)
                options[2] = RelicOption<MarkOfPain>(customDonePage: "DARV.pages.DONE.POSITIVE.description", darv: darv);
            if (__instance.Owner.Character is Silent)
                options[2] = RelicOption<HoveringKite>(customDonePage: "DARV.pages.DONE.POSITIVE.description", darv: darv);
            if (__instance.Owner.Character is Regent)
                options[2] = RelicOption<Monocle>(customDonePage: "DARV.pages.DONE.POSITIVE.description", darv: darv);
            if (__instance.Owner.Character is Necrobinder)
                options[2] = RelicOption<BleedingAnvil>(customDonePage: "DARV.pages.DONE.POSITIVE.description", darv: darv);
            if (__instance.Owner.Character is Defect)
                options[2] = RelicOption<FrozenBattery>(customDonePage: "DARV.pages.DONE.POSITIVE.description", darv: darv);
        }
        
        __result = options;
    }
    
    protected static EventOption RelicOption<T>(string pageName = "INITIAL", string? customDonePage = null, Darv darv = null) where T : RelicModel
    {
        return RelicOption(ModelDb.Relic<T>().ToMutable(), pageName, darv: darv);
    }

    protected static EventOption RelicOption(RelicModel relic, string pageName = "INITIAL", string? customDonePage = null, Darv darv = null)
    {
        relic.AssertMutable();
        relic.Owner = darv.Owner;

        string textKey = $"{StringHelper.Slugify(darv.GetType().Name)}.pages.{pageName}.options.{relic.Id.Entry}";
        return EventOption.FromRelic(relic, darv, OnChosen, textKey);

        async Task OnChosen()
        {
            RelicModel relicModel = await RelicCmd.Obtain(relic, darv.Owner);
            PropertyInfo customDonePageProp = typeof(AncientEventModel).GetProperty("CustomDonePage",
                BindingFlags.NonPublic | BindingFlags.Instance);
            customDonePageProp.SetValue(darv, "DARV.pages.DONE.POSITIVE.description");
        
            MethodInfo doneMethod = typeof(AncientEventModel).GetMethod("Done",
                BindingFlags.NonPublic | BindingFlags.Instance);
            doneMethod.Invoke(darv, null);

        }
    }
}