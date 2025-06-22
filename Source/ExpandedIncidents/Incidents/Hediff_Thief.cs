using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents;

public class Hediff_Thief : HediffWithComps
{
    private Thing lastCarried;
    private FieldInfo lastCell;

    private int lastSpottedTick = -9999;

    private IntVec3 getLastCell(Pawn_PathFollower _this)
    {
        if (lastCell != null)
        {
            return (IntVec3)lastCell.GetValue(_this);
        }

        lastCell = typeof(Pawn_PathFollower).GetField("lastCell",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (lastCell == null)
        {
            Log.ErrorOnce("Unable to reflect Pawn_PathFollower.lastCell!", 0x12348765);
        }

        if (lastCell is not null)
        {
            return (IntVec3)lastCell?.GetValue(_this)!;
        }

        return default;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref lastSpottedTick, "lastSpottedtick", -9999);
        Scribe_References.Look(ref lastCarried, "lastCarried");
    }

    public override void PostAdd(DamageInfo? dinfo)
    {
        base.PostAdd(dinfo);

        pawn.stances.CancelBusyStanceHard();
    }

    public override void Tick()
    {
        if (!pawn.Spawned)
        {
            pawn.health.RemoveHediff(this);
        }

        if (pawn.Downed || pawn.Dead || pawn.pather is { WillCollideNextCell: true })
        {
            pawn.health.RemoveHediff(this);
            alertThief(pawn, pawn.pather?.nextCell.GetFirstPawn(pawn.Map));
        }

        if (pawn.pather != null && getLastCell(pawn.pather).GetDoor(pawn.Map) != null)
        {
            getLastCell(pawn.pather).GetDoor(pawn.Map).StartManualCloseBy(pawn);
        }

        if (pawn.Map == null || lastSpottedTick >= Find.TickManager.TicksGame - 125)
        {
            return;
        }

        lastSpottedTick = Find.TickManager.TicksGame;
        var num = 0;
        while (num < 20)
        {
            var c = pawn.Position + GenRadial.RadialPattern[num];
            var room = RegionAndRoomQuery.RoomAt(c, pawn.Map);
            if (c.InBounds(pawn.Map))
            {
                if (RegionAndRoomQuery.RoomAt(c, pawn.Map) == room)
                {
                    var thingList = c.GetThingList(pawn.Map);
                    foreach (var thing in thingList)
                    {
                        var observer = thing as Pawn;
                        if (observer != null && observer != pawn && observer.Faction != null &&
                            (observer.Faction.IsPlayer || observer.Faction.HostileTo(pawn.Faction)))
                        {
                            var observerSight = observer.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
                            observerSight *= 0.805f + (pawn.Map.glowGrid.GroundGlowAt(pawn.Position) / 4);
                            if (observer.RaceProps.Animal)
                            {
                                observerSight *= 0.9f;
                            }

                            observerSight = Math.Min(2f, observerSight);
                            var thiefMoving = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving);
                            var spotChance = 0.8f * thiefMoving / observerSight;
                            if (!(Rand.Value > spotChance))
                            {
                                continue;
                            }

                            pawn.health.RemoveHediff(this);
                            alertThief(pawn, observer);
                        }
                        else if (observer == null)
                        {
                            if (thing is not Building_Turret { Faction.IsPlayer: true } turret)
                            {
                                continue;
                            }

                            var thiefMoving = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving);
                            var spotChance = 0.99f * thiefMoving;
                            if (!(Rand.Value > spotChance))
                            {
                                continue;
                            }

                            pawn.health.RemoveHediff(this);
                            alertThief(pawn, turret);
                        }
                    }
                }
            }

            num++;
        }
    }

    public override void PostRemoved()
    {
        var holding = pawn.carryTracker.CarriedThing;

        if (!pawn.Spawned && (holding != null || lastCarried != null))
        {
            Messages.Message(
                $"A thief has stolen {(holding != null ? holding.LabelNoCount : lastCarried.LabelNoCount)}!",
                MessageTypeDefOf.ThreatSmall);
        }
    }

    private static void alertThief(Pawn localPawn, Thing observer)
    {
        localPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
        if (!localPawn.Dead)
        {
            var thisPawn = new List<Pawn> { localPawn };
            var parms = new IncidentParms
            {
                faction = localPawn.Faction,
                spawnCenter = localPawn.Position,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack
            };
            try
            {
                parms.raidStrategy.Worker.MakeLords(parms, thisPawn);
            }
            catch
            {
                // ignored
            }

            localPawn.Map.avoidGrid.Regenerate();
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
        }

        if (observer != null)
        {
            Find.LetterStack.ReceiveLetter("LetterLabelThief".Translate(),
                "ThiefRevealed".Translate(observer.LabelShort, localPawn.Faction.Name, localPawn.Named("PAWN")),
                LetterDefOf.ThreatSmall, localPawn);
        }
        else
        {
            Find.LetterStack.ReceiveLetter("LetterLabelThief".Translate(),
                "ThiefInjured".Translate(localPawn.Faction.Name, localPawn.Named("PAWN")),
                LetterDefOf.NegativeEvent, localPawn);
        }
    }
}