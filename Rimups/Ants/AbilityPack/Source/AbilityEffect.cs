using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AbilityPack
{
    public abstract class AbilityEffect
    {
        public abstract bool TryStart(AbilityDef ability, Saveable_Caster caster, ref List<Thing> targets, ref Saveable effectState);
        public abstract IEnumerable<Toil> MakeNewToils(JobDriver_AbilityEffect jobDriver, Saveable_Caster caster, IEnumerable<Thing> targets, Saveable effectState);
        public abstract void ExecuteWhileIncapacitated(Saveable_Caster caster, IEnumerable<Thing> targets, Saveable effectState);
    }
}
