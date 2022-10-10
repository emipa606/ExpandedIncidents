using HarmonyLib;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(PawnUIOverlay), "DrawPawnGUIOverlay")]
public static class PawnUIOverlayPatch
{
    [HarmonyPrefix]
    public static bool ThiefException(PawnUIOverlay __instance)
    {
        var pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        return pawn == null || !pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.Thief);
    }
}