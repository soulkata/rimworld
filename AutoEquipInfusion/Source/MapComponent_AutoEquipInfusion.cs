using AutoEquip;
using Infusion;
using RimWorld;
using Verse;

namespace AutoEquipInfusion
{
    public class MapComponent_AutoEquipInfusion : MapComponent
    {
        static MapComponent_AutoEquipInfusion()
        {
            Log.Message("AutoEquip with Infusion Initialized");
            JobGiver_OptimizeApparelAutoEquip.ApparelScoreRawStatsHandlers += JobGiver_OptimizeApparelAutoEquip_ApparelScoreRawStatsHandlers;
        }

        static void JobGiver_OptimizeApparelAutoEquip_ApparelScoreRawStatsHandlers(Pawn pawn, Apparel apparel, StatDef stat, ref float val)
        {
            InfusionSet inf;
            if (apparel.TryGetInfusions(out inf))
            {               
                StatMod mod;                
                var prefix = inf.Prefix.ToInfusionDef();
                var suffix = inf.Suffix.ToInfusionDef();

                if (!inf.PassPre && prefix.GetStatValue(stat, out mod))
                {
                    val += mod.offset;
                    val *= mod.multiplier;
                }
                if (inf.PassSuf || !suffix.GetStatValue(stat, out mod))
                    return;

                val += mod.offset;
                val *= mod.multiplier;
            }
        }
    }
}
