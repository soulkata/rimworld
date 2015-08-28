namespace AbilityPack
{
    public class AbilityRequeriment_CasterDowned : AbilityRequeriment
    {
        public override bool Sucess(AbilityDef ability, Saveable_Caster pawn) { return pawn.pawn.Downed; }
    }
}
