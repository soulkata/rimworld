using Verse;
using Verse.AI;

namespace AbilityPack
{
    public class ThinkNode_StartAbility : ThinkNode
    {
        public override ThinkResult TryIssueJobPackage(Pawn pawn)
        {
            Saveable_Caster cachePawn = MapComponent_Ability.GetOrCreate().GetPawnHabilty(pawn);
            if (cachePawn.whaitingForThinkNode)
            {
                cachePawn.whaitingForThinkNode = false;
                Saveable_ExecutionLog log = cachePawn.GetLog(cachePawn.currentAbility);
                log.numberOfExecution++;
                log.ticksSinceExecution = 0;
                return new ThinkResult(new Job(DefDatabase<JobDef>.GetNamed(MapComponent_Ability.JobDefName)), this);
            }
            return ThinkResult.NoJob;
        }
    }
}
