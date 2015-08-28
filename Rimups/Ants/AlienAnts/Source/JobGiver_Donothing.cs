using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace AlienAnts
{
    public class JobGiver_DoNothing : ThinkNode_JobGiver
    {
        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            return new Job(DefDatabase<JobDef>.GetNamed("DoNothing"));
        }
    }
}
