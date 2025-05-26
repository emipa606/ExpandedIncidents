using HarmonyLib;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(PawnUIOverlay), nameof(PawnUIOverlay.DrawPawnGUIOverlay))]
public static class PawnUIOverlay_DrawPawnGUIOverlay
{
    public static bool Prefix(Pawn ___pawn)
    {
        return ___pawn == null || !___pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.Thief);
    }
}