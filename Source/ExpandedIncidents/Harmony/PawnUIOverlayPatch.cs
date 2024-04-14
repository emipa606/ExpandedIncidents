using HarmonyLib;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(PawnUIOverlay), nameof(PawnUIOverlay.DrawPawnGUIOverlay))]
public static class PawnUIOverlayPatch
{
    [HarmonyPrefix]
    public static bool ThiefException(Pawn ___pawn)
    {
        return ___pawn == null || !___pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.Thief);
    }
}