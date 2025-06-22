using RimWorld.Planet;
using Verse;

namespace ExpandedIncidents.Settings;

internal class WorldComp(World world) : WorldComponent(world)
{
    public override void FinalizeInit(bool fromLoad)
    {
        base.FinalizeInit(fromLoad);
        Log.Message("Expanded Incident - Settings loaded");
        EI_ModSettings.ChangeDefPost();
    }
}