using BaseLib.Abstracts;
using BaseLib.Extensions;
using Spire1Revamped.Spire1RevampedCode.Extensions;
using Godot;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

public abstract class Spire1RevampedRelic : CustomRelicModel
{
    //Spire1Revamped/images/relics
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();

    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}