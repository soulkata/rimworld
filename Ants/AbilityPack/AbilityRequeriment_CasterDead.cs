using System;

namespace AbilityPack
{
	public class AbilityRequeriment_CasterDead : AbilityRequeriment
	{
		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			return pawn.pawn.Dead;
		}
	}
}
