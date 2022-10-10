using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents;

[HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker", typeof(IncidentParms))]
public static class IncidentWorker_RaidEnemyPatch
{
    [HarmonyPostfix]
    public static void PopSaboteurs(bool __result, IncidentParms parms)
    {
        if (!__result || parms.target is not Map map || !map.IsPlayerHome)
        {
            return;
        }

        var saboteurs = (from p in map.mapPawns.FreeColonistsSpawned
            where p.health.hediffSet.HasHediff(HediffDefOfIncidents.Saboteur)
            select p).ToList();
        if (saboteurs.Count <= 0)
        {
            return;
        }

        var saboteur = saboteurs.RandomElement();
        if (!(Rand.Value < 0.33f))
        {
            return;
        }

        saboteur.health.hediffSet.hediffs.RemoveAll(h => h.def == HediffDefOfIncidents.Saboteur);
        saboteur.SetFaction(parms.faction);
        var enemyLord = saboteur.Map.lordManager.lords.Find(x => x.faction == parms.faction);
        enemyLord.ownedPawns.Add(saboteur);
        saboteur.mindState.duty = new PawnDuty(DutyDefOf.AssaultColony);
        Find.LetterStack.ReceiveLetter("LetterLabelSabotage".Translate(),
            "SaboteurRevealedFaction".Translate(saboteur.LabelShort, parms.faction.Name,
                saboteur.Named("PAWN")), LetterDefOf.ThreatBig, saboteur);
    }
}