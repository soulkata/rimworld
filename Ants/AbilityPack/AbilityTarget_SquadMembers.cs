using RimWorld.SquadAI;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
	public class AbilityTarget_SquadMembers : AbilityTarget
	{
		public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
		{
			Brain squadBrain = BrainUtility.GetSquadBrain(pawn.pawn);
			if (squadBrain == null)
			{
				return AbilityTarget_ThingInCategory.emptyThings;
			}
			return squadBrain.ownedPawns.OfType<Thing>();
		}
	}
}
