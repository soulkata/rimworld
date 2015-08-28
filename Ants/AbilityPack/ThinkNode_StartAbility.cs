using System;
using Verse;
using Verse.AI;

namespace AbilityPack
{
	public class ThinkNode_StartAbility : ThinkNode
	{
		public override ThinkResult TryIssueJobPackage(Pawn pawn)
		{
			Saveable_Caster pawnHabilty = MapComponent_Ability.GetOrCreate().GetPawnHabilty(pawn);
			if (pawnHabilty.whaitingForThinkNode)
			{
				pawnHabilty.whaitingForThinkNode = false;
				Saveable_ExecutionLog log = pawnHabilty.GetLog(pawnHabilty.currentAbility);
				log.numberOfExecution++;
				log.ticksSinceExecution = 0;
				return new ThinkResult(new Job(DefDatabase<JobDef>.GetNamed("AbilityEffect_JobDef", true)), this);
			}
			return ThinkResult.NoJob;
		}
	}
}
