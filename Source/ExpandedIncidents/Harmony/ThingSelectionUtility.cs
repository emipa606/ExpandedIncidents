using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;

namespace ExpandedIncidents.Harmony
{
    [HarmonyPatch(typeof(ThingSelectionUtility), "SelectableByMapClick")]
    public static class ThingSelectionUtilityPatch
    {
        [HarmonyPostfix]
        public static void ThiefException(ref bool __result, Thing t)
        {
            if (t is Pawn && ((Pawn)t).health.hediffSet.HasHediff(HediffDefOfIncidents.Thief))
            {
                __result = false;
            }
        }
    }
}
