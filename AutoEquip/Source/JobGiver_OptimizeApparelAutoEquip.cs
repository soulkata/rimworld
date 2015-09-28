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
		private const float MinScoreGainToCare = 0.09f;
		private const float ScoreFactorIfNotReplacing = 10f;

        public static readonly SimpleCurve InsulationColdScoreFactorCurve_NeedWarm = new SimpleCurve { new CurvePoint(-30f, 8f), new CurvePoint(0f, 1f), new CurvePoint(10f, 0f) };
        public static readonly SimpleCurve InsulationColdScoreFactorCurve_NeedCold = new SimpleCurve { new CurvePoint(-10f, -1f), new CurvePoint(0f, 1f), new CurvePoint(30f, 8f) };
        public static readonly SimpleCurve HitPointsPercentScoreFactorCurve = new SimpleCurve { new CurvePoint(0f, -1f), new CurvePoint(0.5001f, 0.0f), new CurvePoint(0.6f, 0.97f), new CurvePoint(1f, 1f) };

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

            if (Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick)
                return null;

            Saveable_PawnNextApparelConfiguration configurarion = MapComponent_AutoEquip.Get.GetCache(pawn);
            Outfit currentOutfit = pawn.outfits.CurrentOutfit;

            #region [  Wear Apparel  ]

            if (configurarion.toWearApparel.Count > 0)
            {
                List<Thing> list = Find.ListerThings.ThingsInGroup(ThingRequestGroup.Apparel);
                if (list.Count > 0)
                {
                    foreach (Apparel ap in list)
                    {
                        //if (Find.SlotGroupManager.SlotGroupAt(ap.Position) != null)
                        {
                            if (configurarion.toWearApparel.Contains(ap))
                            {
                                if (pawn.CanReserveAndReach(ap, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1))
                                {
#if LOG && JOBS
                Log.Message("Pawn " + pawn + " wear apparel: " + ap);
#endif
                                    configurarion.toWearApparel.Remove(ap);
                                    return new Job(JobDefOf.Wear, ap);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region [  Drops unequiped  ]

            for (int i = configurarion.toDropApparel.Count - 1; i >= 0; i--)
            {
                Apparel a = configurarion.toDropApparel[i];
                configurarion.toDropApparel.Remove(a);

#if LOG && JOBS
                Log.Message("Pawn " + pawn + " drop apparel: " + a);
#endif

                if (pawn.apparel.WornApparel.Contains(a))
                {
                    Apparel t;
                    if (pawn.apparel.TryDrop(a, out t))
                    {                        
                        t.SetForbidden(false, true);                       

                        Job job = HaulAIUtility.HaulToStorageJob(pawn, t);

                        if (job != null)
                            return job;
                        else
                        {
                            pawn.mindState.nextApparelOptimizeTick = Find.TickManager.TicksGame + 350;
                            return null;
                        }
                    }
                }
            }

            #endregion            

            pawn.mindState.nextApparelOptimizeTick = Find.TickManager.TicksGame + 350;
            return null;
        }        
	}
}
