using RimWorld;
using RimWorld.SquadAI;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
	public class AbilityEffect_Evolve : AbilityEffect_Cast
	{
		public List<AbilityEffect_UtilityChangeKind> items;

		public override bool TryStart(AbilityDef ability, Saveable_Caster caster, ref List<Thing> target, ref IExposable effectState)
		{
			if (!base.TryStart(ability, caster, ref target, ref effectState))
			{
				return false;
			}
			if (target == null || this.items == null || !this.items.Any<AbilityEffect_UtilityChangeKind>())
			{
				return false;
			}
			target = (from i in target.OfType<Pawn>()
			where this.items.SelectMany((AbilityEffect_UtilityChangeKind j) => j.@from).Contains(i.def)
			select i).OfType<Thing>().ToList<Thing>();
			return target.Any<Thing>();
		}

		public override void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
		{
			MapComponent_Ability.GetOrCreate();
			using (IEnumerator<Thing> enumerator = targets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Pawn target = (Pawn)enumerator.Current;
					AbilityEffect_UtilityChangeKind abilityEffect_UtilityChangeKind = this.items.First((AbilityEffect_UtilityChangeKind i) => i.from.Contains(target.def));
					Brain squadBrain = BrainUtility.GetSquadBrain(target);
					foreach (PawnKindDef current in abilityEffect_UtilityChangeKind.to)
					{
						Pawn pawn = AbilityEffect_Revive.Copy(caster.pawn, current, target.Faction, false, false, false);
						GenSpawn.Spawn(pawn, target.Position);
						if (squadBrain != null)
						{
							squadBrain.AddPawn(pawn);
						}
					}
					Building building = StoreUtility.StoringBuilding(target);
					if (building != null)
					{
						((Building_Storage)building).Notify_LostThing(target);
					}
					target.Destroy(0);
				}
			}
		}
	}
}
