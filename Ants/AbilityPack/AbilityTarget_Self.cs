using System;
using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
	public class AbilityTarget_Self : AbilityTarget
	{
		public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
		{
			yield return pawn.pawn;
			yield break;
		}
	}
}
