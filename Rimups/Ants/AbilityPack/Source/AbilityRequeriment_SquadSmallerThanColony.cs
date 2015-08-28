using RimWorld;
using RimWorld.SquadAI;
using System.Linq;
using Verse;

namespace AbilityPack
{
    public class AbilityRequeriment_SquadSmallerThanColony : AbilityRequeriment
    {
        public float squadPowerMultiplier = 1;

        public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
        {
            Brain squad = pawn.pawn.GetSquadBrain();
            if (squad == null)
                return true;
            else
                return squad.ownedPawns.Sum(i => i.kindDef.pointsCost) * this.squadPowerMultiplier < AbilityRequeriment_ColonyBiggerThan.ColonySize();
        }
    }
}
