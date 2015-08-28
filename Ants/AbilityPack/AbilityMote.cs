using System;
using Verse;

namespace AbilityPack
{
	public abstract class AbilityMote
	{
		public abstract void AbilityStarted(Saveable_Caster caster, Thing target);
	}
}
