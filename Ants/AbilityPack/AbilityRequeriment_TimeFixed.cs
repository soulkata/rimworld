using System;

namespace AbilityPack
{
	public class AbilityRequeriment_TimeFixed : AbilityRequeriment
	{
		public int value;

		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			return pawn.GetLog(ability).ticksSinceExecution >= this.value;
		}
	}
}
