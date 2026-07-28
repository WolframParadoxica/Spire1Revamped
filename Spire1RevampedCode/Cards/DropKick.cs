using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Spire1Revamped.Spire1RevampedCode.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(IroncladCardPool))]

public class DropKick() : Spire1RevampedCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new DamageVar(5M, ValueProp.Move),
        (DynamicVar) new CardsVar(1)
    ];

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            return this.CombatState != null && this.CombatState.HittableEnemies.Any<Creature>((Func<Creature, bool>) (e =>
            {
                MonsterModel monster = e.Monster;
                return monster != null && e.GetPowerAmount<VulnerablePower>()>0;
            }));
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(),HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DropKick dropKick = this;
        VulnerablePower? power = cardPlay.Target.GetPower<VulnerablePower>();
        AttackCommand attackCommand = await DamageCmd.Attack(dropKick.DynamicVars.Damage.BaseValue).FromCard((CardModel) dropKick, cardPlay).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        IEnumerable<CardModel> cardModels = await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.BaseValue, this.Owner);
        if (power == null)
            return;
        FreeCardPower? freeCardPower = await PowerCmd.Apply<FreeCardPower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature, (CardModel) this);
        ExhaustCardPower? exhaustCardPower = await PowerCmd.Apply<ExhaustCardPower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature, (CardModel) this);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3M);
}