using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace AutoEquip
{
    public class MapComponent_AutoEquip : MapComponent
    {
        public int nextOptimization;
        public List<Saveable_Outfit> outfitCache = new List<Saveable_Outfit>();
        public List<Saveable_Pawn> pawnCache = new List<Saveable_Pawn>();

        public static IEnumerable<KeyValuePair<StatDef, float>> GetStatsOfWorkType(WorkTypeDef worktype)
        {
            yield return new KeyValuePair<StatDef, float>(StatDefOf.ArmorRating_Blunt, 0.00075f);
            yield return new KeyValuePair<StatDef, float>(StatDefOf.ArmorRating_Sharp, 0.001f);

            switch (worktype.defName)
            {
                case "Research":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("ResearchSpeed"), 1.0f);
                    yield break;
                case "Cleaning":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MoveSpeed"), 0.5f);
                    yield break;
                case "Hauling":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MoveSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("CarryingCapacity"), 1.0f);
                    yield break;
                case "Crafting":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"), 0.3f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("StonecuttingSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("SmeltingSpeed"), 1.0f);
                    yield break;
                case "Art":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("SculptingSpeed"), 1.0f);
                    yield break;
                case "Tailoring":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"), 0.9f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("TailoringSpeed"), 1.0f);
                    yield break;
                case "Smithing":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"), 0.9f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("SmithingSpeed"), 1.0f);
                    yield break;
                case "PlantCutting":
                case "Growing":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"), 0.1f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MoveSpeed"), 0.3f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("PlantWorkSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("HarvestFailChance"), -1.0f);
                    yield break;
                case "Mining":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"), 0.1f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MoveSpeed"), 0.2f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MiningSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("CarryingCapacity"), 0.3f);
                    yield break;
                case "Repair":
                case "Construction":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"), 0.1f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MoveSpeed"), 0.2f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("ConstructionSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("SmoothingSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("CarryingCapacity"), 0.9f);
                    yield break;
                case "Hunting":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MoveSpeed"), 0.2f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("AimingDelayFactor"), 0.5f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("ShootingAccuracy"), 0.5f);
                    yield return new KeyValuePair<StatDef, float>(StatDefOf.ArmorRating_Blunt, 0.0015f);
                    yield return new KeyValuePair<StatDef, float>(StatDefOf.ArmorRating_Sharp, 0.002f);
                    yield break;
                case "Cooking":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MoveSpeed"), 0.05f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("WorkSpeedGlobal"), 0.2f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("CookSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("FoodPoisonChance"), -1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("BrewingSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("ButcheryFleshSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("ButcheryFleshEfficiency"), 1.0f);
                    yield break;
                case "Handling":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MoveSpeed"), 0.2f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("CarryingCapacity"), 0.5f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("TameAnimalChance"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("TrainAnimalChance"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MeleeDPS"), 0.2f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MeleeHitChance"), 0.2f);
                    yield return new KeyValuePair<StatDef, float>(StatDefOf.ArmorRating_Blunt, 0.0015f);
                    yield return new KeyValuePair<StatDef, float>(StatDefOf.ArmorRating_Sharp, 0.002f);
                    yield break;
                case "Warden":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("RecruitPrisonerChance"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("GiftImpact"), 0.1f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("TradePriceImprovement"), 0.8f);                    
                    yield break;
                case "Flicker":
                case "Patient":
                case "Firefighter":
                    yield break;
                case "Doctor":
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("MedicalOperationSpeed"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("SurgerySuccessChance"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("BaseHealingQuality"), 1.0f);
                    yield return new KeyValuePair<StatDef, float>(DefDatabase<StatDef>.GetNamed("HealingSpeed"), 1.0f);
                    yield break;
                default:
                    if (!ignowingWorktypeDef.Contains(worktype.defName))
                    {
                        Log.Warning("WorkTypeDef " + worktype.defName + " not handled.");
                        ignowingWorktypeDef.Add(worktype.defName);
                    }
                    yield break;
            }
        }

        static List<string> ignowingWorktypeDef = new List<string>();

#if LOG
        public static StringBuilder logMessage; 
#endif

        public static MapComponent_AutoEquip Get
        {
            get
            {
                MapComponent_AutoEquip getComponent = Find.Map.components.OfType<MapComponent_AutoEquip>().FirstOrDefault();
                if (getComponent == null)
                {
                    getComponent = new MapComponent_AutoEquip();
                    Find.Map.components.Add(getComponent);
                }

                return getComponent;
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.LookList(ref this.outfitCache, "outfits", LookMode.Deep);
            Scribe_Collections.LookList(ref this.pawnCache, "pawns", LookMode.Deep);
            Scribe_Values.LookValue(ref this.nextOptimization, "nextOptimization", 0);
            base.ExposeData();

            if (this.outfitCache == null)
                this.outfitCache = new List<Saveable_Outfit>();

            if (this.pawnCache == null)
                this.pawnCache = new List<Saveable_Pawn>();
        }

        public Saveable_Outfit GetOutfit(Pawn pawn) { return this.GetOutfit(pawn.outfits.CurrentOutfit); }

        public Saveable_Outfit GetOutfit(Outfit outfit)
        {
            foreach (Saveable_Outfit o in this.outfitCache)
                if (o.outfit == outfit)
                    return o;

            Saveable_Outfit ret = new Saveable_Outfit();
            ret.outfit = outfit;
            ret.stats.Add(new Saveable_StatDef() { statDef = StatDefOf.ArmorRating_Sharp, strength = 1.00f });
            ret.stats.Add(new Saveable_StatDef() { statDef = StatDefOf.ArmorRating_Blunt, strength = 0.75f });

            this.outfitCache.Add(ret);

            return ret;
        }

        public Saveable_Pawn GetCache(Pawn pawn)
        {
            foreach (Saveable_Pawn c in this.pawnCache)
                if (c.pawn == pawn)
                    return c;            
            Saveable_Pawn n = new Saveable_Pawn();
            n.pawn = pawn;
            this.pawnCache.Add(n);
            return n;
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (Find.TickManager.TicksGame < this.nextOptimization)
                return;

#if LOG
            MapComponent_AutoEquip.logMessage = new StringBuilder();
            MapComponent_AutoEquip.logMessage.AppendLine("Start Scaning Best Apparel");
            MapComponent_AutoEquip.logMessage.AppendLine();
#endif
            List<Saveable_Pawn> newSaveableList = new List<Saveable_Pawn>();
            List<PawnCalcForApparel> newCalcList = new List<PawnCalcForApparel>();

            List<Apparel> allApparels = new List<Apparel>(Find.ListerThings.ThingsInGroup(ThingRequestGroup.Apparel).OfType<Apparel>());            
            foreach (Pawn pawn in Find.ListerPawns.FreeColonists)
            {
                this.InjectTab(pawn.def);
                Saveable_Pawn newPawnSaveable = this.GetCache(pawn);
                PawnCalcForApparel newPawnCalc = new PawnCalcForApparel(newPawnSaveable);

                newSaveableList.Add(newPawnSaveable);
                newCalcList.Add(newPawnCalc);

                newPawnCalc.InitializeFixedApparelsAndGetAvaliableApparels(allApparels);
            }

            this.pawnCache = newSaveableList;
            PawnCalcForApparel.DoOptimizeApparel(newCalcList, allApparels);

#if LOG
            this.nextOptimization = Find.TickManager.TicksGame + 500;
#else
            this.nextOptimization = Find.TickManager.TicksGame + 5000;
#endif
        }        

        private void InjectTab(ThingDef thingDef)
        {
            Debug.Log("Inject Tab");
            if (thingDef.inspectorTabsResolved == null)
            {
                thingDef.inspectorTabsResolved = new List<ITab>();
                foreach (Type current in thingDef.inspectorTabs)
                    thingDef.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(current));
            }

            if (!thingDef.inspectorTabsResolved.OfType<ITab_Pawn_AutoEquip>().Any())
            {
                thingDef.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(typeof(ITab_Pawn_AutoEquip)));
                Debug.Log("Add Tab");
            }

            for (int i = thingDef.inspectorTabsResolved.Count - 1; i >= 0; i--)
                if (thingDef.inspectorTabsResolved[i].GetType() == typeof(ITab_Pawn_Gear))
                    thingDef.inspectorTabsResolved.RemoveAt(i);

            for (int i = thingDef.inspectorTabs.Count - 1; i >= 0; i--)
                if (thingDef.inspectorTabs[i] == typeof(ITab_Pawn_Gear))
                    thingDef.inspectorTabs.RemoveAt(i);
        }        
    }
}
