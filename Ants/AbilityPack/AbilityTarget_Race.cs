using System;
using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
	public class AbilityTarget_Race : AbilityTarget
	{
		public AbilityTarget target;

		public List<ThingDef> races;

		public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
		{
			foreach (Thing current in this.target.Targets(ability, pawn))
			{
				Pawn pawn2 = current as Pawn;
				if (pawn2 != null)
				{
					if (this.races.Contains(pawn2.def))
					{
						yield return current;
					}
				}
				else
				{
					Corpse corpse = current as Corpse;
					if (corpse != null && this.races.Contains(corpse.innerPawn.def))
					{
						yield return current;
					}
				}
			}
			yield break;
		}
	}
}
