using UnityEngine;
using Verse;

namespace ExpandedIncidents;

public class Graphic_Invisible : Graphic
{
    public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
    {
    }

    public override Material MatAt(Rot4 rot, Thing thing = null)
    {
        return BaseContent.ClearMat;
    }
}