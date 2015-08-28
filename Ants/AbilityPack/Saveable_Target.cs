using System;
using Verse;

namespace AbilityPack
{
	public class Saveable_Target : IExposable
	{
		public Thing target;

		public void ExposeData()
		{
			Scribe_References.LookReference<Thing>(ref this.target, "target");
		}
	}
}
