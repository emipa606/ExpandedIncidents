using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(Pawn), "CheckAcceptArrest")]
public static class Pawn_AcceptArrestPatch
{
    [HarmonyPrefix]
    public static bool RevealSaboteur(Pawn __instance, Pawn arrester)
    {
        if (!__instance.health.hediffSet.HasHediff(HediffDefOfIncidents.Saboteur))
        {
            return true;
        }

        __instance.health.hediffSet.hediffs.RemoveAll(h => h.def == HediffDefOfIncidents.Saboteur);
        var faction = Find.FactionManager.RandomEnemyFaction();
        __instance.SetFaction(faction);
        var thisPawn = new List<Pawn> { __instance };
        var parms = new IncidentParms
        {
            faction = faction,
            spawnCenter = __instance.Position,
            raidStrategy = RaidStrategyDefOf.ImmediateAttack
        };
        parms.raidStrategy.Worker.MakeLords(parms, thisPawn);
        __instance.Map.avoidGrid.Regenerate();
        LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
        if (faction != null)
        {
            Find.LetterStack.ReceiveLetter("LetterLabelSabotage".Translate(),
                "SaboteurRevealedFaction".Translate(__instance.LabelShort, faction.Name,
                    __instance.Named("PAWN")), LetterDefOf.ThreatBig, __instance);
        }
        else
        {
            Find.LetterStack.ReceiveLetter("LetterLabelSabotage".Translate(),
                "SaboteurRevealed".Translate(__instance.LabelShort, __instance.Named("PAWN")),
                LetterDefOf.ThreatBig, __instance);
        }

        return true;
    }
}