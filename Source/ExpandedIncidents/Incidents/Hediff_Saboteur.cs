using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents;

public class Hediff_Saboteur : HediffWithComps
{
    public static Building_Turret FindTurretFor(Pawn p)
    {
        bool TurretValidator(Thing t)
        {
            var building_Turret3 = (Building_TurretGun)t;
            if (!p.CanReserveAndReach(t, PathEndMode.OnCell, Danger.Some))
            {
                return false;
            }

            return building_Turret3.GetComp<CompFlickable>().SwitchIsOn && !building_Turret3.IsBurning();
        }

        var thingDef = ThingDefOf.Turret_MiniTurret;

        bool Validator(Thing b)
        {
            return TurretValidator(b);
        }

        var building_Turret2 = (Building_Turret)GenClosest.ClosestThingReachable(p.Position, p.Map,
            ThingRequest.ForDef(thingDef), PathEndMode.OnCell, TraverseParms.For(p), 9999f, Validator);
        return building_Turret2;
    }

    public static Building FindBreakDownTargetFor(Pawn p)
    {
        bool BreakdownValidator(Thing t)
        {
            var building3 = (Building)t;
            if (!p.CanReserveAndReach(t, PathEndMode.OnCell, Danger.Some))
            {
                return false;
            }

            return !building3.GetComp<CompBreakdownable>().BrokenDown && !building3.IsBurning();
        }

        var thingDef = (from t in DefDatabase<ThingDef>.AllDefsListForReading
            where t.GetCompProperties<CompProperties_Breakdownable>() != null
            select t).ToList().RandomElement();

        bool Validator(Thing b)
        {
            return BreakdownValidator(b);
        }

        var building = (Building)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(thingDef),
            PathEndMode.OnCell, TraverseParms.For(p), 9999f, Validator);
        return building;
    }

    public override void Tick()
    {
        if (pawn.pather?.nextCell.GetDoor(pawn.Map) != null)
        {
            var d = pawn.pather.nextCell.GetDoor(pawn.Map);
            if (pawn.health.hediffSet.HasHediff(HediffDefOfIncidents.Saboteur) & !d.HoldOpen && !pawn.Drafted &&
                Rand.Value < 0.05f)
            {
                Traverse.Create(d).Field("holdOpenInt").SetValue(true);
            }
        }

        if (!pawn.IsHashIntervalTick(5000) || !(Rand.Value < 0.25f))
        {
            return;
        }

        switch (Rand.RangeInclusive(0, 10))
        {
            case 0:
                var breakdownAble = FindBreakDownTargetFor(pawn);
                if (breakdownAble != null)
                {
                    pawn.jobs.StartJob(new Job(JobDefOfIncidents.Sabotage, breakdownAble),
                        JobCondition.InterruptOptional, null, false, false);
                }

                break;
            default:
                var turret = FindTurretFor(pawn);
                if (turret != null)
                {
                    Traverse.Create(turret.GetComp<CompFlickable>()).Field("wantSwitchOn").SetValue(false);
                    turret.Map.designationManager.AddDesignation(
                        new Designation(turret, DesignationDefOf.Flick));
                    pawn.jobs.StartJob(new Job(JobDefOf.Flick, turret), JobCondition.InterruptOptional, null,
                        false, false);
                }

                break;
        }
    }
}