using RimWorld;
using Verse;

namespace AbilityPack
{
    public class AbilityRequeriment_ColonyBiggerThan : AbilityRequeriment
    {
        public float value;
        public static float? forcedValue;

        public static float ColonySize()
        {
            if (AbilityRequeriment_ColonyBiggerThan.forcedValue.HasValue)
                return AbilityRequeriment_ColonyBiggerThan.forcedValue.Value;
            else
                return IncidentMakerUtility.DefaultParmsNow(Find.Storyteller.def, IncidentCategory.ThreatBig).points;
        }

        public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
        {
            return AbilityRequeriment_ColonyBiggerThan.ColonySize() >= this.value;
        }
    }
}
