using BaseLib.Abstracts;
using BaseLib.Extensions;
using Spire1Revamped.Spire1RevampedCode.Extensions;
using Godot;

namespace Spire1Revamped.Spire1RevampedCode.Powers;

public abstract class Spire1RevampedPower : CustomPowerModel
{
    //Loads from Spire1Revamped/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}