//using System.Linq;
//using RimWorld;
//using Verse;

//namespace ExpandedIncidents;

//internal class IncidentWorker_HomesickCured : IncidentWorker
//{
//    protected override bool TryExecuteWorker(IncidentParms parms)
//    {
//        var map = (Map)parms.target;
//        var list = (from p in map.mapPawns.FreeColonistsSpawned
//            where p.Awake() && p.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Homesickness) >
//                0
//            select p).ToList();
//        if (list.Count < map.mapPawns.FreeColonistsSpawned.Count / 2)
//        {
//            return false;
//        }

//        foreach (var colonist in list)
//        {
//            colonist.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOfIncidents.Homesickness);
//        }

//        Find.LetterStack.ReceiveLetter("LetterLabelHomesickCured".Translate(), "HomesickCured".Translate(),
//            LetterDefOf.PositiveEvent);
//        return true;
//    }
//}

