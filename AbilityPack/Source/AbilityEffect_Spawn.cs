using RimWorld.SquadAI;
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
            List<Pawn> newPawns = new List<Pawn>();
            foreach (PawnKindDef pawnKind in this.pawnKinds)
            {
                Pawn newPawn = PawnGenerator.GeneratePawn(pawnKind, pawn.pawn.Faction);
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(pawn.pawn.Position, this.range);
                GenSpawn.Spawn(newPawn, loc);
                newPawns.Add(newPawn);

                if (this.spawnedMote != null)
                    this.spawnedMote.AbilityStarted(pawn, newPawn);
            }

            Brain brain = pawn.pawn.GetSquadBrain();
            if (brain == null)
            {
                StateGraph squadBrainStateGraph = GraphMaker.AssaultColonyGraph(pawn.pawn.Faction, false, false);
                newPawns.Insert(0, pawn.pawn);
                BrainMaker.MakeNewBrain(pawn.pawn.Faction, squadBrainStateGraph, newPawns);
            }
            else
                foreach (Pawn newPawn in newPawns)
                    brain.AddPawn(newPawn);
        }
    }
}
