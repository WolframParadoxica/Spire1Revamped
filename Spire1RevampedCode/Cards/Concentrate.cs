using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(SilentCardPool))]
public class Concentrate() : Spire1RevampedCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Shiv>()];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new EnergyVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Concentrate source = this;
        foreach (CardModel original in (await CardSelectCmd.FromHand(choiceContext, source.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt,2, 2), (Func<CardModel, bool>) null, (AbstractModel) source)).ToList<CardModel>())
        {
            await CardCmd.Discard(choiceContext, original);
            int energyAtTimeOfPlay = source.Owner.PlayerCombatState.Energy;
            CardModel card = (CardModel) source.CombatState.CreateCard<Shiv>(source.Owner);
            if (Traverse.Create(original).Property("HasEnergyCostX").GetValue<bool>())
                await PlayerCmd.GainEnergy(energyAtTimeOfPlay, source.Owner);
            else if (original.EnergyCost.GetResolved() == -1)
                break;
            else
                await PlayerCmd.GainEnergy(original.EnergyCost.GetResolved(), source.Owner);
            if (original.Pile == null)
                return;
            CardPileAddResult ? nullable = await CardCmd.Transform(original, card);
        }
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}