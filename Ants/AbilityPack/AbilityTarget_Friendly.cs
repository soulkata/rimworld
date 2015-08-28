using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
	public class AbilityTarget_Friendly : AbilityTarget
	{
		public AbilityTarget optionalSource;

		public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
		{
			if (this.optionalSource == null)
			{
				return Find.ListerPawns.PawnsInFaction(pawn.pawn.Faction).OfType<Thing>();
			}
			return this.Filter(ability, pawn);
		}

		public IEnumerable<Thing> Filter(AbilityDef ability, Saveable_Caster pawn)
		{
			foreach (Thing current in this.optionalSource.Targets(ability, pawn))
			{
				Pawn pawn2 = current as Pawn;
				if (pawn2 != null)
				{
					if (pawn2.Faction == pawn.pawn.Faction)
					{
						yield return current;
					}
				}
				else
				{
					Corpse corpse = current as Corpse;
					if (corpse != null && corpse.innerPawn.Faction == pawn.pawn.Faction)
					{
						yield return current;
					}
				}
			}
			yield break;
		}
	}
}
