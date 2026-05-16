using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(RegentCardPool))]
public class FromTheAether() : Spire1RevampedCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        FromTheAether fromTheAether = this;
        await CreatureCmd.TriggerAnim(fromTheAether.Owner.Creature, "Cast", fromTheAether.Owner.Character.CastAnimDelay);
        FromTheAetherPower fromTheAetherPower = await PowerCmd.Apply<FromTheAetherPower>(choiceContext, fromTheAether.Owner.Creature, 1M, fromTheAether.Owner.Creature, (CardModel) fromTheAether);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}