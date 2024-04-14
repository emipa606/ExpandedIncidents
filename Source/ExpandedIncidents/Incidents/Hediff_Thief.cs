using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExpandedIncidents;

public class Hediff_Thief : HediffWithComps
{
    private FieldInfo _graphicInt;
    private FieldInfo _lastCell;
    private FieldInfo _shadowGraphic;

    private Thing lastCarried;

    //private Graphic lastCarriedGraphic;
    private int lastSpottedTick = -9999;

    //private PawnGraphicSet oldGraphics;
    //private Graphic_Shadow oldShadow;

    private void SetShadowGraphic(PawnRenderer _this, Graphic_Shadow newValue)
    {
        if (_shadowGraphic == null)
        {
            _shadowGraphic =
                typeof(PawnRenderer).GetField("shadowGraphic", BindingFlags.Instance | BindingFlags.NonPublic);
            if (_shadowGraphic == null)
            {
                Log.ErrorOnce("Unable to reflect PawnRenderer.shadowGraphic!", 0x12348765);
            }
        }

        _shadowGraphic?.SetValue(_this, newValue);
    }

    private Graphic_Shadow GetShadowGraphic(PawnRenderer _this)
    {
        if (_shadowGraphic != null)
        {
            return (Graphic_Shadow)_shadowGraphic.GetValue(_this);
        }

        _shadowGraphic =
            typeof(PawnRenderer).GetField("shadowGraphic", BindingFlags.Instance | BindingFlags.NonPublic);
        if (_shadowGraphic == null)
        {
            Log.ErrorOnce("Unable to reflect PawnRenderer.shadowGraphic!", 0x12348765);
        }

        return (Graphic_Shadow)_shadowGraphic?.GetValue(_this);
    }

    private void SetGraphicInt(Thing _this, Graphic newValue)
    {
        if (_graphicInt == null)
        {
            _graphicInt = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic);
            if (_graphicInt == null)
            {
                Log.ErrorOnce("Unable to reflect Thing.graphicInt!", 0x12348765);
            }
        }

        _graphicInt?.SetValue(_this, newValue);
    }

    private IntVec3 GetLastCell(Pawn_PathFollower _this)
    {
        if (_lastCell != null)
        {
            return (IntVec3)_lastCell.GetValue(_this);
        }

        _lastCell = typeof(Pawn_PathFollower).GetField("lastCell",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (_lastCell == null)
        {
            Log.ErrorOnce("Unable to reflect Pawn_PathFollower.lastCell!", 0x12348765);
        }

        if (_lastCell is not null)
        {
            return (IntVec3)_lastCell?.GetValue(_this)!;
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
        //oldGraphics = pawn.Drawer.renderer.graphics;
        //oldShadow = GetShadowGraphic(pawn.Drawer.renderer);
        //pawn.Drawer.renderer.graphics = new PawnGraphicSet_Invisible(pawn);
        //var shadowData = new ShadowData { volume = new Vector3(0, 0, 0), offset = new Vector3(0, 0, 0) };
        //SetShadowGraphic(pawn.Drawer.renderer, new Graphic_Shadow(shadowData));

        pawn.stances.CancelBusyStanceHard();
        //if (lastCarried == null || lastCarried != pawn.carryTracker.CarriedThing)
        //{
        //    return;
        //}

        //lastCarriedGraphic = pawn.carryTracker.CarriedThing.Graphic;
        //SetGraphicInt(pawn.carryTracker.CarriedThing, new Graphic_Invisible());
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
            AlertThief(pawn, pawn.pather?.nextCell.GetFirstPawn(pawn.Map));
        }

        if (pawn.pather != null && GetLastCell(pawn.pather).GetDoor(pawn.Map) != null)
        {
            GetLastCell(pawn.pather).GetDoor(pawn.Map).StartManualCloseBy(pawn);
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
                            AlertThief(pawn, observer);
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
                            AlertThief(pawn, turret);
                        }
                    }
                }
            }

            num++;
        }

        //var holding = pawn.carryTracker.CarriedThing;
        //if (lastCarried == holding)
        //{
        //    return;
        //}

        //if (lastCarried != null)
        //{
        //    SetGraphicInt(lastCarried, lastCarriedGraphic);
        //}

        //if (holding == null)
        //{
        //    return;
        //}

        //lastCarried = holding;
        //lastCarriedGraphic = holding.Graphic;
        ////SetGraphicInt(lastCarried, new Graphic_Invisible());
    }

    public override void PostRemoved()
    {
        //pawn.Drawer.renderer.graphics = oldGraphics;
        //pawn.Drawer.renderer.graphics.ResolveAllGraphics();
        //SetShadowGraphic(pawn.Drawer.renderer, oldShadow);
        //

        var holding = pawn.carryTracker.CarriedThing;
        //if (holding != null)
        //{
        //    SetGraphicInt(holding, lastCarriedGraphic);
        //}
        //else if (lastCarried != null)
        //{
        //    SetGraphicInt(lastCarried, lastCarriedGraphic);
        //}

        if (!pawn.Spawned && (holding != null || lastCarried != null))
        {
            Messages.Message(
                $"A thief has stolen {(holding != null ? holding.LabelNoCount : lastCarried.LabelNoCount)}!",
                MessageTypeDefOf.ThreatSmall);
        }
    }

    public void AlertThief(Pawn localPawn, Thing observer)
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