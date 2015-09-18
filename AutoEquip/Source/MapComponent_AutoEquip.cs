using RimWorld;
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
            yield break;
            //yield return new KeyValuePair<StatDef, float>(StatDefOf.ArmorRating_Blunt, 0.0075f);
            //yield return new KeyValuePair<StatDef, float>(StatDefOf.ArmorRating_Sharp, 0.01f);

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
                                    if (pawnConfiguration.pawn.apparel.WornApparel.Contains(apprel))
                                    {
                                        otherPawn.LooseConflict(apprel);
                                        break;
                                    }

                                    if (otherPawn.pawn.apparel.WornApparel.Contains(apprel))
                                    {
                                        pawnConfiguration.LooseConflict(apprel);
                                        break;
                                    }

                                    if (!apparalGainPercentual.HasValue)
                                    {
                                        pawnConfiguration.NormalizeTotalStats();
                                        float noStats = pawnConfiguration.ApparelTotalStats(pawnConfiguration.calculedApparel, apprel);
                                        if (noStats == 0)
                                            Log.Warning("No Stat to optimize apparel");
                                        apparalGainPercentual = pawnConfiguration.totalStats.Value / noStats;
                                    }

                                    otherPawn.NormalizeTotalStats();

                                    float otherNoStats = otherPawn.ApparelTotalStats(otherPawn.calculedApparel, apprel);
                                    if (otherNoStats == 0)
                                        Log.Warning("No Stat to optimize apparel");
                                    float otherApparalGainPercentual = otherPawn.totalStats.Value / otherNoStats;

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

                pawnConfiguration.calculedApparel = null;
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
    }
}
