using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
    public class AbilityRequeriment_Any : AbilityRequeriment
    {
        public List<AbilityRequeriment> items;

        public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
        {
            if (this.items == null)
                return false;
            foreach (AbilityRequeriment requeriment in this.items)
                if (requeriment.Sucess(ability, pawn))
                    return true;
            return false;
        }
    }
}
