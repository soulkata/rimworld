using AbilityPack;
using RimWorld;
using RimWorld.SquadAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace FactionGauntlet
{
    public class MapComponentFactionGauntlet : MapComponent
    {
        public static int roundLenght = 50000;
        public static MapComponentFactionGauntlet GetMapComponent()
        {
            MapComponentFactionGauntlet ret = Find.Map.components.OfType<MapComponentFactionGauntlet>().FirstOrDefault();
            if (ret == null)
            {
                ret = new MapComponentFactionGauntlet();
                Find.Map.components.Add(ret);
            }

            return ret;
        }

        public ExecutingStage stage;
        public ChallengeType? competitorTable;
        public FightStyle? style;
        public int? roundNumbers;
        public float? sandBagChance;
        public float? wallChance;
        public List<float> raidScales = new List<float>();

        public List<SaveableFight> fights = new List<SaveableFight>();
        public int remainingTicks = 0;

        public SaveableFight currentFight;
        public Brain currentBrain1;
        public Brain currentBrain2;
        public float currentPoints1;
        public float currentPoints2;

        public List<Faction> toRaid = new List<Faction>();
        
        public override void MapComponentOnGUI()
        {
            base.MapComponentOnGUI();

            if (!Prefs.DevMode)
                return;

            switch (this.stage)
            {
                case ExecutingStage.PickingParameters:
                    if (!this.competitorTable.HasValue)
                    {
                        if (!Find.LayerStack.HasLayerOfType(typeof(LayerChooserChallengeType)))
                            Find.LayerStack.Add(new LayerChooserChallengeType());                    
                        return;
                    }

                    if (!this.style.HasValue)
                    {
                        if (!Find.LayerStack.HasLayerOfType(typeof(LayerChooserFightStyle)))
                            Find.LayerStack.Add(new LayerChooserFightStyle());
                        return;
                    }

                    if (!this.raidScales.Any())
                    {
                        if (!Find.LayerStack.HasLayerOfType(typeof(LayerChooserRaidScale)))
                            Find.LayerStack.Add(new LayerChooserRaidScale());
                        return;
                    }

                    if ((this.toRaid.Count == 0) ||
                        ((this.competitorTable.Value == ChallengeType.AnotherFaction) &&
                         (this.toRaid.Count == 1)))
                    {
                        if (!Find.LayerStack.HasLayerOfType(typeof(LayerChooserFaction)))
                            Find.LayerStack.Add(new LayerChooserFaction());
                        return;
                    }

                    if (!this.sandBagChance.HasValue)
                    {
                        if (!Find.LayerStack.HasLayerOfType(typeof(LayerChooserArenaGround)))
                            Find.LayerStack.Add(new LayerChooserArenaGround());
                        return;
                    }

                    this.stage = ExecutingStage.CalculatingFights;
                    return;
                case ExecutingStage.Fighting:
                case ExecutingStage.DisplaingResults:
                    string text = string.Empty;

                    foreach (SaveableFight fight in this.fights)
                        text += fight.factionOne.name + "  " + fight.winners.Where(i => i == fight.factionOne).Count() + "      X      " + fight.winners.Where(i => i == fight.factionTwo).Count() + "  " + fight.factionTwo.name + "   -   Points: " + fight.points.ToString("N0") + Environment.NewLine;

                    if (this.currentFight != null)
                    {
                        text += "Current Fight";
                        if (this.style.Value == FightStyle.TimedRounds)
                            text += "  Remaining: " + this.remainingTicks.ToString("N0") + " ticks";
                        text += Environment.NewLine;
                        text += this.currentFight.factionOne.name + "  " + (this.currentPoints1 * 100 / (this.currentPoints1 + this.currentPoints2)).ToString("N2") + "% X " + (this.currentPoints2 * 100 / (this.currentPoints1 + this.currentPoints2)).ToString("N2") + "  " + this.currentFight.factionTwo.name;
                    }

                    GUI.Label(new Rect(0, 0, 500, 600), text);
                    return;
            }
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();        

            switch (this.stage)
            {
                case ExecutingStage.ClearParameters:
                    this.competitorTable = null;
                    this.style = null;
                    this.roundNumbers = null;
                    this.sandBagChance = null;
                    this.wallChance = null;
                    this.raidScales.Clear();
                    this.stage = ExecutingStage.PickingParameters;
                    break;
                case ExecutingStage.PickingParameters:
                    break;
                case ExecutingStage.CalculatingFights:
                    this.fights.Clear();
                    switch (this.competitorTable.Value)
                    {
                        case ChallengeType.AnotherFaction:
                            this.CalculateFights(this.toRaid[0], this.toRaid[1]);
                            break;
                        case ChallengeType.AllOtherFactions:
                            foreach (Faction faction in Find.FactionManager.AllFactions.Where(i => i.HostileTo(Faction.OfColony)))
                                this.CalculateFights(this.toRaid[0], faction);
                            break;
                        case ChallengeType.AllCoreFactions:
                            foreach (Faction faction in Find.FactionManager.AllFactions.Where(i => i.HostileTo(Faction.OfColony)))
                                if ((faction.def.defName == "Colony") ||
                                    (faction.def.defName == "Spacer") ||
                                    (faction.def.defName == "SpacerHostile") ||
                                    (faction.def.defName == "Outlander") ||
                                    (faction.def.defName == "Pirate") ||
                                    (faction.def.defName == "Tribe") ||
                                    (faction.def.defName == "Mechanoid"))
                                this.CalculateFights(this.toRaid[0], faction);
                            break;
                    }
                    this.stage = ExecutingStage.CreatingArena;
                    break;
                case ExecutingStage.CreatingArena:

                    if (this.fights.Where(i => i.winners.Count < this.roundNumbers.Value).Any())
                        this.currentFight = this.fights.Where(i => i.winners.Count < this.roundNumbers.Value).First();
                    else
                    {
                        this.stage = ExecutingStage.DisplaingResults;
                        return;
                    }

                    CellRect rect = CellRect.FromLimits(5, 5, 100, 60);

                    rect.ClipInsideMap();
                
                    foreach (IntVec3 current in rect)
                        Find.RoofGrid.SetRoof(current, null);

                    foreach (IntVec3 current in rect)
                    {
                        List<Thing> thingList = current.GetThingList();
                        for (int i = thingList.Count - 1; i >= 0; i--)
                        {
                            if (!Thing.allowDestroyNonDestroyable && !thingList[i].def.destroyable)
                                thingList[i].DeSpawn();
                            else
                                thingList[i].Destroy(DestroyMode.Vanish);
                        }

                        if (Rand.Range(0, 100) < this.sandBagChance.Value)
                            GenSpawn.Spawn(ThingDefOf.Sandbags, current);
                        else
                            if (Rand.Range(0, 100) < this.wallChance.Value)
                            {
                                ThingDef wall = ThingDefOf.Wall;
                                Thing newThing = ThingMaker.MakeThing(wall, ThingDefOf.Plasteel);
                                GenSpawn.Spawn(newThing, current);
                            }
                    }

                    foreach (IntVec3 current in rect)
                        Find.TerrainGrid.SetTerrain(current, TerrainDefOf.Concrete);

                    foreach (IntVec3 current in rect.EdgeCells)
                    {
                        ThingDef wall = ThingDefOf.Wall;
                        Thing newThing = ThingMaker.MakeThing(wall, ThingDefOf.Plasteel);
                        GenSpawn.Spawn(newThing, current);
                    }

                    AbilityRequeriment_ColonyBiggerThan.forcedValue = this.currentFight.points;

                    this.currentFight.factionOne.SetHostileTo(this.currentFight.factionTwo, true);
                    this.currentFight.factionOne.SetHostileTo(Faction.OfColony, true);
                    this.currentFight.factionTwo.SetHostileTo(this.currentFight.factionOne, true);
                    this.currentFight.factionTwo.SetHostileTo(Faction.OfColony, true);

                    List<Brain> oldBrains = new List<Brain>(Find.SquadBrianManager.allSquadBrains);
                
                    IncidentDef incidentDef = IncidentDef.Named("RaidEnemy");
                    IncidentParms incidentParms = Find.Storyteller.incidentMaker.ParmsNow(incidentDef.category);
                    incidentParms.faction = this.currentFight.factionOne;
                    incidentParms.spawnCenter = new IntVec3(30, 0, 35);
                    incidentParms.raidStyle = RaidStyle.ImmediateAttack;
                    incidentParms.points = this.currentFight.points;
                    incidentDef.Worker.TryExecute(incidentParms);

                    incidentDef = IncidentDef.Named("RaidEnemy");
                    incidentParms = Find.Storyteller.incidentMaker.ParmsNow(incidentDef.category);
                    incidentParms.faction = this.currentFight.factionTwo;
                    incidentParms.spawnCenter = new IntVec3(80, 0, 35);
                    incidentParms.raidStyle = RaidStyle.ImmediateAttack;
                    incidentParms.points = this.currentFight.points;
                    incidentDef.Worker.TryExecute(incidentParms);

                    List<Brain> newBrains = new List<Brain>(Find.SquadBrianManager.allSquadBrains);
                    foreach (Brain b in oldBrains)
                        newBrains.Remove(b);

                    this.currentBrain1 = this.GetBrain(this.currentFight.factionOne, newBrains);
                    this.currentBrain2 = this.GetBrain(this.currentFight.factionTwo, newBrains);
                    this.currentPoints1 = this.BrainPoints(this.currentBrain1);
                    this.currentPoints2 = this.BrainPoints(this.currentBrain2);

                    this.remainingTicks = MapComponentFactionGauntlet.roundLenght;
                    this.stage = ExecutingStage.Fighting;
                    break;
                case ExecutingStage.Fighting:

                    this.currentPoints1 = this.BrainPoints(this.currentBrain1);
                    this.currentPoints2 = this.BrainPoints(this.currentBrain2);

                    if ((this.currentPoints1 == 0.0f) ||
                        (this.currentPoints2 == 0.0f))
                    {
                        this.EndFight();
                        return;
                    }

                    if (this.style.Value == FightStyle.TimedRounds)
                    {
                        if (this.remainingTicks > 0)
                            this.remainingTicks--;
                        else
                        {
                            this.EndFight();
                            return;
                        }
                    }

                    break;
                case ExecutingStage.CleaningArena:
                    this.remainingTicks--;
                    if (this.remainingTicks <= 0)
                        this.stage = ExecutingStage.CreatingArena;
                    break;
            }
        }

        public Brain GetBrain(Faction faction, IEnumerable<Brain> brains)
        {            
            foreach (Brain b in brains)
            {
                if (b.faction != faction)
                    continue;
                if (!b.ownedPawns.Any())
                    continue;
                return b;
            }
            return null;
        }

        public IEnumerable<Pawn> BrainUnits(Brain brain) { return brain.ownedPawns.Where(i => (!i.Downed) && (!i.Dead)); }
        public float BrainPoints(Brain brain) { return this.BrainUnits(brain).Sum(i => i.kindDef.pointsCost); }

        private void CalculateFights(Faction factionOne, Faction factionTwo)
        {
            if (factionOne == factionTwo)
                return;

            foreach (float scale in this.raidScales)
                this.CalculateFights(factionOne, factionTwo, scale);
        }

        private void CalculateFights(Faction factionOne, Faction factionTwo, float scale)
        {
            if (scale < factionOne.def.MinPointsToGeneratePawnGroup())
            {
                Log.Warning("Faction " + factionOne.name + " cannot be create with " + scale + " points (Min: " + factionOne.def.MinPointsToGeneratePawnGroup().ToString("N0") + ".");
                return;
            }

            if (scale < factionTwo.def.MinPointsToGeneratePawnGroup())
            {
                Log.Warning("Faction " + factionTwo.name + " cannot be create with " + scale + " points (Min: " + factionTwo.def.MinPointsToGeneratePawnGroup().ToString("N0") + ".");
                return;
            }

            SaveableFight fight = new SaveableFight();
            fight.points = scale;
            fight.factionOne = factionOne;
            fight.factionTwo = factionTwo;
            this.fights.Add(fight);
        }

        private void EndFight()
        {            
            if (this.currentPoints1 == 0.0f)
                this.currentFight.winners.Add(this.currentFight.factionTwo);
            else
                if (this.currentPoints2 == 0.0f)
                    this.currentFight.winners.Add(this.currentFight.factionOne);
                else
                {
                    if (this.currentPoints1 > this.currentPoints2)
                        this.currentFight.winners.Add(this.currentFight.factionOne);
                    else
                        if (this.currentPoints2 > this.currentPoints1)
                            this.currentFight.winners.Add(this.currentFight.factionTwo);
                }
            this.stage = ExecutingStage.CleaningArena;
            this.remainingTicks = 100;
            this.currentFight = null;
            this.currentBrain1 = null;
            this.currentBrain2 = null;
            this.currentPoints1 = 0.0f;
            this.currentPoints2 = 0.0f;
        }
    }
}
