using AbilityPack;
using RimWorld;
using RimWorld.SquadAI;
using System.Linq;
using Verse;

namespace AlienAnts
{
    public class AbilityRequeriment_NoQueenInSquad : AbilityRequeriment
    {
        public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
        {            
            Brain brain = pawn.pawn.GetSquadBrain();
            if (brain == null)
                return false;

            MapComponent_Ability mapComponent = MapComponent_Ability.GetOrCreate();

            foreach (Pawn p in brain.ownedPawns)
            {
                if (p.def.defName == "Ant_Queen")
                    return false;

                Saveable_Caster save;
                if (mapComponent.TryGetPawnHability(p, out save))
                {
                    if ((save.currentAbility != null) &&
                        (save.currentAbility.defName == "AlienEvolveToPrincessAbility"))
                        return false;
                }
            }

            return true;
        }
    }
}
