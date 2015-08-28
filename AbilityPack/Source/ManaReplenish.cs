using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AbilityPack
{
    public abstract class ManaReplenish
    {
        public abstract float Replenish(Saveable_Caster pawn);
        public abstract bool Visible(Saveable_Caster pawn);
    }
}
