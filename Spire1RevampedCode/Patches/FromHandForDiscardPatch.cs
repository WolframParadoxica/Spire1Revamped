using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Patches;

[HarmonyPatch]
public static class CardSelectFromHandPatch
{
    private static bool _isFromDiscard = false;
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromHandForDiscard))]
    [HarmonyPrefix]
    static void Prefix_Discard(out bool __state)
    {
        __state = _isFromDiscard;
        _isFromDiscard = true;
    }
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromHandForDiscard))]
    [HarmonyPostfix]
    static void Postfix_Discard(bool __state)
    {
        _isFromDiscard = __state;
    }
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromHand))]
    [HarmonyPrefix]
    static void Prefix_FromHand(ref CardSelectorPrefs prefs)
    {
        if (!_isFromDiscard || prefs.ShouldGlowGold is null) return;
        var originalGlow = prefs.ShouldGlowGold;
        prefs.ShouldGlowGold = c =>
        {
            if (originalGlow(c)) return true;
            if (!c.IsSlyThisTurn) return false;
            return !c.Keywords.Contains(CardKeyword.Unplayable);
        };
    }
}