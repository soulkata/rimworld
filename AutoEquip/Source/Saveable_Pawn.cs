using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AutoEquip
{
    public class Saveable_Pawn : IExposable
    {
        // Exposed members
        public Pawn pawn;
        public List<Saveable_StatDef> stats = new List<Saveable_StatDef>();

        public List<Apparel> toWearApparel = new List<Apparel>();
        public List<Apparel> toDropApparel = new List<Apparel>();
        public List<Apparel> targetApparel = new List<Apparel>();

        public void ExposeData()
        {
            Scribe_References.LookReference(ref this.pawn, "pawn");
            Scribe_Collections.LookList(ref this.stats, "stats", LookMode.Deep);
        }

        public IEnumerable<Saveable_StatDef> NormalizeCalculedStatDef()
        {
            Saveable_Outfit outFit = MapComponent_AutoEquip.Get.GetOutfit(this.pawn);
            List<Saveable_StatDef> calculedStatDef = new List<Saveable_StatDef>(outFit.stats);

            if ((outFit.appendIndividualPawnStatus) &&
                (this.stats != null))
            {
                foreach (Saveable_StatDef stat in this.stats)
                {
                    int index = -1;
                    for (int i = 0; i < calculedStatDef.Count; i++)
                    {
                        if (calculedStatDef[i].statDef == stat.statDef)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index == -1)
                        calculedStatDef.Add(stat);
                    else
                        calculedStatDef[index] = stat;
                }
            }

            //foreach (WorkTypeDef wType in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder)
            //{
            //    int priority = this.pawn.workSettings.GetPriority(wType);

            //    float priorityAjust;
            //    switch (priority)
            //    {                        
            //        case 1:
            //            priorityAjust = 0.5f;
            //            break;
            //        case 2:
            //            priorityAjust = 0.25f;
            //            break;
            //        case 3:
            //            priorityAjust = 0.125f;
            //            break;
            //        case 4:
            //            priorityAjust = 0.0625f;
            //            break;
            //        default:
            //            continue;
            //    }

            //    foreach (KeyValuePair<StatDef, float> stat in MapComponent_AutoEquip.GetStatsOfWorkType(wType))
            //    {
            //        Saveable_StatDef statdef = null;
            //        foreach (Saveable_StatDef s in calculedStatDef)
            //        {
            //            if (s.statDef == stat.Key)
            //            {
            //                statdef = s;
            //                break;
            //            }
            //        }

            //        if (statdef == null)
            //        {
            //            statdef = new Saveable_StatDef();
            //            statdef.statDef = stat.Key;
            //            statdef.strength = stat.Value * priorityAjust;
            //            calculedStatDef.Add(statdef);
            //        }
            //        else
            //            statdef.strength = Math.Max(statdef.strength, stat.Value * priorityAjust);
            //    }
            //}

            //Log.Message(" ");
            //Log.Message("Stats of Pawn " + this.pawn);
            //foreach (Saveable_Outfit_StatDef s in List<Saveable_StatDef>)
            //    Log.Message("  * " + s.strength.ToString("N5") + " - " + s.statDef.label);

            return calculedStatDef.OrderByDescending(i => Math.Abs(i.strength));            
        }        
    }
}
