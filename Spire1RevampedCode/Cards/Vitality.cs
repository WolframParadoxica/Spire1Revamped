using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Spire1Revamped.Spire1RevampedCode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class Vitality() : Spire1RevampedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new SummonVar(3M), new ("HPThreshold",9M)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var missingHp = Owner.Creature.MaxHp - Owner.Creature.CurrentHp;

        var interval = (int)DynamicVars["HPThreshold"].BaseValue;
        var triggers = missingHp / interval;

        for (var i = 0; i < triggers; ++i)
        {
            await OstyCmd.Summon(choiceContext, Owner, DynamicVars.Summon.BaseValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Summon.UpgradeValueBy(-2M);
        DynamicVars["HPThreshold"].UpgradeValueBy(-6M);
    }
}