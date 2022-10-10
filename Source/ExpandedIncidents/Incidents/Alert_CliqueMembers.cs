using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ExpandedIncidents;

public class Alert_CliqueMembers : Alert
{
    private IEnumerable<Pawn>[] cachedCliques;

    public Alert_CliqueMembers()
    {
        defaultLabel = "CliqueMembers".Translate();
        defaultPriority = AlertPriority.Critical;
    }

    private IEnumerable<Pawn> CliqueLeaders =>
        from c in PawnsFinder.AllMaps_FreeColonistsSpawned
        where c.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Clique) > 0
        select c;

    public override TaggedString GetExplanation()
    {
        var leaderNum = 0;
        if (cachedCliques == null || Find.TickManager.TicksGame % 200 == 0)
        {
            var currentLeaders = CliqueLeaders.ToList();
            var cliques = new IEnumerable<Pawn>[currentLeaders.Count];
            foreach (var leader in currentLeaders)
            {
                var members = (from c in PawnsFinder.AllMaps_FreeColonistsSpawned
                    where c == leader || c.Map == leader.Map && c.relations.OpinionOf(leader) > 20
                    select c).ToList();
                cliques[leaderNum] = members;
                var cliqueLeaderWhoMadeUpSomehow = (from c in members
                    where currentLeaders.Contains(c)
                    select c).FirstOrDefault();
                if (cliqueLeaderWhoMadeUpSomehow != null)
                {
                    //Kiss and make up.
                    leader.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                        ThoughtDefOfIncidents.Clique, cliqueLeaderWhoMadeUpSomehow);
                    cliqueLeaderWhoMadeUpSomehow.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                        ThoughtDefOfIncidents.Clique, leader);
                }

                leaderNum++;
            }

            cachedCliques = cliques;
        }

        var cliqueList = new StringBuilder();
        foreach (var pawns in cachedCliques)
        {
            var clique = (List<Pawn>)pawns;
            leaderNum = 0;
            foreach (var member in clique)
            {
                cliqueList.AppendLine(leaderNum == 0
                    ? "Leader: " + member.NameShortColored
                    : member.NameShortColored);
                leaderNum++;
            }

            cliqueList.AppendLine();
        }

        return "CliqueMembersDesc".Translate(cliqueList);
    }

    public override AlertReport GetReport()
    {
        if (CliqueLeaders.Count() < 2)
        {
            return AlertReport.Inactive;
        }

        return new AlertReport
        {
            active = true,
            culpritsPawns = CliqueLeaders.ToList()
        };
    }
}