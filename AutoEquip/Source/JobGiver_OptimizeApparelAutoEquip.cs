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
		private static StringBuilder debugSb;
        public static bool debugApparelOptimize = false;

		private static readonly SimpleCurve InsulationColdScoreFactorCurve_NeedWarm = new SimpleCurve
		{
			new CurvePoint(-30f, 8f),
			new CurvePoint(0f, 1f)
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
            if (!JobGiver_OptimizeApparelAutoEquip.debugApparelOptimize)
			{
				if (Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick)
				{
					return null;
				}
			}
			else
			{
                JobGiver_OptimizeApparelAutoEquip.debugSb = new StringBuilder();
                JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine(string.Concat(new object[]
				{
					"Scanning for ",
					pawn,
					" at ",
					pawn.Position
				}));
			}

            #region [  Drops forbidden Apparel  ]

            Outfit currentOutfit = pawn.outfits.CurrentOutfit;
            List<Apparel> wornApparel = pawn.apparel.WornApparel;
            for (int i = wornApparel.Count - 1; i >= 0; i--)
            {
                Apparel t;
                if (!currentOutfit.filter.Allows(wornApparel[i]) && pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[i]) && pawn.apparel.TryDrop(wornApparel[i], out t))
                {
                    t.SetForbidden(false, false);
                    Job job = HaulAIUtility.HaulToStorageJob(pawn, t);
                    if (job != null)
                    {
                        return job;
                    }
                }
            } 

            #endregion

			Thing thing = null;			
			List<Thing> list = Find.ListerThings.ThingsInGroup(ThingRequestGroup.Apparel);
			if (list.Count > 0)
			{
                JobGiver_OptimizeApparelAutoEquip.neededWarmth = PawnApparelGenerator.CalculateNeededWarmth(pawn, GenDate.CurrentMonth);
                float num = 0f;
                for (int j = 0; j < list.Count; j++)
                {
                    Apparel apparel = (Apparel)list[j];
                    
                    if (currentOutfit.filter.Allows(apparel))
                    {
                        if (Find.SlotGroupManager.SlotGroupAt(apparel.Position) != null)
                        {
                            if (!apparel.IsForbidden(pawn))
                            {
                                float num2 = JobGiver_OptimizeApparelAutoEquip.ApparelScoreGain(pawn, apparel);
                                //if (JobGiver_OptimizeApparelAutoEquip.debugApparelOptimize)
                                //{
                                //    JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine(apparel.LabelCap + ": " + num2.ToString("F2"));
                                //}

                                //if (num2 >= num)
                                if (num2 >= 0.01f && num2 >= num)
                                {
                                    if (pawn.CanReserveAndReach(apparel, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1))
                                    {
                                        thing = apparel;
                                        num = num2;
                                    }
                                }
                            }
                        }
                    }
                }                
			}
            else
            {
                //Log.Message("A");
                //foreach (Thing t in Find.ListerThings.AllThings.Where(i => i.def.IsMeleeWeapon || i.def.IsRangedWeapon))
                //    Log.Message("B" + t.ToString());

                //list = Find.ListerThings.ThingsInGroup(ThingRequestGroup.thin);
            }

            if (thing != null)
            {
                if (JobGiver_OptimizeApparelAutoEquip.debugApparelOptimize)
                {
                    JobGiver_OptimizeApparelAutoEquip.debugSb.AppendLine("BEST: " + thing);                    
                    Log.Message(JobGiver_OptimizeApparelAutoEquip.debugSb.ToString());
                    JobGiver_OptimizeApparelAutoEquip.debugSb = null;
                }

                return new Job(JobDefOf.Wear, thing);
            }

            if (JobGiver_OptimizeApparelAutoEquip.debugApparelOptimize)
                JobGiver_OptimizeApparelAutoEquip.debugSb = null;
            this.SetNextOptimizeTick(pawn);
            return null;            
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

        public static float ApparalScoreRawInsulationColdAjust(Apparel ap)
        {
            float num3 = 1f;
            if (JobGiver_OptimizeApparelAutoEquip.neededWarmth == NeededWarmth.Warm)
            {
                float statValueAbstract = ap.def.GetStatValueAbstract(StatDefOf.Insulation_Cold, null);
                num3 *= JobGiver_OptimizeApparelAutoEquip.InsulationColdScoreFactorCurve_NeedWarm.Evaluate(statValueAbstract);
            }
            return num3;
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

        public static float ApparelScoreRawStats(Pawn pawn, Apparel ap)
        {
            float num = 0.0f;
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
	}
}
