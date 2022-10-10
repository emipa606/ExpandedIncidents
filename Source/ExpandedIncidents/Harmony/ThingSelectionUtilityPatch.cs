using HarmonyLib;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(ThingSelectionUtility), "SelectableByMapClick")]
public static class ThingSelectionUtilityPatch
{
    [HarmonyPostfix]
    public static void ThiefException(ref bool __result, Thing t)
    {
        if (t is Pawn pawn && pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.Thief))
        {
            __result = false;
        }
    }
}