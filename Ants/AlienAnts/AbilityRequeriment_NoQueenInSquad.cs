using AbilityPack;
using RimWorld.SquadAI;
using System;
using Verse;

namespace AlienAnts
{
	public class AbilityRequeriment_NoQueenInSquad : AbilityRequeriment
	{
		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			Brain squadBrain = BrainUtility.GetSquadBrain(pawn.pawn);
			bool flag;
			if (squadBrain != null)
			{
				MapComponent_Ability orCreate = MapComponent_Ability.GetOrCreate();
				foreach (Pawn current in squadBrain.ownedPawns)
				{
					if (current.def.defName == "Ant_Princess" || current.def.defName == "Ant_Queen")
					{
						flag = false;
						bool result = flag;
						return result;
					}
					Saveable_Caster saveable_Caster;
					if (orCreate.TryGetPawnHability(current, out saveable_Caster) && saveable_Caster.currentAbility != null && saveable_Caster.currentAbility.defName == "AlienEvolveToPrincessAbility")
					{
						flag = false;
						bool result = flag;
						return result;
					}
				}
				flag = true;
				return flag;
			}
			flag = false;
			return flag;
		}
	}
}
