using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Spire1Revamped.Spire1RevampedCode.Cards;
using Spire1Revamped.Spire1RevampedCode.Extensions;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public class DisarmedPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Disarm>();
    
    public string CustomPackedIconPath => "disarmed_power.png".PowerImagePath();
    public string CustomBigIconPath => "disarmed_power.png".BigPowerImagePath();

    protected override bool IsPositive => false;
}