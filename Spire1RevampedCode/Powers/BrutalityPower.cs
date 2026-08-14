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
  public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
  {
    if (target != Owner || result.UnblockedDamage <= 0 || CombatManager.Instance.IsOverOrEnding || Owner.IsDead) return;
    switch (DynamicVars["HPLossCount"].IntValue)
    {
      case 0:
        DynamicVars["HPLossCount"].UpgradeValueBy(1);
        break;
      case 1:
        DynamicVars["HPLossCount"].UpgradeValueBy(-1);
        Flash();
        for (var i = 0; (decimal) i < Amount; ++i) await CardPileCmd.Draw(choiceContext, Owner.Player!);
        break;
    }
    this.InvokeSecondAmountChanged();
  }

  public string GetSecondAmount() => $"{DynamicVars["HPLossCount"].IntValue}";
}