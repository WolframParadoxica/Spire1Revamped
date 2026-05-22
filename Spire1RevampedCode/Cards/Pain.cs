using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(CurseCardPool))]
public class Pain() : Spire1RevampedCard(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    public override bool CanBeGeneratedByModifiers => false;

    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain,CardKeyword.Unplayable];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new HpLossVar(1M)];
    
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        Pain pain = this;
        if (cardPlay.Card.Owner != pain.Owner)
            return;
        CardPile pile = this.Pile;
        if (pile.Type == PileType.Hand)
        {
            IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
                pain.Owner.Creature, 1M, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, (CardModel) pain);
        }
    }
}