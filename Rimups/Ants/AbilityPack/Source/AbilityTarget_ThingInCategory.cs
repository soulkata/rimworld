using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
    public class AbilityTarget_ThingInCategory : AbilityTarget
    {
        public static Thing[] emptyThings = new Thing[0];

        public ThingRequestGroup group;

        public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn) 
        {
            IEnumerable<Thing> things = Find.ListerThings.ThingsMatching(new ThingRequest() { group = this.group });
            if (things == null)
                return AbilityTarget_ThingInCategory.emptyThings;
            else
                return things;
        }
    }
}
