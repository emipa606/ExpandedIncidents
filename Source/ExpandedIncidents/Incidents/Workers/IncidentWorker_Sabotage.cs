using System.Linq;
using RimWorld;
using Verse;

namespace ExpandedIncidents;

internal class IncidentWorker_Sabotage : IncidentWorker
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        var list = (from p in map.mapPawns.AllPawnsSpawned
            where p.RaceProps.Humanlike && p.Faction.IsPlayer &&
                  p.health.hediffSet.HasHediff(HediffDefOfIncidents.PotentialSaboteur)
            select p).ToList();
        if (list.Count < 3)
        {
            return false;
        }

        var pawn = list.RandomElement();
        var hediff = HediffMaker.MakeHediff(HediffDefOfIncidents.Saboteur, pawn);
        pawn.health.AddHediff(hediff);
        pawn.health.hediffSet.hediffs.RemoveAll(h => h.def == HediffDefOfIncidents.PotentialSaboteur);
        return true;
    }
}