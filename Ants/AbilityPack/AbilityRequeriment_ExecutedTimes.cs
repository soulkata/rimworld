using System;

namespace AbilityPack
{
	public class AbilityRequeriment_ExecutedTimes : AbilityRequeriment
	{
		public int limit;

		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			return pawn.GetLog(ability).numberOfExecution < this.limit;
		}
	}
}
