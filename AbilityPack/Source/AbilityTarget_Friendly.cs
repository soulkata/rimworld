using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
    public class AbilityTarget_Friendly : AbilityTarget
    {
        public AbilityTarget optionalSource;

        public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn) 
        {
            if (this.optionalSource == null)
                return Find.ListerPawns.PawnsInFaction(pawn.pawn.Faction).OfType<Thing>();
            else
                return this.Filter(ability, pawn);
        }

        public IEnumerable<Thing> Filter(AbilityDef ability, Saveable_Caster pawn)
        {
            foreach (Thing thing in this.optionalSource.Targets(ability, pawn))
            {
                Pawn thingPawn = thing as Pawn;
                if (thingPawn != null)
                {
                    if (thingPawn.Faction == pawn.pawn.Faction)
                        yield return thing;
                    continue;
                }

                Corpse corpsePawn = thing as Corpse;
                if (corpsePawn != null)
                {
                    if (corpsePawn.innerPawn.Faction == pawn.pawn.Faction)
                        yield return thing;
                    continue;
                }
            }
        }
    }
}
