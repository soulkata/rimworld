using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AutoEquip
{
    public class Saveable_PawnNextApparelConfiguration //: IExposable
    {
        public Pawn pawn;
        public List<Apparel> toWearApparel = new List<Apparel>();
        public List<Apparel> toDropApparel = new List<Apparel>();

        //public void ExposeData()
        //{
        //    Scribe_References.LookReference(ref this.pawn, "pawn");
        //    Scribe_Collections.LookList(ref this.toWearApparel, "toWearApparel", LookMode.MapReference);
        //    Scribe_Collections.LookList(ref this.toDropApparel, "toDropApparel", LookMode.MapReference);
        //}

        public bool optimized;
        public List<Apparel> calculedApparel;
        public List<Apparel> fixedApparels;
        public List<Apparel> allApparels;
        public Outfit outfit;
        public float needWarmCurve;
        public float needCoolCurve;
        public float? totalStats = null;
        public List<Saveable_Outfit_StatDef> calculedStatDef;

        public void EndCalc()
        {
            this.fixedApparels = null;
            this.allApparels = null;
        }

        public void CalculateNeededWarmth(Month month)
        {
            float num = (GenTemperature.OutdoorTemp + GenTemperature.AverageTemperatureAtWorldCoordsForMonth(Find.Map.WorldCoords, month) * 2) / 3;
            float stat = this.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin);

            this.needWarmCurve = new SimpleCurve
                {
                    new CurvePoint(stat - 10f, 1.5f),
                    new CurvePoint(stat - 4f, 1f),
                    new CurvePoint(stat, 0.5f), 
                    new CurvePoint(stat + 4f, 0.2f), 
                    new CurvePoint(stat + 10f, 0.0f)
                }.Evaluate(num);

            stat = this.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax);
            this.needCoolCurve = new SimpleCurve 
                {
                    new CurvePoint(stat - 10f, 0.0f), 
                    new CurvePoint(stat - 4f, 0.2f), 
                    new CurvePoint(stat, 0.5f), 
                    new CurvePoint(stat + 4f, 1f), 
                    new CurvePoint(stat + 10f, 1.5f) 
                }.Evaluate(num);

            //Log.Message("Pawn: " + this.pawn.LabelCap + "     Temp: " + num + Environment.NewLine +
            //    "     MinTemp: " + this.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin) + Environment.NewLine +
            //    "     MinCurv: " + this.needWarmCurve + Environment.NewLine +
            //    "     MaxTemp: " + this.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax) + Environment.NewLine +
            //    "     MaxCurv: " + this.needCoolCurve + Environment.NewLine);
        }        

        public float GetStatValueAbstract(StatDef stat)
        {
            return this.pawn.def.GetStatValueAbstract(stat, null);
        }

        public void OptimeFromList(ref bool changed)
        {
            if (this.optimized)
                return;

            while (true)
            {
                Apparel changeApparel = null;
                float changeApparelScore = 0;
                
                Outfit currentOutfit = this.pawn.outfits.CurrentOutfit;

                foreach (Apparel apparel in this.allApparels)
                {
                    if ((!apparel.IsForbidden(this.pawn)) &&
                        (currentOutfit.filter.Allows(apparel)))
                    {
                        float apparelScore;
                        if (this.ApparelScoreGain(apparel, out apparelScore))
                        {
                            if ((apparelScore > changeApparelScore) ||
                                ((apparelScore == changeApparelScore) && (pawn.apparel.WornApparel.Contains(apparel))))
                            {
                                changeApparel = apparel;
                                changeApparelScore = apparelScore;
                            }
                        }
                    }
                }

                if (changeApparel == null)
                {
                    this.optimized = true;
                    return;
                }
                else
                {
                    changed = true;
                    allApparels.Remove(changeApparel);
                    this.calculedApparel.Add(changeApparel);
                }
            }
        }

        public bool ApparelScoreGain(Apparel ap, out float gain)
        {
            if (ap.def == ThingDefOf.Apparel_PersonalShield && pawn.equipment.Primary != null && !pawn.equipment.Primary.def.Verbs[0].MeleeRange)
            {
                gain = -1000f;
                return false;
            }

            gain = this.ApparelScoreRaw(ap);
            //bool dropAny = false;
            foreach (Apparel wornApparel in this.calculedApparel)
            {
                if (!ApparelUtility.CanWearTogether(wornApparel.def, ap.def))
                {
                    if (!pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel))
                        return false;
                    gain -= this.ApparelScoreRaw(wornApparel);
                    //dropAny = true;
                }
            }
            //if (!dropAny)
            //    gain = Math.Max(gain, 0.0001f);

            return true;
        }

        public float ApparelScoreRaw(Apparel ap)
        {
            return this.ApparelScoreRawStats(ap)
                * this.ApparelModifierRaw(ap);
        }

        public float ApparelModifierRaw(Apparel ap)
        {
            float modHit = this.ApparelScoreRawHitPointAjust(ap);
            float modCold = this.ApparalScoreRawInsulationColdAjust(ap);
            if ((modHit < 0) && (modCold < 0))
                return modHit * modCold * -1;
            else
                return modHit * modCold;
        }

        public delegate void ApparelScoreRawStatsHandler(Pawn pawn, Apparel apparel, StatDef statDef, ref float num);
        public static event ApparelScoreRawStatsHandler ApparelScoreRawStatsHandlers;

        public float ApparelScoreRawStats(Apparel ap)
        {
            float num = 1.0f;
            float count = 1.0f;
            foreach (Saveable_Outfit_StatDef stat in this.GetStats())
            {
                try
                {
                    float nint = GetStatValue(ap, stat);
                    num += nint * stat.strength;
                    count++;
                }
                catch (Exception e)
                {
                    throw new Exception("Error Calculation Stat: " + stat.statDef, e);
                }
            }
            return num / count;
        }

        public float GetStatValue(Apparel ap, Saveable_Outfit_StatDef stat)
        {
            float baseStat = ap.GetStatValue(stat.statDef, true);
            float currentStat = baseStat;
            currentStat += ap.def.equippedStatOffsets.GetStatOffsetFromList(stat.statDef);

            if (ApparelScoreRawStatsHandlers != null)
                ApparelScoreRawStatsHandlers(pawn, ap, stat.statDef, ref currentStat);

            if (baseStat == 0)
                return currentStat;
            else
                return currentStat / baseStat;
        }

        public float ApparelScoreRawHitPointAjust(Apparel ap)
        {
            if (ap.def.useHitPoints)
            {
                float x = (float)ap.HitPoints / (float)ap.MaxHitPoints;
                return JobGiver_OptimizeApparelAutoEquip.HitPointsPercentScoreFactorCurve.Evaluate(x);
            }
            else
                return 1;
        }

        public float ApparalScoreRawInsulationColdAjust(Apparel ap)
        {
            float num3 = 1f;
            if (this.needWarmCurve != 0)
            {
                float statValueAbstract = ap.GetStatValue(StatDefOf.Insulation_Cold);
                num3 *= JobGiver_OptimizeApparelAutoEquip.InsulationColdScoreFactorCurve_NeedWarm.Evaluate(statValueAbstract) * this.needWarmCurve;
            }
            if (this.needCoolCurve != 0)
            {
                float statValueAbstract = ap.GetStatValue(StatDefOf.Insulation_Heat);
                num3 *= JobGiver_OptimizeApparelAutoEquip.InsulationColdScoreFactorCurve_NeedCold.Evaluate(statValueAbstract) * this.needCoolCurve;
            }
            return num3;
        }

        public float ApparelTotalStats(IEnumerable<Apparel> apparels, Apparel ignore)
        {
            float num = 1.0f;
            foreach (Saveable_Outfit_StatDef stat in this.GetStats())
            {
                float nint = stat.statDef.defaultBaseValue;

                foreach (Apparel a in apparels)
                {
                    if (a == ignore)
                        continue;
                    nint += a.def.equippedStatOffsets.GetStatOffsetFromList(stat.statDef);
                }

                foreach (Apparel a in apparels)
                {
                    if (a == ignore)
                        continue;

                    if (ApparelScoreRawStatsHandlers != null)
                        ApparelScoreRawStatsHandlers(pawn, a, stat.statDef, ref nint);
                }

                num += nint * stat.strength;
            }

            return num;
        }

        private IEnumerable<Saveable_Outfit_StatDef> GetStats() { return this.calculedStatDef; }

        public void NormalizeTotalStats()
        {
            if (!this.totalStats.HasValue)
            {
                this.totalStats = this.ApparelTotalStats(this.calculedApparel, null);
                if (this.totalStats.Value == 0)
                    Log.Warning("No Stat to optimize apparel");
            }
        }

        public void NormalizeCalculedStatDef()
        {
            if (this.calculedStatDef == null)
            {
                this.calculedStatDef = new List<Saveable_Outfit_StatDef>(MapComponent_AutoEquip.Get.GetOutfit(this.pawn).stats);

                foreach (WorkTypeDef wType in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder)
                {
                    int priority = this.pawn.workSettings.GetPriority(wType);
                    
                    float priorityAjust;
                    switch (priority)
                    {                        
                        case 1:
                            priorityAjust = 0.5f;
                            break;
                        case 2:
                            priorityAjust = 0.25f;
                            break;
                        case 3:
                            priorityAjust = 0.125f;
                            break;
                        case 4:
                            priorityAjust = 0.0625f;
                            break;
                        default:
                            continue;
                    }

                    foreach (KeyValuePair<StatDef, float> stat in MapComponent_AutoEquip.GetStatsOfWorkType(wType))
                    {
                        Saveable_Outfit_StatDef statdef = null;
                        foreach (Saveable_Outfit_StatDef s in this.calculedStatDef)
                        {
                            if (s.statDef == stat.Key)
                            {
                                statdef = s;
                                break;
                            }
                        }

                        if (statdef == null)
                        {
                            statdef = new Saveable_Outfit_StatDef();
                            statdef.statDef = stat.Key;
                            statdef.strength = stat.Value * priorityAjust;
                            this.calculedStatDef.Add(statdef);
                        }
                        else
                            statdef.strength = Math.Max(statdef.strength, stat.Value * priorityAjust);
                    }
                }

                this.calculedStatDef = new List<Saveable_Outfit_StatDef>(this.calculedStatDef.OrderByDescending(i => Math.Abs(i.strength)));

                //Log.Message(" ");
                //Log.Message("Stats of Pawn " + this.pawn);
                //foreach (Saveable_Outfit_StatDef s in this.calculedStatDef)
                //    Log.Message("  * " + s.strength.ToString("N5") + " - " + s.statDef.label);
            }
        }

        public void LooseConflict(Apparel apprel)
        {
            this.optimized = false;
            this.totalStats = null;
            if (!this.calculedApparel.Remove(apprel))
                Log.Warning("Warning on LooseConflict loser didnt have the apparel");
            this.calculedApparel.Remove(apprel);
        }
    }
}
