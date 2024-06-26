﻿using HarmonyLib;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.ThreatDisabled))]
public static class Pawn_ThreatDisabledPatch
{
    [HarmonyPostfix]
    public static void IgnoreThief(Pawn __instance, ref bool __result)
    {
        __result = __result || __instance.health.hediffSet.HasHediff(HediffDefOfIncidents.Thief);
    }
}