using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ExpandedIncidents
{
    public class Alert_CliqueMembers : Alert
    {
        public Alert_CliqueMembers()
        {
            this.defaultLabel = "CliqueMembers".Translate();
            this.defaultPriority = AlertPriority.Critical;
        }

        public override TaggedString GetExplanation()
        {
            int leaderNum = 0;
            if (cachedCliques == null || Find.TickManager.TicksGame % 200 == 0)
            {
                var currentLeaders = CliqueLeaders.ToList();
                List<Pawn>[] cliques = new List<Pawn>[currentLeaders.Count()];
                foreach (Pawn leader in currentLeaders)
                {
                    List<Pawn> members = (from c in PawnsFinder.AllMaps_FreeColonistsSpawned
                                          where c == leader || (c.Map == leader.Map && c.relations.OpinionOf(leader) > 20)
                                          select c).ToList();
                    cliques[leaderNum] = members;
                    Pawn cliqueLeaderWhoMadeUpSomehow = (from c in members
                                                         where currentLeaders.Contains(c)
                                                         select c).FirstOrDefault();
                    if (cliqueLeaderWhoMadeUpSomehow != null)
                    {
                        //Kiss and make up.
                        leader.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOfIncidents.Clique, cliqueLeaderWhoMadeUpSomehow);
                        cliqueLeaderWhoMadeUpSomehow.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOfIncidents.Clique, leader);
                    }
                    leaderNum++;
                }
                cachedCliques = cliques;
            }
            StringBuilder cliqueList = new StringBuilder();
            foreach(List<Pawn> clique in cachedCliques)
            {
                leaderNum = 0;
                foreach (Pawn member in clique)
                {
                    cliqueList.AppendLine(leaderNum == 0 ? "Leader: " + member.NameShortColored : member.NameShortColored);
                    leaderNum++;
                }
                cliqueList.AppendLine();
            }
            return "CliqueMembersDesc".Translate(cliqueList);
        }

        public override AlertReport GetReport()
        {
            if(CliqueLeaders.Count() < 2)
            {
                return AlertReport.Inactive;
            }
            return new AlertReport
            {
                active = true,
                culpritsPawns = CliqueLeaders.ToList()
            };
        }

        private IEnumerable<Pawn> CliqueLeaders
        {
            get
            {
                return (from c in PawnsFinder.AllMaps_FreeColonistsSpawned
                        where c.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Clique) > 0
                        select c);
            }
        }

        private IEnumerable<Pawn>[] cachedCliques;
    }
}
