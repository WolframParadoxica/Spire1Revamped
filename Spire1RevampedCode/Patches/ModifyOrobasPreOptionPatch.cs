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
    public static void Postfix(Orobas? __instance, ref IEnumerable<EventOption> __result)
    {
        if (__instance is null)
            return;
        var options = __result.ToList();
        options.Add(RelicOption<MillenniumEgg>(orobas: __instance));
        __result = options;
    }

    private static EventOption RelicOption<T>(string pageName = "INITIAL", Orobas? orobas = null) where T : RelicModel
    {
        return RelicOption(ModelDb.Relic<T>().ToMutable(), pageName, orobas: orobas);
    }

    private static EventOption RelicOption(RelicModel relic, string pageName = "INITIAL", Orobas? orobas = null)
    {
        relic.AssertMutable();
        relic.Owner = orobas!.Owner!;

        var textKey = $"{StringHelper.Slugify(orobas.GetType().Name)}.pages.{pageName}.options.{relic.Id.Entry}";
        return EventOption.FromRelic(relic, orobas, OnChosen, textKey);

        async Task OnChosen()
        {
            await RelicCmd.Obtain(relic, orobas.Owner!);
            var customDonePageProp = typeof(AncientEventModel).GetProperty("CustomDonePage",
                BindingFlags.NonPublic | BindingFlags.Instance);
            customDonePageProp!.SetValue(orobas, "OROBAS.pages.DONE.POSITIVE.description");

            var doneMethod = typeof(AncientEventModel).GetMethod("Done",
                BindingFlags.NonPublic | BindingFlags.Instance);
            doneMethod!.Invoke(orobas, null);
        }
    }
}