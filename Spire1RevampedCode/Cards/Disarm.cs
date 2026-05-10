using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Spire1Revamped.Spire1RevampedCode.Cards;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Disarm() : Spire1RevampedCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Disarm disarm = this;
        await CreatureCmd.TriggerAnim(disarm.Owner.Creature, "Cast", disarm.Owner.Character.CastAnimDelay);
        DisarmPower disarmPower = await PowerCmd.Apply<DisarmPower>(choiceContext, disarm.Owner.Creature, 1M, disarm.Owner.Creature, (CardModel) disarm);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}