using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
    public class AbilityTarget_Take : AbilityTarget
    {
        public int count;
        public AbilityTarget target;

        public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn) { return this.target.Targets(ability, pawn).Take(this.count); }
    }
}
