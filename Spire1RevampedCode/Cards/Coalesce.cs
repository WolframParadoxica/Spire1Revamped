#nullable enable
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(RegentCardPool))]
public class Coalesce() : Spire1RevampedCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new StarsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Coalesce coalesce = this;
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainStars(this.DynamicVars.Stars.BaseValue, this.Owner);
        CardModel copy = coalesce.CreateClone();
        copy.EnergyCost.AddThisCombat(1);
        copy.DynamicVars.Stars.UpgradeValueBy(1M);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, this.Owner, CardPilePosition.Random), 2.2f);
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Retain);
}