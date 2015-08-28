using System.Collections.Generic;
using Verse.AI;

namespace AbilityPack
{
    public class JobDriver_AbilityEffect : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Saveable_Caster caster = MapComponent_Ability.GetOrCreate().GetPawnHabilty(this.pawn);
            return caster.currentAbility.effect.MakeNewToils(this, caster, caster.Targets, caster.effectState);
        }
    }
}
