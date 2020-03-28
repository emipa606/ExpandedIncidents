using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace ExpandedIncidents.Harmony
{
    [HarmonyPatch(typeof(InteractionWorker), "Interacted")]
    public static class InteractionWorkerHomesicknessPatch
    {
        [HarmonyPrefix]
        public static bool SpreadHomesickness(InteractionWorker __instance, Pawn initiator, Pawn recipient)
        {
            if (__instance.GetType() == typeof(InteractionWorker_DeepTalk) || (__instance.GetType().Name == "InteractionWorker_Conversation" && recipient.relations.OpinionOf(initiator) > 20))
            {
                try
                {
                    ((Action)(() =>
                    {
                        if (initiator.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Homesickness) > 0 && recipient.GetComp<Psychology.CompPsychology>() != null && recipient.GetComp<Psychology.CompPsychology>().isPsychologyPawn)
                        {
                            if (Rand.Value < (0.1f + initiator.GetStatValue(StatDefOf.SocialImpact) * (0.25f + recipient.GetComp<Psychology.CompPsychology>().Psyche.GetPersonalityRating(DefDatabase<Psychology.PersonalityNodeDef>.GetNamed("Nostalgic")))))
                            {
                                recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Homesickness);
                            }
                        }
                    }))();
                }
                catch (TypeLoadException)
                {
                    if (initiator.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Homesickness) > 0 && Rand.Value < (0.1f + initiator.GetStatValue(StatDefOf.SocialImpact)))
                    {
                        recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Homesickness);
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(InteractionWorker), "Interacted")]
    public static class InteractionWorkerCliquePatch
    {
        [HarmonyPrefix]
        public static bool ManageCliques(InteractionWorker __instance, Pawn initiator, Pawn recipient)
        {
            bool clique;
            if (initiator.RaceProps.Humanlike && recipient.RaceProps.Humanlike)
            {
                IEnumerable<Pawn> cliqueLeadersPresent = (from c in initiator.Map.mapPawns.AllPawnsSpawned
                                                          where c.IsColonist && c.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Clique) > 0
                                                          select c);
                bool checkInitiator = AnyCliqueLinks(initiator, recipient, cliqueLeadersPresent, out clique);
                if (clique && initiator.RaceProps.Humanlike && recipient.RaceProps.Humanlike)
                {
                    if (Rand.Value <= 0.05f && lastFightTick <= Find.TickManager.TicksGame - 5000)
                    {
                        if (!checkInitiator)
                        {
                            return StartSocialFight(recipient, initiator, cliqueLeadersPresent);
                        }
                        else
                        {
                            return StartSocialFight(initiator, recipient, cliqueLeadersPresent);
                        }
                    }
                }
            }
            return true;
        }

        private static bool StartSocialFight(Pawn initiator, Pawn recipient, IEnumerable<Pawn> CliqueLeadersPresent)
        {
            if(Rand.Value <= Mathf.InverseLerp(50f, -100f, initiator.relations.OpinionOf(recipient)))
            {
                if (!(CliqueLeadersPresent.Contains(initiator) && CliqueLeadersPresent.Contains(recipient)))
                {
                    recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.CliqueFollower, initiator);
                    initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.CliqueFollower, recipient);
                }
                initiator.interactions.StartSocialFight(recipient);
                lastFightTick = Find.TickManager.TicksGame;
                return false;
            }
            return true;
        }

        private static bool AnyCliqueLinks(Pawn initiator, Pawn recipient, IEnumerable<Pawn> CliqueLeadersPresent, out bool checkInitiator)
        {
            checkInitiator = false;
            //No cliques, no cliquer
            if (!CliqueLeadersPresent.Any() || CliqueLeadersPresent.Count() < 2)
            {
                return false;
            }
            //First, check for existing clique follower thoughts so we don't have to go through pawns.
            bool foundCliqueFollowerThoughtRecipient = (from t in recipient.needs.mood.thoughts.memories.Memories
                                                        where t.def == ThoughtDefOfIncidents.CliqueFollower && t.otherPawn == initiator
                                                        select t).FirstOrDefault() != null;
            if (foundCliqueFollowerThoughtRecipient)
            {
                return true; //Clique member starts fight with known clique leader, initiator
            }
            bool foundCliqueFollowerThoughtInitiator = (from t in initiator.needs.mood.thoughts.memories.Memories
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
            Pawn nonLeader;
            if (CliqueLeadersPresent.Contains(initiator))
            {
                //Initiator is clique leader
                leadersNonLeaderLikes = (from p in CliqueLeadersPresent
                                         where recipient.relations.OpinionOf(p) > 20
                                         orderby recipient.relations.OpinionOf(p) descending
                                         select p);
                leader = initiator;
                nonLeader = recipient;
            }
            else if(CliqueLeadersPresent.Contains(recipient))
            {
                //Recipient is clique leader
                leadersNonLeaderLikes = (from p in CliqueLeadersPresent
                                         where initiator.relations.OpinionOf(p) > 20
                                         orderby initiator.relations.OpinionOf(p) descending
                                         select p);
                leader = initiator;
                nonLeader = recipient;
            }
            else
            {
                //Neither are clique leaders, but may still both be in different cliques
                IEnumerable<Pawn> leadersRecipientLikes = (from p in CliqueLeadersPresent
                                         where recipient.relations.OpinionOf(p) > 20
                                         orderby recipient.relations.OpinionOf(p) descending
                                         select p);
                IEnumerable<Pawn> leadersInitiatorLikes = (from p in CliqueLeadersPresent
                                         where initiator.relations.OpinionOf(p) > 20
                                         orderby initiator.relations.OpinionOf(p) descending
                                         select p);
                if (leadersRecipientLikes.Count() == 0 || leadersInitiatorLikes.Count() == 0 || leadersInitiatorLikes.First() == leadersRecipientLikes.First())
                {
                    //Either one of them is not in any cliques, or they are both in the same clique and both prefer the same leader
                    return false;
                }
                //They are both in cliques and do not prefer the same leader.
                return true; //Recipient starts fight with initiator for preferring another clique leader
            }
            //One or the other is a clique leader
            if (leadersNonLeaderLikes.Count() == 0)
            {
                //Non-leader is not friends with any leader, so they aren't a part of the clique.
                return false;
            }
            if (!leadersNonLeaderLikes.Contains(initiator))
            {
                checkInitiator = (leader == recipient);
                return true; //Non-leader starts fight with leader for being leader of an enemy clique
            }
            else if (leadersNonLeaderLikes.First() != leader)
            {
                checkInitiator = (leader == initiator);
                return true; //Leader starts a fight with the non-leader for preferring the other leader
            }
            return false; //Non-leader is friends with both leaders, but prefers the leader they're speaking to
        }

        private static int lastFightTick = -9999;
    }
}
