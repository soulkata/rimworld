using Verse;

namespace AbilityPack
{
    public class ManaReplenish_Regenerate : ManaReplenish
    {
        public float ammount;
        public float ammountDead;
        public float ammountDowned;

        public override float Replenish(Saveable_Caster pawn)
        {
            if (pawn.pawn.Dead)
                return this.ammountDead;
            if (pawn.pawn.Downed)
                return this.ammountDowned;
            return this.ammount; 
        }

        public override bool Visible(Saveable_Caster pawn)
        {
            if (pawn.pawn.Dead)
                return this.ammountDead != 0;
            else
                if (pawn.pawn.Downed)
                    return this.ammountDowned != 0;
                else
                    return this.ammount != 0;
        }
    }
}
