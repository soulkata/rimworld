using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace AbilityPack
{
    public class AbilityTarget_Enemy : AbilityTarget
    {
        public AbilityTarget optionalSource;

        public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
        {
            if (this.optionalSource == null)
                return GenAI.PawnTargetsFor(pawn.pawn.Faction).Cast<Thing>();
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
                    if (FactionUtility.HostileTo(pawn.pawn.Faction, thingPawn.Faction))
                        yield return thing;
                    continue;
                }

                Corpse corpsePawn = thing as Corpse;
                if (corpsePawn != null)
                {
                    if (FactionUtility.HostileTo(pawn.pawn.Faction, corpsePawn.innerPawn.Faction))
                        yield return thing;
                    continue;
                }
            }
        }
    }
}
