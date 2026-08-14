using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]
public class Concentrate() : Spire1RevampedCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public static Dictionary<CardModel, int> DiscardEnergyMap { get; } = new();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Shiv>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var selectedCards = (await CardSelectCmd.FromHandForDiscard(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 2), null, this)).ToList();

        foreach (var original in selectedCards)
        {
            var calculatedEnergy = original.EnergyCost.CostsX ? original.Owner.PlayerCombatState?.Energy ?? 0 : original.EnergyCost.GetResolved() is -1 ? 0 : original.EnergyCost.GetWithModifiers(CostModifiers.All);
            DiscardEnergyMap[original] = calculatedEnergy;
        }

        try { await CardCmd.Discard(choiceContext, selectedCards); }
        finally { foreach (var original in selectedCards) DiscardEnergyMap.Remove(original); }

        foreach (var original in selectedCards)
        {
            if (original.Pile is null) continue;
            CardModel card = CombatState!.CreateCard<Shiv>(Owner);
            await CardCmd.Transform(original, card);
        }
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDiscarded), typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardModel))]
class Hook_AfterCardDiscarded_Patch
{
    [HarmonyPostfix]
    private static void PostDiscardHook(ref Task __result, CardModel card)
    {
        __result = PostDiscardHookAsync(__result, card);
    }

    private static async Task PostDiscardHookAsync(Task? originalTask, CardModel card)
    {
        if (originalTask is not null) await originalTask;
        if (Concentrate.DiscardEnergyMap.TryGetValue(card, out var energy))
        {
            try { if (energy > 0) await PlayerCmd.GainEnergy(energy, card.Owner); }
            finally { Concentrate.DiscardEnergyMap.Remove(card); }
        }
    }
}