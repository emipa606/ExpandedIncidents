using HarmonyLib;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(Pawn_Ownership), "Notify_ChangedGuestStatus")]
public static class Notify_ChangedGuestStatusPatch
{
    [HarmonyPostfix]
    public static void MakePotentialSaboteur(Pawn_Ownership __instance)
    {
        var pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        if (pawn == null || !pawn.IsColonist ||
            pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.PotentialSaboteur) || !Rand.Chance(0.33f))
        {
            return;
        }

        var potentialSaboteur = HediffMaker.MakeHediff(HediffDefOfIncidents.PotentialSaboteur, pawn);
        pawn.health.AddHediff(potentialSaboteur);
    }
}