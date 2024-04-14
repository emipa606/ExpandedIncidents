using HarmonyLib;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(Pawn_Ownership), nameof(Pawn_Ownership.Notify_ChangedGuestStatus))]
public static class Notify_ChangedGuestStatusPatch
{
    [HarmonyPostfix]
    public static void MakePotentialSaboteur(ref Pawn ___pawn)
    {
        if (___pawn is not { IsColonist: true } ||
            ___pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.PotentialSaboteur) || !Rand.Chance(0.33f))
        {
            return;
        }

        var potentialSaboteur = HediffMaker.MakeHediff(HediffDefOfIncidents.PotentialSaboteur, ___pawn);
        ___pawn.health.AddHediff(potentialSaboteur);
    }
}