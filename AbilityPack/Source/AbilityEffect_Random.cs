using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AbilityPack
{
    public class AbilityEffect_Random : AbilityEffect
    {
        public List<AbilityEffect> items;

        public override bool TryStart(AbilityDef ability, Saveable_Caster caster, ref List<Thing> targets, ref IExposable effectState)
        {
            AbilityEffect_RandomState state = new AbilityEffect_RandomState();
            state.item = Rand.Range(0, this.items.Count);
            effectState = state;

            return this.items[state.item].TryStart(ability, caster, ref targets, ref state.effectState);
        }

        public override IEnumerable<Toil> MakeNewToils(JobDriver_AbilityEffect jobDriver, Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
        {
            AbilityEffect_RandomState typedState = (AbilityEffect_RandomState)effectState;
            return this.items[typedState.item].MakeNewToils(jobDriver, caster, targets, typedState.effectState);
        }

        public override void ExecuteWhileIncapacitated(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
        {
            AbilityEffect_RandomState typedState = (AbilityEffect_RandomState)effectState;
            this.items[typedState.item].ExecuteWhileIncapacitated(caster, targets, typedState.effectState);
        }
    }

    public class AbilityEffect_RandomState : IExposable
    {
        public int item;
        public IExposable effectState;

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref this.item, "item");
            Scribe_Deep.LookDeep<IExposable>(ref this.effectState, "effectState");
        }
    }
}
