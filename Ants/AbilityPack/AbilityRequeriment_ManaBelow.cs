using System;

namespace AbilityPack
{
	public class AbilityRequeriment_ManaBelow : AbilityRequeriment
	{
		public float value;

		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			return pawn.manaValue <= this.value;
		}
	}
}
