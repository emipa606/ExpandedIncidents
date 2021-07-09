using System.Linq;
using RimWorld;
using Verse;

namespace ExpandedIncidents
{
    internal class IncidentWorker_Homesick : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map) parms.target;
            var list = (from p in map.mapPawns.FreeColonistsSpawned
                where p.Awake() &&
                      p.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Homesickness) == 0
                select p).ToList();
            if (list.Count == 0)
            {
                return false;
            }

            var pawn = list.RandomElement();
            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Homesickness);
            Find.LetterStack.ReceiveLetter("LetterLabelHomesick".Translate(),
                "ColonistHomesick".Translate(pawn.LabelShort, pawn.Named("PAWN")), LetterDefOf.NegativeEvent, pawn);
            return true;
        }
    }
}