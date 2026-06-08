using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Spire1Revamped.Spire1RevampedCode.Relics;

namespace Spire1Revamped.Spire1RevampedCode.Patches;

[HarmonyPatch(typeof(Orobas), "AllPossibleOptions", MethodType.Getter)]
public class AddAllOrobasOptionsPatch
{
    public static void Postfix(Orobas __instance, ref IEnumerable<EventOption> __result)
    {
        if (__instance is not Orobas orobas)
            return;
        List<EventOption> options = __result.ToList();
        options.Add(RelicOption<MillenniumEgg>(customDonePage: "OROBAS.pages.DONE.POSITIVE.description", orobas: orobas));
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
        //string textKey = orobas.OptionKey(pageName, relic.Id.Entry);
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