using System;
using HarmonyLib;
using Psychology;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Harmony;

[HarmonyPatch(typeof(InteractionWorker), "Interacted")]
public static class InteractionWorkerHomesicknessPatch
{
    [HarmonyPrefix]
    public static bool SpreadHomesickness(InteractionWorker __instance, Pawn initiator, Pawn recipient)
    {
        if (__instance.GetType() != typeof(InteractionWorker_DeepTalk) &&
            (__instance.GetType().Name != "InteractionWorker_Conversation" ||
             recipient.relations.OpinionOf(initiator) <= 20))
        {
            return true;
        }

        try
        {
            ((Action)(() =>
            {
                if (initiator.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents
                        .Homesickness) <= 0 || recipient.GetComp<CompPsychology>() == null ||
                    !recipient.GetComp<CompPsychology>().IsPsychologyPawn)
                {
                    return;
                }

                if (Rand.Value < 0.1f + (initiator.GetStatValue(StatDefOf.SocialImpact) *
                                         (0.25f + recipient.GetComp<CompPsychology>().Psyche
                                             .GetPersonalityRating(
                                                 DefDatabase<PersonalityNodeDef>.GetNamed("Nostalgic")))))
                {
                    recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents
                        .Homesickness);
                }
            }))();
        }
        catch (TypeLoadException)
        {
            if (initiator.needs.mood.thoughts.memories.NumMemoriesOfDef(ThoughtDefOfIncidents.Homesickness) >
                0 && Rand.Value < 0.1f + initiator.GetStatValue(StatDefOf.SocialImpact))
            {
                recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfIncidents.Homesickness);
            }
        }

        return true;
    }
}