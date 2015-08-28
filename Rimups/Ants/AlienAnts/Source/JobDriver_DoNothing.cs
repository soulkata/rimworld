using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AlienAnts
{
    public class JobDriver_DoNothing : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_General.Wait(1000);
        }
    }
}
