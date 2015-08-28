using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
    public abstract class AbilityTarget
    {
        public abstract IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn);
    }
}
