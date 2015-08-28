using System;
using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
	public class AbilityTarget_ThingInCategory : AbilityTarget
	{
		public static Thing[] emptyThings = new Thing[0];

		public ThingRequestGroup group;

		public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
		{
			IEnumerable<Thing> enumerable = Find.ListerThings.ThingsMatching(new ThingRequest
			{
				group = this.group
			});
			if (enumerable == null)
			{
				return AbilityTarget_ThingInCategory.emptyThings;
			}
			return enumerable;
		}
	}
}
