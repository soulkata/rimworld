using System;
using System.Collections.Generic;
using Verse.AI;

namespace AlienAnts
{
	public class JobDriver_DoNothing : JobDriver
	{
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_General.Wait(1000);
			yield break;
		}
	}
}
