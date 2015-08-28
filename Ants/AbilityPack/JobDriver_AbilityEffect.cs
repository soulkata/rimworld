using System;
using System.Collections.Generic;
using Verse.AI;

namespace AbilityPack
{
	public class JobDriver_AbilityEffect : JobDriver
	{
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Saveable_Caster pawnHabilty = MapComponent_Ability.GetOrCreate().GetPawnHabilty(this.pawn);
			return pawnHabilty.currentAbility.effect.MakeNewToils(this, pawnHabilty, pawnHabilty.Targets, pawnHabilty.effectState);
		}
	}
}
