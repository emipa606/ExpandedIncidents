using RimWorld.Planet;

namespace ExpandedIncidents.Settings;

internal class WorldComp(World world) : WorldComponent(world)
{
    public override void FinalizeInit(bool fromLoad)
    {
        base.FinalizeInit(fromLoad);
        EI_ModSettings.ChangeDef();
    }
}