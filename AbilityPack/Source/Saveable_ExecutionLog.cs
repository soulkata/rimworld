using Verse;

namespace AbilityPack
{
    public class Saveable_ExecutionLog : IExposable
    {
        public AbilityDef ability;
        public int numberOfExecution;
        public int ticksSinceExecution;
        public bool isValid;

        public void ExposeData()
        {
            Scribe_Defs.LookDef(ref this.ability, "ability");
            Scribe_Values.LookValue(ref this.numberOfExecution, "number");
            Scribe_Values.LookValue(ref this.ticksSinceExecution, "ticks");
            Scribe_Values.LookValue(ref this.isValid, "isValid");
        }
    }
}
