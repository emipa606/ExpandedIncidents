using HarmonyLib;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(Building_Door), nameof(Building_Door.PawnCanOpen))]
public static class Building_Door_CanOpenPatch
{
    [HarmonyPostfix]
    public static void AllowThievesToOpen(ref bool __result, Pawn p)
    {
        __result = __result || p?.health != null && p.health.hediffSet.HasHediff(HediffDefOfIncidents.Thief);
    }
}