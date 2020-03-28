using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ExpandedIncidents.Harmony
{
    [HarmonyPatch(typeof(PawnUIOverlay), "DrawPawnGUIOverlay")]
    public static class PawnUIOverlayPatch
    {
        [HarmonyPrefix]
        public static bool ThiefException(PawnUIOverlay __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null && pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.Thief))
            {
                return false;
            }
            return true;
        }
    }
}
