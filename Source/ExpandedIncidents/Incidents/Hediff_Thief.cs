﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using System.Reflection;
using UnityEngine;

namespace ExpandedIncidents
{
    // Token: 0x02000188 RID: 392
    public class Hediff_Thief : HediffWithComps
    {
        private FieldInfo _shadowGraphic;
        private FieldInfo _graphicInt;
        private FieldInfo _lastCell;

        private void SetShadowGraphic(PawnRenderer _this, Graphic_Shadow newValue)
        {
            if (_shadowGraphic == null)
            {
                _shadowGraphic = typeof(PawnRenderer).GetField("shadowGraphic", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_shadowGraphic == null)
                {
                    Log.ErrorOnce("Unable to reflect PawnRenderer.shadowGraphic!", 0x12348765);
                }
            }
            _shadowGraphic.SetValue(_this, newValue);
        }

        private Graphic_Shadow GetShadowGraphic(PawnRenderer _this)
        {
            if (_shadowGraphic == null)
            {
                _shadowGraphic = typeof(PawnRenderer).GetField("shadowGraphic", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_shadowGraphic == null)
                {
                    Log.ErrorOnce("Unable to reflect PawnRenderer.shadowGraphic!", 0x12348765);
                }
            }
            return (Graphic_Shadow)_shadowGraphic.GetValue(_this);
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
            _graphicInt.SetValue(_this, newValue);
        }

        private IntVec3 GetLastCell(Pawn_PathFollower _this)
        {
            if (_lastCell == null)
            {
                _lastCell = typeof(Pawn_PathFollower).GetField("lastCell", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_lastCell == null)
                {
                    Log.ErrorOnce("Unable to reflect Pawn_PathFollower.lastCell!", 0x12348765);
                }
            }
            return (IntVec3)_lastCell.GetValue(_this);
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
            oldGraphics = pawn.Drawer.renderer.graphics;
            oldShadow = GetShadowGraphic(pawn.Drawer.renderer);
            pawn.Drawer.renderer.graphics = new PawnGraphicSet_Invisible(pawn);
            ShadowData shadowData = new ShadowData();
            shadowData.volume = new Vector3(0, 0, 0);
            shadowData.offset = new Vector3(0, 0, 0);
            SetShadowGraphic(pawn.Drawer.renderer, new Graphic_Shadow(shadowData));
            pawn.stances.CancelBusyStanceHard();
            if(lastCarried != null && lastCarried == pawn.carryTracker.CarriedThing)
            {
                lastCarriedGraphic = pawn.carryTracker.CarriedThing.Graphic;
                SetGraphicInt(pawn.carryTracker.CarriedThing, new Graphic_Invisible());
            }
        }

        public override void Tick()
        {
            if (!pawn.Spawned)
            {
                pawn.health.RemoveHediff(this);
            }
            if (pawn.Downed || pawn.Dead || (pawn.pather != null && pawn.pather.WillCollideWithPawnOnNextPathCell()))
            {
                pawn.health.RemoveHediff(this);
                if(pawn.pather != null)
                {
                    AlertThief(pawn, pawn.pather.nextCell.GetFirstPawn(pawn.Map));
                }
                else
                {
                    AlertThief(pawn, null);
                }
            }
            if (pawn.pather != null && GetLastCell(pawn.pather).GetDoor(pawn.Map) != null)
            {
                GetLastCell(pawn.pather).GetDoor(pawn.Map).StartManualCloseBy(pawn);
            }
            if (pawn.Map != null && lastSpottedTick < Find.TickManager.TicksGame - 125)
            {
                lastSpottedTick = Find.TickManager.TicksGame;
                int num = 0;
                while (num < 20)
                {
                    IntVec3 c = pawn.Position + GenRadial.RadialPattern[num];
                    Room room = RegionAndRoomQuery.RoomAt(c, pawn.Map);
                    if (c.InBounds(pawn.Map))
                    {
                        if (RegionAndRoomQuery.RoomAt(c, pawn.Map) == room)
                        {
                            List<Thing> thingList = c.GetThingList(pawn.Map);
                            foreach (Thing thing in thingList)
                            {
                                Pawn observer = thing as Pawn;
                                if (observer != null && observer != pawn && observer.Faction != null && (observer.Faction.IsPlayer || observer.Faction.HostileTo(pawn.Faction)))
                                {
                                    float observerSight = observer.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
                                    observerSight *= 0.805f + (pawn.Map.glowGrid.GameGlowAt(pawn.Position)/4);
                                    if(observer.RaceProps.Animal)
                                    {
                                        observerSight *= 0.9f;
                                    }
                                    observerSight = Math.Min(2f, observerSight);
                                    float thiefMoving = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving);
                                    float spotChance = 0.8f * thiefMoving / observerSight;
                                    if (Rand.Value > spotChance)
                                    {
                                        pawn.health.RemoveHediff(this);
                                        AlertThief(pawn, observer);
                                    }
                                }
                                else if(observer == null)
                                {
                                    Building_Turret turret = thing as Building_Turret;
                                    if(turret != null && turret.Faction != null && turret.Faction.IsPlayer)
                                    {
                                        float thiefMoving = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving);
                                        float spotChance = 0.99f * thiefMoving;
                                        if(Rand.Value > spotChance)
                                        {
                                            pawn.health.RemoveHediff(this);
                                            AlertThief(pawn, turret);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    num++;
                }
                Thing holding = pawn.carryTracker.CarriedThing;
                if (lastCarried != holding)
                {
                    if (lastCarried != null)
                    {
                        SetGraphicInt(lastCarried, lastCarriedGraphic);
                    }
                    if (holding != null)
                    {
                        lastCarried = holding;
                        lastCarriedGraphic = holding.Graphic;
                        SetGraphicInt(lastCarried, new Graphic_Invisible());
                    }
                }
            }
        }

        public override void PostRemoved()
        {
            pawn.Drawer.renderer.graphics = oldGraphics;
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            SetShadowGraphic(pawn.Drawer.renderer, oldShadow);
            Thing holding = pawn.carryTracker.CarriedThing;
            if (holding != null)
            {
                SetGraphicInt(holding, lastCarriedGraphic);
            }
            else if(lastCarried != null)
            {
                SetGraphicInt(lastCarried, lastCarriedGraphic);
            }
            if(!pawn.Spawned && (holding != null || lastCarried != null))
            {
                Messages.Message("A thief has stolen "+(holding != null ? holding.LabelNoCount : lastCarried.LabelNoCount)+"!", MessageTypeDefOf.ThreatSmall);
            }
        }

        public void AlertThief(Pawn pawn, Thing observer)
        {
            pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            if(!pawn.Dead)
            {
                List<Pawn> thisPawn = new List<Pawn>();
                thisPawn.Add(pawn);
                IncidentParms parms = new IncidentParms();
                parms.faction = pawn.Faction;
                parms.spawnCenter = pawn.Position;
                parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;try
                {
                    parms.raidStrategy.Worker.MakeLords(parms, thisPawn); 
                }
                catch (Exception)
                {
                }
                pawn.Map.avoidGrid.Regenerate();
                LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
            }
            if(observer != null)
            {
                Find.LetterStack.ReceiveLetter("LetterLabelThief".Translate(), "ThiefRevealed".Translate(observer.LabelShort, pawn.Faction.Name, pawn.Named("PAWN")), LetterDefOf.ThreatSmall, pawn, null);
            }
            else
            {
                Find.LetterStack.ReceiveLetter("LetterLabelThief".Translate(), "ThiefInjured".Translate(pawn.Faction.Name, pawn.Named("PAWN")), LetterDefOf.NegativeEvent, pawn, null);
            }
        }

        private PawnGraphicSet oldGraphics;
        private Graphic_Shadow oldShadow;
        private int lastSpottedTick = -9999;
        private Graphic lastCarriedGraphic;
        private Thing lastCarried;
    }
}
