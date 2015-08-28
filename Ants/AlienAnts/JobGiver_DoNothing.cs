using System;
using Verse;
using Verse.AI;

namespace AlienAnts
{
	public class JobGiver_DoNothing : ThinkNode_JobGiver
	{
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			return new Job(DefDatabase<JobDef>.GetNamed("DoNothing", true));
		}
	}
}
