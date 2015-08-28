using RimWorld.SquadAI;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
    public class AbilityTarget_SquadMembers : AbilityTarget
    {
        public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
        {
            Brain brain = pawn.pawn.GetSquadBrain();
            if (brain == null)
                return AbilityTarget_ThingInCategory.emptyThings;
            return brain.ownedPawns.OfType<Thing>();
        }
    }
}
