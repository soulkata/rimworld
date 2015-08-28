using Verse;

namespace AbilityPack
{
    public class AbilityRequeriment_ManaAbove : AbilityRequeriment
    {
        public float value;

        public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
        {
            return pawn.manaValue >= this.value;
        }
    }
}
