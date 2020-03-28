using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace ExpandedIncidents.Harmony
{
    [HarmonyPatch(typeof(RimWorld.Pawn_Ownership), "Notify_ChangedGuestStatus")]
    public static class Notify_ChangedGuestStatusPatch
    {
        [HarmonyPostfix]
        public static void MakePotentialSaboteur(RimWorld.Pawn_Ownership __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null && pawn.IsColonist && !pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.PotentialSaboteur) && Rand.Chance(0.33f))
            {
                Hediff potentialSaboteur = HediffMaker.MakeHediff(HediffDefOfIncidents.PotentialSaboteur, pawn);
                pawn.health.AddHediff(potentialSaboteur);
            }
        }
    }
}
