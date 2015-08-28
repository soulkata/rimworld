using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
    public class AbilityTarget_Race : AbilityTarget
    {
        public AbilityTarget target;
        public List<ThingDef> races;

        public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
        {
            foreach (Thing thing in this.target.Targets(ability, pawn))
            {
                Pawn thingPawn = thing as Pawn;
                if (thingPawn != null)
                {
                    if (this.races.Contains(thingPawn.def))
                        yield return thing;
                    continue;
                }

                Corpse corpsePawn = thing as Corpse;
                if (corpsePawn != null)
                {
                    if (this.races.Contains(corpsePawn.innerPawn.def))
                        yield return thing;
                    continue;
                }
            }
        }
    }
}
