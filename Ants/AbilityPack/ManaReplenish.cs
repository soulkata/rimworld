using System;

namespace AbilityPack
{
	public abstract class ManaReplenish
	{
		public abstract float Replenish(Saveable_Caster pawn);

		public abstract bool Visible(Saveable_Caster pawn);
	}
}
