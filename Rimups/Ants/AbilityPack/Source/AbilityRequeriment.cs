using Verse;

namespace AbilityPack
{
    public abstract class AbilityRequeriment
    {
        public abstract bool Sucess(AbilityDef ability, Saveable_Caster pawn);
    }
}
