using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class BrutalityPower : Spire1RevampedPower, IHasSecondAmount
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("HPLossCount", 0)];
  public override async Task AfterDamageReceived(
    PlayerChoiceContext choiceContext,
    Creature target,
    DamageResult result,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource)
  {
    BrutalityPower brutalityPower = this;
    if (target != brutalityPower.Owner || result.UnblockedDamage <= 0 || CombatManager.Instance.IsOverOrEnding || brutalityPower.Owner.IsDead)
      return;
    switch (DynamicVars["HPLossCount"].IntValue)
    {
      case 0:
        DynamicVars["HPLossCount"].UpgradeValueBy(1);
        break;
      case 1:
        DynamicVars["HPLossCount"].UpgradeValueBy(-1);
        this.Flash();
        for (int i = 0; (Decimal) i < this.Amount; ++i)
        {
          CardModel cardModel = await CardPileCmd.Draw(choiceContext, this.Owner.Player);
        }
        break;
    }
    this.InvokeSecondAmountChanged();
  }

  public string GetSecondAmount() => $"{DynamicVars["HPLossCount"].IntValue}";
}