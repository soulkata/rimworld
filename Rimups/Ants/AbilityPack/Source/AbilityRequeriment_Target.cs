using System.Linq;

namespace AbilityPack
{
    public class AbilityRequeriment_Target : AbilityRequeriment
    {
        public AbilityTarget target;

        public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
        {
            if (this.target == null)
                return ability.target.Targets(ability, pawn).Any();
            else
                return this.target.Targets(ability, pawn).Any();
        }
    }
}
