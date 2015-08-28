using System;
using Verse;

namespace AbilityPack
{
	public class AbilityRequeriment_RandomChance : AbilityRequeriment
	{
		public float accepts;

		public float total;

		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			return Rand.Range(0f, this.total) < 64f * this.accepts;
		}
	}
}
