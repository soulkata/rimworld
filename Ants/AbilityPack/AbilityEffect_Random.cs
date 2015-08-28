using System;
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
			AbilityEffect_RandomState abilityEffect_RandomState = new AbilityEffect_RandomState();
			abilityEffect_RandomState.item = Rand.Range(0, this.items.Count);
			effectState = abilityEffect_RandomState;
			return this.items[abilityEffect_RandomState.item].TryStart(ability, caster, ref targets, ref abilityEffect_RandomState.effectState);
		}

		public override IEnumerable<Toil> MakeNewToils(JobDriver_AbilityEffect jobDriver, Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
		{
			AbilityEffect_RandomState abilityEffect_RandomState = (AbilityEffect_RandomState)effectState;
			return this.items[abilityEffect_RandomState.item].MakeNewToils(jobDriver, caster, targets, abilityEffect_RandomState.effectState);
		}

		public override void ExecuteWhileIncapacitated(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
		{
			AbilityEffect_RandomState abilityEffect_RandomState = (AbilityEffect_RandomState)effectState;
			this.items[abilityEffect_RandomState.item].ExecuteWhileIncapacitated(caster, targets, abilityEffect_RandomState.effectState);
		}
	}
}
