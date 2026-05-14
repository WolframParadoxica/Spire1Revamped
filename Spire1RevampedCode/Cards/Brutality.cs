using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Brutality() : Spire1RevampedCard(0,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new HpLossVar(1M)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Brutality brutality = this;
        await CreatureCmd.TriggerAnim(brutality.Owner.Creature, "Cast", brutality.Owner.Character.CastAnimDelay);
        BrutalityPower brutalityPower = await PowerCmd.Apply<BrutalityPower>(choiceContext, brutality.Owner.Creature, 1M, brutality.Owner.Creature, (CardModel) brutality);
        VfxCmd.PlayOnCreatureCenter(brutality.Owner.Creature, "vfx/vfx_bloody_impact");
        IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, brutality.Owner.Creature, 1M, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, (CardModel) brutality);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}