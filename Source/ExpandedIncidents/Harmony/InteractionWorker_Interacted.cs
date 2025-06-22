using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(InteractionWorker), nameof(InteractionWorker.Interacted))]
public static class InteractionWorker_Interacted
{
    private static int lastFightTick = -9999;

    public static bool Prefix(Pawn initiator, Pawn recipient)
    {
        if (initiator == null || recipient == null)
        {
            return true; //No interaction
        }

        if (!initiator.RaceProps.Humanlike || !recipient.RaceProps.Humanlike)
        {
            return true;
        }

        var cliqueLeadersPresent = from c in initiator.Map.mapPawns.AllPawnsSpawned
            where c.IsColonist &&
                  c.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Clique) > 0
            select c;
        var leadersPresent = cliqueLeadersPresent as Pawn[] ?? cliqueLeadersPresent.ToArray();
        var checkInitiator = anyCliqueLinks(initiator, recipient, leadersPresent, out var clique);
        if (!clique || !initiator.RaceProps.Humanlike || !recipient.RaceProps.Humanlike)
        {
            return true;
        }

        if (!(Rand.Value <= 0.05f) || lastFightTick > Find.TickManager.TicksGame - 5000)
        {
            return true;
        }

        return !checkInitiator
            ? startSocialFight(recipient, initiator, leadersPresent)
            : startSocialFight(initiator, recipient, leadersPresent);
    }

    private static bool startSocialFight(Pawn initiator, Pawn recipient, IEnumerable<Pawn> cliqueLeadersPresent)
    {
        if (!(Rand.Value <= Mathf.InverseLerp(50f, -100f, initiator.relations.OpinionOf(recipient))))
        {
            return true;
        }

        var leadersPresent = cliqueLeadersPresent as Pawn[] ?? cliqueLeadersPresent.ToArray();
        if (!(leadersPresent.Contains(initiator) && leadersPresent.Contains(recipient)))
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

    private static bool anyCliqueLinks(Pawn initiator, Pawn recipient, IEnumerable<Pawn> cliqueLeadersPresent,
        out bool checkInitiator)
    {
        checkInitiator = false;
        //No cliques, no cliquer
        var leadersPresent = cliqueLeadersPresent as Pawn[] ?? cliqueLeadersPresent.ToArray();
        if (!leadersPresent.Any() || leadersPresent.Count() < 2)
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
        if (leadersPresent.Contains(initiator) && leadersPresent.Contains(recipient))
        {
            return true; //Recipient clique leader starts fight with initiator clique leader
        }

        IEnumerable<Pawn> leadersNonLeaderLikes;
        Pawn leader;
        if (leadersPresent.Contains(initiator))
        {
            //Initiator is clique leader
            leadersNonLeaderLikes = from p in leadersPresent
                where recipient.relations.OpinionOf(p) > 20
                orderby recipient.relations.OpinionOf(p) descending
                select p;
            leader = initiator;
        }
        else if (leadersPresent.Contains(recipient))
        {
            //Recipient is clique leader
            leadersNonLeaderLikes = from p in leadersPresent
                where initiator.relations.OpinionOf(p) > 20
                orderby initiator.relations.OpinionOf(p) descending
                select p;
            leader = initiator;
        }
        else
        {
            //Neither are clique leaders, but may still both be in different cliques
            IEnumerable<Pawn> leadersRecipientLikes = from p in leadersPresent
                where recipient.relations.OpinionOf(p) > 20
                orderby recipient.relations.OpinionOf(p) descending
                select p;
            IEnumerable<Pawn> leadersInitiatorLikes = from p in leadersPresent
                where initiator.relations.OpinionOf(p) > 20
                orderby initiator.relations.OpinionOf(p) descending
                select p;
            var recipientLikes = leadersRecipientLikes as Pawn[] ?? leadersRecipientLikes.ToArray();
            return recipientLikes.Any() && leadersInitiatorLikes.Any() &&
                   leadersInitiatorLikes.First() != recipientLikes.First();
            //Either one of them is not in any cliques, or they are both in the same clique and both prefer the same leader
            //They are both in cliques and do not prefer the same leader.
            //Recipient starts fight with initiator for preferring another clique leader
        }

        //One or the other is a clique leader
        var nonLeaderLikes = leadersNonLeaderLikes as Pawn[] ?? leadersNonLeaderLikes.ToArray();
        if (!nonLeaderLikes.Any())
        {
            //Non-leader is not friends with any leader, so they aren't a part of the clique.
            return false;
        }

        if (!nonLeaderLikes.Contains(initiator))
        {
            checkInitiator = leader == recipient;
            return true; //Non-leader starts fight with leader for being leader of an enemy clique
        }

        if (nonLeaderLikes.First() == leader)
        {
            return false; //Non-leader is friends with both leaders, but prefers the leader they're speaking to
        }

        checkInitiator = leader == initiator;
        return true; //Leader starts a fight with the non-leader for preferring the other leader
    }
}