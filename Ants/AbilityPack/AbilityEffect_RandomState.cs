using System;
using Verse;

namespace AbilityPack
{
	public class AbilityEffect_RandomState : IExposable
	{
		public int item;

		public IExposable effectState;

		public void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.item, "item", 0, false);
			Scribe_Deep.LookDeep<IExposable>(ref this.effectState, "effectState", new object[0]);
		}
	}
}
