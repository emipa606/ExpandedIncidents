using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents
{
    class IncidentWorker_Homesick : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Pawn> list = (from p in map.mapPawns.FreeColonistsSpawned
                               where p.Awake() && p.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Homesickness) == 0
                               select p).ToList();
            if (list.Count == 0)
            {
                return false;
            }
            Pawn pawn = list.RandomElement();
            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Homesickness, null);
            Find.LetterStack.ReceiveLetter("LetterLabelHomesick".Translate(), "ColonistHomesick".Translate(pawn.LabelShort, pawn.Named("PAWN")), LetterDefOf.NegativeEvent, pawn, null);
            return true;
        }
    }
}
