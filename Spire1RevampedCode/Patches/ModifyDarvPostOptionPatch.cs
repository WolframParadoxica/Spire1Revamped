using System.Reflection;
using HarmonyLib;
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

        if (darv.Owner!.RunState.Modifiers.Count > 0)
            return;
        var options = __result.ToList();
        switch (options[2].TextKey)
        {
            case "DARV.pages.INITIAL.options.DUSTY_TOME":
                break;
            default:
            {
                if (__instance.Owner!.Character is Ironclad)
                    options[2] = RelicOption<MarkOfPain>(darv: darv);
                if (__instance.Owner.Character is Silent)
                    options[2] = RelicOption<HoveringKite>(darv: darv);
                if (__instance.Owner.Character is Regent)
                    options[2] = RelicOption<Monocle>(darv: darv);
                if (__instance.Owner.Character is Necrobinder)
                    options[2] = RelicOption<BleedingAnvil>(darv: darv);
                if (__instance.Owner.Character is Defect)
                    options[2] = RelicOption<FrozenBattery>(darv: darv);
                break;
            }
        }

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