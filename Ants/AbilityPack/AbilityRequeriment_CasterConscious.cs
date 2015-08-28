using System;

namespace AbilityPack
{
	public class AbilityRequeriment_CasterConscious : AbilityRequeriment
	{
		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			return !pawn.pawn.Dead && !pawn.pawn.Downed;
		}
	}
}
