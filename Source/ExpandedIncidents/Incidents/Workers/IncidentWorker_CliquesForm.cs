using System.Linq;
using RimWorld;
using Verse;

namespace ExpandedIncidents;

internal class IncidentWorker_CliquesForm : IncidentWorker
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        var pawn = map.mapPawns.FreeColonistsSpawned.RandomElement();
        if (pawn == null)
        {
            return false;
        }

        var enemies = from p in map.mapPawns.FreeColonistsSpawned
            where p != pawn && p.relations.OpinionOf(pawn) < -20 && pawn.relations.OpinionOf(p) < -20
            select p;
        if (!enemies.Any())
        {
            return false;
        }

        var enemy = enemies.RandomElement();
        pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Clique, enemy);
        enemy.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Clique, pawn);
        Find.LetterStack.ReceiveLetter("LetterLabelCliques".Translate(),
            "CliquesFormed".Translate(pawn.LabelShort, enemy.LabelShort), LetterDefOf.NegativeEvent, pawn);
        return true;
    }
}