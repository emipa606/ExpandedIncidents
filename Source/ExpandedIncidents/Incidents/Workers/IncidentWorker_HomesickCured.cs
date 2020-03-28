using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents
{
    class IncidentWorker_HomesickCured : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Pawn> list = (from p in map.mapPawns.FreeColonistsSpawned
                               where p.Awake() && p.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Homesickness) > 0
                               select p).ToList();
            if (list.Count < (map.mapPawns.FreeColonistsSpawned.Count() / 2))
            {
                return false;
            }
            foreach(Pawn colonist in list)
            {
                colonist.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOfIncidents.Homesickness);
            }
            Find.LetterStack.ReceiveLetter("LetterLabelHomesickCured".Translate(), "HomesickCured".Translate(), LetterDefOf.PositiveEvent, null);
            return true;
        }
    }
}
