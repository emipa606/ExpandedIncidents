using System;
using RimWorld.Planet;
using Verse;

namespace ExpandedIncidents.Settings
{
    // Token: 0x0200000F RID: 15
    internal class WorldComp : WorldComponent
    {
        public WorldComp(World world) : base(world)
        {
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Log.Message("Expanded Incident - Settings loaded", false);
            EI_ModSettings.ChangeDefPost();
        }
    }
}
