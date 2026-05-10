#nullable enable
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public abstract class DisarmedPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => (AbstractModel) ModelDb.Card<Disarm>();

    protected override bool IsPositive => false;
}