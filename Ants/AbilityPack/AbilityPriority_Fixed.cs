using System;
using Verse;

namespace AbilityPack
{
	public class AbilityPriority_Fixed : AbilityPriority
	{
		public int value;

		public override int GetPriority(AbilityDef ability, Pawn pawn)
		{
			return this.value;
		}
	}
}
