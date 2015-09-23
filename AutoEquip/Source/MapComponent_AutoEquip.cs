using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AutoEquip
{
    public class MapComponent_AutoEquip : MapComponent
    {
        public int nextOptimization;
        public List<Saveable_Outfit> outfitCache = new List<Saveable_Outfit>();
        public List<Saveable_PawnNextApparelConfiguration> pawnCache = new List<Saveable_PawnNextApparelConfiguration>();

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
                    Log.Warning("WorkTypeDef " + worktype.defName + " not handled.");
                    yield break;
            }

            //if (worktype == WorkTypeDefOf.Cleaning)
            //{
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.MoveSpeed, 0.1f);
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.DoorOpenSpeed, 0.1f);
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.Cleanliness, 1.0f);
            //    yield break;
            //}

            //if ((worktype == WorkTypeDefOf.Construction) ||
            //    (worktype == WorkTypeDefOf.Repair))
            //{
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.MoveSpeed, 0.1f);
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.DoorOpenSpeed, 0.1f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.ConstructionSpeed, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.CarryingCapacity, 0.75f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.SmoothingSpeed, 1.0f);
            //    yield break;
            //}

            //if (worktype == WorkTypeDefOf.Cooking)
            //{
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.DoorOpenSpeed, 0.1f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.FoodPoisonChance, -1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.WorkTableWorkSpeedFactor, 0.1f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.WorkSpeedGlobal, 0.1f);
            //    yield break;
            //}

            //if (worktype == WorkTypeDefOf.Doctor)
            //{
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.BaseHealingQuality, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.MedicalPotency, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.MedicalTreatmentQualityFactor, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.SurgerySuccessChance, 1.0f);                
            //    yield break;
            //}

            //if (worktype == WorkTypeDefOf.Firefighter)
            //    yield break;

            //if ((worktype == WorkTypeDefOf.Growing) ||
            //    (worktype == WorkTypeDefOf.PlantCutting))
            //{
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.MoveSpeed, 0.2f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.HarvestFailChance, -1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.PlantWorkSpeed, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.WorkSpeedGlobal, 0.1f);
            //    yield break;
            //}

            //if (worktype == WorkTypeDefOf.Handling)
            //{
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.MoveSpeed, 0.3f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.TameAnimalChance, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.TrainAnimalChance, 1.0f);
            //    yield break;
            //}

            //if (worktype == WorkTypeDefOf.Hauling)
            //{
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.MoveSpeed, 0.8f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.CarryingCapacity, 1.0f);
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.DoorOpenSpeed, 0.8f);
            //    yield break;
            //}

            //if (worktype == WorkTypeDefOf.Hunting)
            //    yield break;

            //if (worktype == WorkTypeDefOf.Mining)
            //{
            //    //yield return new KeyValuePair<StatDef, float>(StatDefOf.MoveSpeed, 0.1f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.MiningSpeed, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.WorkSpeedGlobal, 0.1f);
            //    yield break;
            //}            

            //if (worktype == WorkTypeDefOf.Research)
            //{
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.ResearchSpeed, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.ResearchSpeedFactor, 1.0f);
            //    yield break;
            //}

            //if (worktype == WorkTypeDefOf.Warden)
            //{
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.RecruitPrisonerChance, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.SellPriceFactor, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.SocialImpact, 1.0f);
            //    yield return new KeyValuePair<StatDef, float>(StatDefOf.TradePriceImprovement, 1.0f);
            //    yield break;
            //}

            //yield return new KeyValuePair<StatDef, float>(StatDefOf.TrapSpringChance, 1.0f);
            //yield return new KeyValuePair<StatDef, float>(StatDefOf.WorkSpeedGlobal, 0.1f);
            //yield return new KeyValuePair<StatDef, float>(StatDefOf.WorkTableWorkSpeedFactor, 0.1f);
        }

        public static StringBuilder logMessage;

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
            //Scribe_Collections.LookList(ref this.pawnCache, "pawnCache", LookMode.Deep);
            Scribe_Values.LookValue(ref this.nextOptimization, "nextOptimization", 0);
            base.ExposeData();

            if (this.outfitCache == null)
                this.outfitCache = new List<Saveable_Outfit>();

            if (this.pawnCache == null)
                this.pawnCache = new List<Saveable_PawnNextApparelConfiguration>();
        }

        public Saveable_Outfit GetOutfit(Pawn pawn) { return this.GetOutfit(pawn.outfits.CurrentOutfit); }

        public Saveable_Outfit GetOutfit(Outfit outfit)
        {
            foreach (Saveable_Outfit o in this.outfitCache)
                if (o.outfit == outfit)
                    return o;

            Saveable_Outfit ret = new Saveable_Outfit();
            ret.outfit = outfit;
            ret.stats.Add(new Saveable_Outfit_StatDef() { statDef = StatDefOf.ArmorRating_Sharp, strength = 1.00f });
            ret.stats.Add(new Saveable_Outfit_StatDef() { statDef = StatDefOf.ArmorRating_Blunt, strength = 0.75f });

            this.outfitCache.Add(ret);

            return ret;
        }

        public Saveable_PawnNextApparelConfiguration GetCache(Pawn pawn)
        {
            foreach (Saveable_PawnNextApparelConfiguration c in this.pawnCache)
                if (c.pawn == pawn)
                    return c;
            Saveable_PawnNextApparelConfiguration n = new Saveable_PawnNextApparelConfiguration();
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

            this.pawnCache = new List<Saveable_PawnNextApparelConfiguration>();
            List<Apparel> allApparels = new List<Apparel>(Find.ListerThings.ThingsInGroup(ThingRequestGroup.Apparel).OfType<Apparel>());            
            foreach (Pawn pawn in Find.ListerPawns.FreeColonists)
            {
                this.InjectTab(pawn.def);

                Saveable_PawnNextApparelConfiguration pawnConfiguration = new Saveable_PawnNextApparelConfiguration();
                pawnConfiguration.pawn = pawn;
                pawnConfiguration.fixedApparels = new List<Apparel>();
                pawnConfiguration.NormalizeCalculedStatDef();

                this.pawnCache.Add(pawnConfiguration);

                foreach (Apparel pawnApparel in pawn.apparel.WornApparel)
                    if (pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(pawnApparel))
                        allApparels.Add(pawnApparel);
                    else
                        pawnConfiguration.fixedApparels.Add(pawnApparel);
            }

            foreach (Saveable_PawnNextApparelConfiguration pawnConfiguration in this.pawnCache)
            {
                pawnConfiguration.outfit = pawnConfiguration.pawn.outfits.CurrentOutfit;
                pawnConfiguration.allApparels = new List<Apparel>(allApparels);
                pawnConfiguration.calculedApparel = new List<Apparel>(pawnConfiguration.fixedApparels);
                pawnConfiguration.optimized = false;
            }

            while (true)
            {
                bool changed = false;
                foreach (Saveable_PawnNextApparelConfiguration pawnConfiguration in this.pawnCache)
                {
                    pawnConfiguration.OptimeFromList(ref changed);

#if LOG && PARTIAL_OPTIMIZE
                    MapComponent_AutoEquip.logMessage.AppendLine("Optimization For Pawn: " + pawnConfiguration.pawn);
                    foreach (Apparel ap in pawnConfiguration.calculedApparel)
                        MapComponent_AutoEquip.logMessage.AppendLine("    * Apparel: " + ap);
                    MapComponent_AutoEquip.logMessage.AppendLine();
#endif
                }

                if (!changed)
                    break;

                foreach (Saveable_PawnNextApparelConfiguration pawnConfiguration in this.pawnCache)
                {
                    foreach (Apparel apprel in pawnConfiguration.calculedApparel)
                    {
                        float? apparalGainPercentual = null;

                        foreach (Saveable_PawnNextApparelConfiguration otherPawn in this.pawnCache)
                        {
                            if (otherPawn == pawnConfiguration)
                                continue;

                            foreach (Apparel otherApprel in otherPawn.calculedApparel)
                            {
                                if (otherApprel == apprel)
                                {
#if LOG && CONFLICT
                                    MapComponent_AutoEquip.logMessage.AppendLine("Conflict " + apprel);                                    
#endif
                                    //if (pawnConfiguration.pawn.apparel.WornApparel.Contains(apprel))
                                    //{
                                    //    otherPawn.LooseConflict(apprel);
                                    //    break;
                                    //}

                                    //if (otherPawn.pawn.apparel.WornApparel.Contains(apprel))
                                    //{
                                    //    pawnConfiguration.LooseConflict(apprel);
                                    //    break;
                                    //}

                                    if (!apparalGainPercentual.HasValue)
                                    {
                                        pawnConfiguration.NormalizeTotalStats();
                                        float noStats = pawnConfiguration.ApparelTotalStats(pawnConfiguration.calculedApparel, apprel);
                                        if (noStats == 0)
                                            Log.Warning("No Stat to optimize apparel");
                                        apparalGainPercentual = pawnConfiguration.totalStats.Value / noStats;

                                        if (pawnConfiguration.pawn.apparel.WornApparel.Contains(apprel))
                                            apparalGainPercentual *= 1.3f;
                                    }

                                    otherPawn.NormalizeTotalStats();

                                    float otherNoStats = otherPawn.ApparelTotalStats(otherPawn.calculedApparel, apprel);
                                    if (otherNoStats == 0)
                                        Log.Warning("No Stat to optimize apparel");
                                    float otherApparalGainPercentual = otherPawn.totalStats.Value / otherNoStats;

                                    if (otherPawn.pawn.apparel.WornApparel.Contains(apprel))
                                        otherApparalGainPercentual *= 1.3f;

                                    if (apparalGainPercentual.Value > otherApparalGainPercentual)
                                    {
                                        otherPawn.LooseConflict(apprel);
                                        break;
                                    }
                                    else
                                    {
                                        pawnConfiguration.LooseConflict(apprel);
                                        break;
                                    }
                                }
                            }

                            if ((!pawnConfiguration.optimized) ||
                                (!otherPawn.optimized))
                                break;
                        }

                        if (!pawnConfiguration.optimized)
                            break;
                    }
                }
            }

            foreach (Saveable_PawnNextApparelConfiguration pawnConfiguration in this.pawnCache)
            {
                pawnConfiguration.toWearApparel = new List<Apparel>();

                List<Apparel> pawnApparel = new List<Apparel>(pawnConfiguration.pawn.apparel.WornApparel);
                foreach (Apparel ap in pawnConfiguration.calculedApparel)
                {
                    if (pawnApparel.Contains(ap))
                    {
                        pawnApparel.Remove(ap);
                        continue;
                    }
                    pawnConfiguration.toWearApparel.Add(ap);
                }
                foreach (Apparel ap in pawnApparel)
                    pawnConfiguration.toDropApparel.Add(ap);

#if LOG && CHANGES
                if (pawnConfiguration.toWearApparel.Any() || pawnConfiguration.toDropApparel.Any())
                {
                    MapComponent_AutoEquip.logMessage.AppendLine();
                    MapComponent_AutoEquip.logMessage.AppendLine("Apparel Change for: " + pawnConfiguration.pawn);

                    foreach (Apparel ap in pawnConfiguration.toDropApparel)
                        MapComponent_AutoEquip.logMessage.AppendLine(" * Drop: " + ap);

                    foreach (Apparel ap in pawnConfiguration.toWearApparel)
                        MapComponent_AutoEquip.logMessage.AppendLine(" * Wear: " + ap);
                }
#endif

                pawnConfiguration.fixedApparels = null;
                pawnConfiguration.allApparels = null;
                pawnConfiguration.outfit = null;
                pawnConfiguration.totalStats = null;
            }

#if LOG
            Log.Message(MapComponent_AutoEquip.logMessage.ToString());
            MapComponent_AutoEquip.logMessage = null;

            this.nextOptimization = Find.TickManager.TicksGame + 500;
#else
            this.nextOptimization = Find.TickManager.TicksGame + 3000;
#endif
        }

        private void InjectTab(ThingDef thingDef)
        {
            if (thingDef.inspectorTabsResolved == null)
            {
                thingDef.inspectorTabsResolved = new List<ITab>();
                foreach (Type current in thingDef.inspectorTabs)
                    thingDef.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(current));
            }

            if (!thingDef.inspectorTabsResolved.OfType<ITab_Pawn_AutoEquip>().Any())
                thingDef.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(typeof(ITab_Pawn_AutoEquip)));

            for (int i = thingDef.inspectorTabsResolved.Count - 1; i >= 0; i--)
                if (thingDef.inspectorTabsResolved[i].GetType() == typeof(ITab_Pawn_Gear))
                    thingDef.inspectorTabsResolved.RemoveAt(i);

            for (int i = thingDef.inspectorTabs.Count - 1; i >= 0; i--)
                if (thingDef.inspectorTabs[i] == typeof(ITab_Pawn_Gear))
                    thingDef.inspectorTabs.RemoveAt(i);
        }
    }
}
