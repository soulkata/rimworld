using System;

namespace AbilityPack
{
	public class AbilityRequeriment_TimeChangind : AbilityRequeriment
	{
		public int initial;

		public int between;

		public int minimum;

		public int maximum;

		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			Saveable_ExecutionLog log = pawn.GetLog(ability);
			int num = this.initial + log.numberOfExecution * this.between;
			if (this.minimum > 0)
			{
				num = Math.Max(this.minimum, num);
			}
			if (this.maximum > 0)
			{
				num = Math.Min(this.maximum, num);
			}
			return log.ticksSinceExecution >= num;
		}
	}
}
