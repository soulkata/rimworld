using System;
using Verse;

namespace AbilityPack
{
	public abstract class AbilityPriority
	{
		public abstract int GetPriority(AbilityDef ability, Pawn pawn);
	}
}
