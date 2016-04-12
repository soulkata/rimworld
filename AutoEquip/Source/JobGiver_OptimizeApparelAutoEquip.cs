using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace AutoEquip
{
    public class JobGiver_OptimizeApparelAutoEquip : ThinkNode_JobGiver
    {
        private const int ApparelOptimizeCheckInterval = 3000;

        private const float MinScoreGainToCare = 0.09f;

        private const float ScoreFactorIfNotReplacing = 10f;

        private static NeededWarmth neededWarmth;
#if LOG
        private static StringBuilder debugSb;
#endif

        private static readonly SimpleCurve InsulationColdScoreFactorCurve_NeedWarm = new SimpleCurve
        {
            new CurvePoint(-30f, 8f),
            new CurvePoint(0f, 1f)
        };

        private static readonly SimpleCurve InsulationWarmScoreFactorCurve_NeedCold = new SimpleCurve
        {
            new CurvePoint(30f, 8f),
            new CurvePoint(0f, 1f),
            new CurvePoint(-10, 0.1f)
        };

        private static readonly SimpleCurve HitPointsPercentScoreFactorCurve = new SimpleCurve
        {
            new CurvePoint(0f, 0f),
            new CurvePoint(0.25f, 0.15f),
            new CurvePoint(0.5f, 0.7f),
            new CurvePoint(1f, 1f)
        };

        private void SetNextOptimizeTick(Pawn pawn)
        {
            pawn.mindState.nextApparelOptimizeTick = Find.TickManager.TicksGame + 3000;
        }

        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            if (pawn.outfits == null)
            {
                Log.ErrorOnce(pawn + " tried to run JobGiver_OptimizeApparel without an OutfitTracker", 5643897);
                return null;
            }
            if (pawn.Faction != Faction.OfColony)
            {
                Log.ErrorOnce("Non-colonist " + pawn + " tried to optimize apparel.", 764323);
                return null;
            }
#if !LOG
            if (Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick)
                return null;
#else
            JobGiver_OptimizeApparelAutoEquip.debugSb = new StringBuilder();
            JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine(string.Concat(new object[]
            {
                "Scanning for ",
                pawn,
                " at ",
                pawn.Position
            }));
#endif

            #region [  Drops forbidden Apparel  ]

            Outfit currentOutfit = pawn.outfits.CurrentOutfit;
            List<Apparel> wornApparel = pawn.apparel.WornApparel;
            for (int i = wornApparel.Count - 1; i >= 0; i--)
            {
                if ((!HandleOutfitFilter(currentOutfit, wornApparel[i])) && pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[i]))
                {
                    return new Job(JobDefOf.RemoveApparel, wornApparel[i])
                    {
                        haulDroppedApparel = true
                    };
                }
            }

            #endregion

            #region [  If no Apparel are found, Delays the next search  ]

            Apparel thing = null;
            float num = 0f;
            List<Thing> list = Find.ListerThings.ThingsInGroup(ThingRequestGroup.Apparel);
            if (list.Count == 0)
            {
                this.SetNextOptimizeTick(pawn);
                return null;
            } 

            #endregion

            JobGiver_OptimizeApparelAutoEquip.neededWarmth = JobGiver_OptimizeApparelAutoEquip.CalculateNeededWarmth(pawn, GenDate.CurrentMonth);
#if LOG
            if (JobGiver_OptimizeApparelAutoEquip.neededWarmth != NeededWarmth.Any)
                JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine("Temperature: " + JobGiver_OptimizeApparelAutoEquip.neededWarmth);
#endif

            for (int j = 0; j < list.Count; j++)
			{
				Apparel apparel = (Apparel)list[j];

#if LOG
                JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine(apparel.LabelCap);
#endif

                if (HandleOutfitFilter(currentOutfit, apparel))
				{
					if (Find.SlotGroupManager.SlotGroupAt(apparel.Position) != null)
					{
						if (!apparel.IsForbidden(pawn))
						{
							float num2 = JobGiver_OptimizeApparelAutoEquip.ApparelScoreGain(pawn, apparel);
                            if (num2 >= 0.09f && num2 >= num)
							{
								if (ApparelUtility.HasPartsToWear(pawn, apparel.def))
								{
									if (pawn.CanReserveAndReach(apparel, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1))
									{
										thing = apparel;
										num = num2;
									}
#if LOG
                                    else
                                        JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine("  CantReserve");
#endif
                                }
                            }
						}
#if LOG
                        else
                            JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine("  IsForbidden");
#endif
                    }
#if LOG
                    else
                        JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine("  SlotGroupAtNull");
#endif
                }
#if LOG
                else
                    JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine("  FilterNotAllows");
#endif
            }

#if LOG
            if (thing != null)
            {
                JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine();
                JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine("BEST: " + thing.LabelCap + ":        Raw: " + ApparelScoreRaw(pawn, thing).ToString("F2") + "        Gain: " + JobGiver_OptimizeApparelAutoEquip.ApparelScoreGain(pawn, thing).ToString("F2"));
            }

            if (JobGiver_OptimizeApparelAutoEquip.debugSb.Length > 0)
                Log.Message(JobGiver_OptimizeApparelAutoEquip.debugSb.ToString());
            JobGiver_OptimizeApparelAutoEquip.debugSb = null;
#endif

            #region [  If no Apparel is Selected to Wear, Delays the next search  ]

            if (thing == null)
            {
                this.SetNextOptimizeTick(pawn);
                return null;
            } 

            #endregion

            return new Job(JobDefOf.Wear, thing);
        }

        public static bool HandleOutfitFilter(Outfit currentOutfit, Apparel apparel)
        {
            return (currentOutfit.filter.Allows(apparel));
        }

        public static float ApparelScoreGain(Pawn pawn, Apparel ap)
        {
            if (ap.def == ThingDefOf.Apparel_PersonalShield && pawn.equipment.Primary != null && !pawn.equipment.Primary.def.Verbs[0].MeleeRange)
            {
                return -1000f;
            }
            float num = JobGiver_OptimizeApparelAutoEquip.ApparelScoreRaw(pawn, ap);
            List<Apparel> wornApparel = pawn.apparel.WornApparel;
            bool flag = false;
            for (int i = 0; i < wornApparel.Count; i++)
            {
                if (!ApparelUtility.CanWearTogether(wornApparel[i].def, ap.def))
                {
                    if (!pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[i]))
                    {
                        return -1000f;
                    }
                    num -= JobGiver_OptimizeApparelAutoEquip.ApparelScoreRaw(pawn, wornApparel[i]);
                    flag = true;
                }
            }
            if (!flag)
            {
                num *= 10f;
            }
            return num;
        }

        public static float ApparelScoreRaw(Pawn pawn, Apparel ap)
        {
            float num = ApparelScoreRawStats(pawn, ap);
            num *= ApparelScoreRawHitPointAjust(ap);
            num *= ApparalScoreRawInsulationColdAjust(ap);
            return num;
        }

        public static float ApparelScoreRawStats(Pawn pawn, Apparel ap)
        {
            float num = 0.00f;
            foreach (Saveable_Outfit_StatDef stat in MapComponent_AutoEquip.Get.GetOutfit(pawn.outfits.CurrentOutfit).stats)
            {
                float nint = ap.GetStatValue(stat.statDef, true);
                nint += ap.def.equippedStatOffsets.GetStatOffsetFromList(stat.statDef);

                if (ApparelScoreRawStatsHandlers != null)
                    ApparelScoreRawStatsHandlers(pawn, ap, stat.statDef, ref nint);

                num += nint * stat.strength;
            }
            return num;
        }

        public static float ApparalScoreRawInsulationColdAjust(Apparel ap)
        {
            switch (JobGiver_OptimizeApparelAutoEquip.neededWarmth)
            {
                case NeededWarmth.Warm:
                    {
                        float statValueAbstract = ap.def.GetStatValueAbstract(StatDefOf.Insulation_Cold, null);
                        return JobGiver_OptimizeApparelAutoEquip.InsulationColdScoreFactorCurve_NeedWarm.Evaluate(statValueAbstract);
                    }
                case NeededWarmth.Cool:
                    {
                        float statValueAbstract = ap.def.GetStatValueAbstract(StatDefOf.Insulation_Heat, null);
                        return JobGiver_OptimizeApparelAutoEquip.InsulationWarmScoreFactorCurve_NeedCold.Evaluate(statValueAbstract);
                    }
                default:
                    return 1;
            }
        }

        public static float ApparelScoreRawHitPointAjust(Apparel ap)
        {
            if (ap.def.useHitPoints)
            {
                float x = (float)ap.HitPoints / (float)ap.MaxHitPoints;
                return JobGiver_OptimizeApparelAutoEquip.HitPointsPercentScoreFactorCurve.Evaluate(x);
            }
            else
                return 1;
        }

        public delegate void ApparelScoreRawStatsHandler(Pawn pawn, Apparel apparel, StatDef statDef, ref float num);
        public static event ApparelScoreRawStatsHandler ApparelScoreRawStatsHandlers;        

        public static NeededWarmth CalculateNeededWarmth(Pawn pawn, Month month)
        {
            float num = GenTemperature.AverageTemperatureAtWorldCoordsForMonth(Find.Map.WorldCoords, month);

            if (num < pawn.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin, null) - 4f)
                return NeededWarmth.Warm;

            if (num > pawn.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax, null) + 4f)
                return NeededWarmth.Cool;

            return NeededWarmth.Any;
        }
    }
}
