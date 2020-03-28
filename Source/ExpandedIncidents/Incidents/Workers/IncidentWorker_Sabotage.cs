using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents
{
    class IncidentWorker_Sabotage : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Pawn> list = (from p in map.mapPawns.AllPawnsSpawned
                               where p.RaceProps.Humanlike && p.Faction.IsPlayer && p.health.hediffSet.HasHediff(HediffDefOfIncidents.PotentialSaboteur)
                               select p).ToList();
            if (list.Count < 3)
            {
                return false;
            }
            Pawn pawn = list.RandomElement();
            Hediff hediff = HediffMaker.MakeHediff(HediffDefOfIncidents.Saboteur, pawn, null);
            pawn.health.AddHediff(hediff, null, null);
            pawn.health.hediffSet.hediffs.RemoveAll((Hediff h) => h.def == HediffDefOfIncidents.PotentialSaboteur);
            return true;
        }
    }
}
