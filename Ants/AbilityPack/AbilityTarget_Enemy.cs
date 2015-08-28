using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AbilityPack
{
	public class AbilityTarget_Enemy : AbilityTarget
	{
		public AbilityTarget optionalSource;

		public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster pawn)
		{
			IEnumerable<Thing> result;
			if (this.optionalSource == null)
			{
				result = (IEnumerable<Thing>)GenAI.PawnTargetsFor(pawn.pawn.Faction);
			}
			else
			{
				result = this.Filter(ability, pawn);
			}
			return result;
		}

		public IEnumerable<Thing> Filter(AbilityDef ability, Saveable_Caster pawn)
		{
			foreach (Thing current in this.optionalSource.Targets(ability, pawn))
			{
				Pawn pawn2 = current as Pawn;
				if (pawn2 != null)
				{
					if (FactionUtility.HostileTo(pawn.pawn.Faction, pawn2.Faction))
					{
						yield return current;
					}
				}
				else
				{
					Corpse corpse = current as Corpse;
					if (corpse != null && FactionUtility.HostileTo(pawn.pawn.Faction, corpse.innerPawn.Faction))
					{
						yield return current;
					}
				}
			}
			yield break;
		}
	}
}
