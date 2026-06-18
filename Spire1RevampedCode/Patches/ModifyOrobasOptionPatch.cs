using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Spire1Revamped.Spire1RevampedCode.Relics;

namespace Spire1Revamped.Spire1RevampedCode.Patches;

[HarmonyPatch(typeof(AncientEventModel), "GenerateInitialOptionsWrapper")]
public class ModifyOrobasOptionsPatch
{
    public static void Postfix(AncientEventModel __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (__instance is not Orobas orobas)
            return;

        if (orobas.Owner.RunState.Modifiers.Count > 0)
            return;

        List<EventOption> options = __result.ToList();
        MillenniumEgg millenniumEgg = (MillenniumEgg) ModelDb.Relic<MillenniumEgg>().ToMutable();
        
        if (__instance.Owner != null && millenniumEgg.SetupForPlayer(__instance.Owner) && __instance.Rng.NextFloat() < 0.3333333134651184)
            options[2] = RelicOption(millenniumEgg, "MillenniumEgg.eventDescription", orobas: orobas);
        
        __result = options;
    }
    
    protected static EventOption RelicOption<T>(string pageName = "INITIAL", string? customDonePage = null, Orobas orobas = null) where T : RelicModel
    {
        return RelicOption(ModelDb.Relic<T>().ToMutable(), pageName, orobas: orobas);
    }

    protected static EventOption RelicOption(RelicModel relic, string pageName = "INITIAL", string? customDonePage = null, Orobas orobas = null)
    {
        relic.AssertMutable();
        relic.Owner = orobas.Owner;

        string textKey = $"{StringHelper.Slugify(orobas.GetType().Name)}.pages.{pageName}.options.{relic.Id.Entry}";
        return EventOption.FromRelic(relic, orobas, OnChosen, textKey);

        async Task OnChosen()
        {
            RelicModel relicModel = await RelicCmd.Obtain(relic, orobas.Owner);
            PropertyInfo customDonePageProp = typeof(AncientEventModel).GetProperty("CustomDonePage",
                BindingFlags.NonPublic | BindingFlags.Instance);
            customDonePageProp.SetValue(orobas, "OROBAS.pages.DONE.POSITIVE.description");
        
            MethodInfo doneMethod = typeof(AncientEventModel).GetMethod("Done",
                BindingFlags.NonPublic | BindingFlags.Instance);
            doneMethod.Invoke(orobas, null);

        }
    }
}