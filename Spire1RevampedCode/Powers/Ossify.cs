using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public sealed class OssifyPower : Spire1RevampedPower
{
  public override PowerType Type => PowerType.Buff;

  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.SummonStatic)];
  
  public override decimal ModifySummonAmount(Player player, decimal amount1, AbstractModel? abstractModel)
  {
    if (this.Owner != player.Creature || abstractModel is not CardModel)
      return amount1;
    return amount1 + this.Owner.GetPowerAmount<DexterityPower>()*this.Amount;
  }
  
  public override decimal ModifyPowerAmountGiven(
    PowerModel power,
    Creature giver,
    decimal amount,
    Creature? target,
    CardModel? cardSource)
  {
    return power is not SummonNextTurnPower ? amount : amount + this.Owner.GetPowerAmount<DexterityPower>()*this.Amount;
  }
}