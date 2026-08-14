using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Spire1Revamped.Spire1RevampedCode.Relics;

namespace Spire1Revamped.Spire1RevampedCode.Patches;

[HarmonyPatch(typeof(Darv), "AllPossibleOptions", MethodType.Getter)]
public class AddAllDarvOptionsPatch
{
    public static void Postfix(Darv? __instance, ref IEnumerable<EventOption> __result)
    {
        if (__instance is null)
            return;
        var options = __result.ToList();
        options.Add(RelicOption<MarkOfPain>(darv: __instance));
        options.Add(RelicOption<HoveringKite>(darv: __instance));
        options.Add(RelicOption<Monocle>(darv: __instance));
        options.Add(RelicOption<BleedingAnvil>(darv: __instance));
        options.Add(RelicOption<FrozenBattery>(darv: __instance));
        __result = options;
    }

    private static EventOption RelicOption<T>(string pageName = "INITIAL", Darv? darv = null) where T : RelicModel
    {
        return RelicOption(ModelDb.Relic<T>().ToMutable(), pageName, darv: darv!);
    }

    private static EventOption RelicOption(RelicModel relic, string pageName = "INITIAL", Darv? darv = null)
    {
        relic.AssertMutable();
        relic.Owner = darv!.Owner!;

        var textKey = $"{StringHelper.Slugify(darv.GetType().Name)}.pages.{pageName}.options.{relic.Id.Entry}";

        return EventOption.FromRelic(relic, darv, OnChosen, textKey);

        Task OnChosen()
        {
            try
            {
                var customDonePageProp = typeof(AncientEventModel).GetProperty("CustomDonePage",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                customDonePageProp!.SetValue(darv, "DARV.pages.DONE.POSITIVE.description");
        
                var doneMethod = typeof(AncientEventModel).GetMethod("Done",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                doneMethod!.Invoke(darv, null);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }
    }
}