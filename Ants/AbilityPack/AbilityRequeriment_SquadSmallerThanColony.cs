using RimWorld.SquadAI;
using System;
using System.Linq;
using Verse;

namespace AbilityPack
{
	public class AbilityRequeriment_SquadSmallerThanColony : AbilityRequeriment
	{
		public float squadPowerMultiplier = 1f;

		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			Brain squadBrain = BrainUtility.GetSquadBrain(pawn.pawn);
			if (squadBrain == null)
			{
				return true;
			}
			return squadBrain.ownedPawns.Sum((Pawn i) => i.kindDef.combatPower) * this.squadPowerMultiplier < AbilityRequeriment_ColonyBiggerThan.ColonySize();
		}
	}
}
