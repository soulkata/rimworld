using RimWorld.SquadAI;
using System;
using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
	public class AbilityEffect_Spawn : AbilityEffect_Cast
	{
		public List<PawnKindDef> pawnKinds;

		public AbilityMote spawnedMote;

		public int range;

		public override void OnSucessfullCast(Saveable_Caster pawn, IEnumerable<Thing> targets, IExposable effectState)
		{
			List<Pawn> list = new List<Pawn>();
			foreach (PawnKindDef current in this.pawnKinds)
			{
				Pawn pawn2 = PawnGenerator.GeneratePawn(current, pawn.pawn.Faction, false);
				IntVec3 intVec = CellFinder.RandomClosewalkCellNear(pawn.pawn.Position, this.range);
				GenSpawn.Spawn(pawn2, intVec);
				list.Add(pawn2);
				if (this.spawnedMote != null)
				{
					this.spawnedMote.AbilityStarted(pawn, pawn2);
				}
			}
			Brain squadBrain = BrainUtility.GetSquadBrain(pawn.pawn);
			if (squadBrain == null)
			{
				StateGraph stateGraph = GraphMaker.AssaultColonyGraph(pawn.pawn.Faction, false, false, false);
				list.Insert(0, pawn.pawn);
				BrainMaker.MakeNewBrain(pawn.pawn.Faction, stateGraph, list);
				return;
			}
			foreach (Pawn current2 in list)
			{
				squadBrain.AddPawn(current2);
			}
		}
	}
}
