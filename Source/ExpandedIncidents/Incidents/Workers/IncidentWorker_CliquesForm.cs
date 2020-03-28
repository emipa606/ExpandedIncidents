using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents
{
    class IncidentWorker_CliquesForm : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Pawn pawn = map.mapPawns.FreeColonistsSpawned.RandomElement();
            if (pawn == null)
            {
                return false;
            }
            IEnumerable<Pawn> enemies = (from p in map.mapPawns.FreeColonistsSpawned
                                  where p != pawn && p.relations.OpinionOf(pawn) < -20 && pawn.relations.OpinionOf(p) < -20
                                  select p);
            if (enemies.Count() == 0)
            {
                return false;
            }
            Pawn enemy = enemies.RandomElement<Pawn>();
            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Clique, enemy);
            enemy.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Clique, pawn);
            Find.LetterStack.ReceiveLetter("LetterLabelCliques".Translate(), "CliquesFormed".Translate(pawn.LabelShort, enemy.LabelShort), LetterDefOf.NegativeEvent, pawn, null);
            return true;
        }
    }
}
