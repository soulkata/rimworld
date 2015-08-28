using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
    public class AbilityTarget_Union : AbilityTarget
    {
        public List<AbilityTarget> targets;

        public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn) { return this.targets.SelectMany(i => i.Targets(ability, pawn)); }
    }
}
