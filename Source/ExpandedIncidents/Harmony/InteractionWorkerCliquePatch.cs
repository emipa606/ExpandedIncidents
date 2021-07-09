using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExpandedIncidents.Harmony
{
    [HarmonyPatch(typeof(InteractionWorker), "Interacted")]
    public static class InteractionWorkerCliquePatch
    {
        private static int lastFightTick = -9999;

        [HarmonyPrefix]
        public static bool ManageCliques(InteractionWorker __instance, Pawn initiator, Pawn recipient)
        {
            if (!initiator.RaceProps.Humanlike || !recipient.RaceProps.Humanlike)
            {
                return true;
            }

            var cliqueLeadersPresent = from c in initiator.Map.mapPawns.AllPawnsSpawned
                where c.IsColonist &&
                      c.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Clique) > 0
                select c;
            var checkInitiator = AnyCliqueLinks(initiator, recipient, cliqueLeadersPresent, out var clique);
            if (!clique || !initiator.RaceProps.Humanlike || !recipient.RaceProps.Humanlike)
            {
                return true;
            }

            if (!(Rand.Value <= 0.05f) || lastFightTick > Find.TickManager.TicksGame - 5000)
            {
                return true;
            }

            if (!checkInitiator)
            {
                return StartSocialFight(recipient, initiator, cliqueLeadersPresent);
            }

            return StartSocialFight(initiator, recipient, cliqueLeadersPresent);
        }

        private static bool StartSocialFight(Pawn initiator, Pawn recipient, IEnumerable<Pawn> CliqueLeadersPresent)
        {
            if (!(Rand.Value <= Mathf.InverseLerp(50f, -100f, initiator.relations.OpinionOf(recipient))))
            {
                return true;
            }

            if (!(CliqueLeadersPresent.Contains(initiator) && CliqueLeadersPresent.Contains(recipient)))
            {
                recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.CliqueFollower,
                    initiator);
                initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.CliqueFollower,
                    recipient);
            }

            initiator.interactions.StartSocialFight(recipient);
            lastFightTick = Find.TickManager.TicksGame;
            return false;
        }

        private static bool AnyCliqueLinks(Pawn initiator, Pawn recipient, IEnumerable<Pawn> CliqueLeadersPresent,
            out bool checkInitiator)
        {
            checkInitiator = false;
            //No cliques, no cliquer
            if (!CliqueLeadersPresent.Any() || CliqueLeadersPresent.Count() < 2)
            {
                return false;
            }

            //First, check for existing clique follower thoughts so we don't have to go through pawns.
            var foundCliqueFollowerThoughtRecipient = (from t in recipient.needs.mood.thoughts.memories.Memories
                where t.def == ThoughtDefOfIncidents.CliqueFollower && t.otherPawn == initiator
                select t).FirstOrDefault() != null;
            if (foundCliqueFollowerThoughtRecipient)
            {
                return true; //Clique member starts fight with known clique leader, initiator
            }

            var foundCliqueFollowerThoughtInitiator = (from t in initiator.needs.mood.thoughts.memories.Memories
                where t.def == ThoughtDefOfIncidents.CliqueFollower && t.otherPawn == recipient
                select t).FirstOrDefault() != null;
            if (foundCliqueFollowerThoughtInitiator)
            {
                checkInitiator = true;
                return false; //Clique member starts fight with known clique leader, recipient
            }

            //Two clique leaders interact
            if (CliqueLeadersPresent.Contains(initiator) && CliqueLeadersPresent.Contains(recipient))
            {
                return true; //Recipient clique leader starts fight with initiator clique leader
            }

            IEnumerable<Pawn> leadersNonLeaderLikes;
            Pawn leader;
            if (CliqueLeadersPresent.Contains(initiator))
            {
                //Initiator is clique leader
                leadersNonLeaderLikes = from p in CliqueLeadersPresent
                    where recipient.relations.OpinionOf(p) > 20
                    orderby recipient.relations.OpinionOf(p) descending
                    select p;
                leader = initiator;
            }
            else if (CliqueLeadersPresent.Contains(recipient))
            {
                //Recipient is clique leader
                leadersNonLeaderLikes = from p in CliqueLeadersPresent
                    where initiator.relations.OpinionOf(p) > 20
                    orderby initiator.relations.OpinionOf(p) descending
                    select p;
                leader = initiator;
            }
            else
            {
                //Neither are clique leaders, but may still both be in different cliques
                IEnumerable<Pawn> leadersRecipientLikes = from p in CliqueLeadersPresent
                    where recipient.relations.OpinionOf(p) > 20
                    orderby recipient.relations.OpinionOf(p) descending
                    select p;
                IEnumerable<Pawn> leadersInitiatorLikes = from p in CliqueLeadersPresent
                    where initiator.relations.OpinionOf(p) > 20
                    orderby initiator.relations.OpinionOf(p) descending
                    select p;
                if (!leadersRecipientLikes.Any() || !leadersInitiatorLikes.Any() ||
                    leadersInitiatorLikes.First() == leadersRecipientLikes.First())
                {
                    //Either one of them is not in any cliques, or they are both in the same clique and both prefer the same leader
                    return false;
                }

                //They are both in cliques and do not prefer the same leader.
                return true; //Recipient starts fight with initiator for preferring another clique leader
            }

            //One or the other is a clique leader
            if (!leadersNonLeaderLikes.Any())
            {
                //Non-leader is not friends with any leader, so they aren't a part of the clique.
                return false;
            }

            if (!leadersNonLeaderLikes.Contains(initiator))
            {
                checkInitiator = leader == recipient;
                return true; //Non-leader starts fight with leader for being leader of an enemy clique
            }

            if (leadersNonLeaderLikes.First() == leader)
            {
                return false; //Non-leader is friends with both leaders, but prefers the leader they're speaking to
            }

            checkInitiator = leader == initiator;
            return true; //Leader starts a fight with the non-leader for preferring the other leader
        }
    }
}