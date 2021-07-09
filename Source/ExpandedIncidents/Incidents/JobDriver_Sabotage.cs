using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;

namespace ExpandedIncidents
{
    public class JobDriver_Sabotage : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_Sabotage.DoSabotage(TargetIndex.A);
        }
    }
}