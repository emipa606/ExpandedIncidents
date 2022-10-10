using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents;

public static class Toils_Sabotage
{
    public static Toil DoSabotage(TargetIndex ind)
    {
        var toil = new Toil { defaultCompleteMode = ToilCompleteMode.Instant };
        toil.FailOnDespawnedOrNull(ind);
        toil.AddFinishAction(delegate
        {
            var building = (Building)toil.actor.jobs.curJob.GetTarget(ind).Thing;
            if (!building.GetComp<CompBreakdownable>().BrokenDown)
            {
                building.GetComp<CompBreakdownable>().DoBreakdown();
            }
        });
        return toil;
    }
}