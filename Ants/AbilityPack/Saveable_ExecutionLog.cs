using System;
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
			Scribe_Defs.LookDef<AbilityDef>(ref this.ability, "ability");
			Scribe_Values.LookValue<int>(ref this.numberOfExecution, "number", 0, false);
			Scribe_Values.LookValue<int>(ref this.ticksSinceExecution, "ticks", 0, false);
			Scribe_Values.LookValue<bool>(ref this.isValid, "isValid", false, false);
		}
	}
}
