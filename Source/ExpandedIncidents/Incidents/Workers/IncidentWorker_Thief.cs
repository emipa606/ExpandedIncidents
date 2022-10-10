using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ExpandedIncidents;

internal class IncidentWorker_Thief : IncidentWorker
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        var enemy = Find.FactionManager.RandomEnemyFaction();
        var thiefKind = DefDatabase<PawnKindDef>.GetNamed("Scavenger");
        switch (enemy.def.backstoryFilters.First().categories.First())
        {
            case "Tribal":
                thiefKind = DefDatabase<PawnKindDef>.GetNamed("Tribal_Warrior");
                break;
            case "Civil":
                thiefKind = DefDatabase<PawnKindDef>.GetNamed("Villager");
                break;
        }

        var thief = PawnGenerator.GeneratePawn(thiefKind, enemy);
        if (!RCellFinder.TryFindRandomPawnEntryCell(out var intVec, map, CellFinder.EdgeRoadChance_Hostile))
        {
            return false;
        }

        GenSpawn.Spawn(thief, intVec, map);
        var hediff = HediffMaker.MakeHediff(HediffDefOfIncidents.Thief, thief);
        thief.health.AddHediff(hediff);
        if (!RCellFinder.TryFindBestExitSpot(thief, out var c))
        {
            return false;
        }

        var valuables = (from t in map.listerThings.ThingsInGroup(ThingRequestGroup.HaulableAlways)
            where t.MarketValue * Mathf.Min(t.stackCount,
                      (int)(thief.GetStatValue(StatDefOf.CarryingCapacity) / t.def.VolumePerUnit)) >= 250 &&
                  HaulAIUtility.PawnCanAutomaticallyHaulFast(thief, t, false)
            select t).ToList();
        if (valuables.Count == 0)
        {
            thief.Destroy();
            return false;
        }

        var valuable = valuables.RandomElement();
        thief.jobs.StartJob(new Job(JobDefOf.Steal)
        {
            targetA = valuable,
            targetB = c,
            count = Mathf.Min(valuable.stackCount,
                (int)(thief.GetStatValue(StatDefOf.CarryingCapacity) / valuable.def.VolumePerUnit))
        });
        return true;
    }
}