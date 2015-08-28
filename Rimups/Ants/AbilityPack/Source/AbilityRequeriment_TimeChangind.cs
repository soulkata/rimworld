using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

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
            int ticks = this.initial + log.numberOfExecution * this.between;
            if (this.minimum > 0)
                ticks = Math.Max(this.minimum, ticks);
            if (this.maximum > 0)
                ticks = Math.Min(this.maximum, ticks);
            return log.ticksSinceExecution >= ticks;
        }
    }
}
