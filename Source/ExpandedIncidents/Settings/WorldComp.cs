using RimWorld.Planet;
using Verse;

namespace ExpandedIncidents.Settings;

internal class WorldComp : WorldComponent
{
    public WorldComp(World world) : base(world)
    {
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        Log.Message("Expanded Incident - Settings loaded");
        EI_ModSettings.ChangeDefPost();
    }
}