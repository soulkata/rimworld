using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
	public class AbilityEffect_Workaround_MakeCorpseProvideCover : AbilityEffect_Cast
	{
		public override bool TryStart(AbilityDef ability, Saveable_Caster caster, ref List<Thing> targets, ref IExposable effectState)
		{
			if (!base.TryStart(ability, caster, ref targets, ref effectState))
			{
				return false;
			}
			if (targets == null)
			{
				return false;
			}
			List<Thing> list = AbilityEffect_Revive.SelectCorpses(targets);
			if (list.Any<Thing>())
			{
				targets = list;
				return true;
			}
			return false;
		}

		public override void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
		{
			using (IEnumerator<Thing> enumerator = targets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Corpse corpse = (Corpse)enumerator.Current;
					corpse.def.fillPercent = 0.75f;
					Find.CoverGrid.Register(corpse);
				}
			}
		}
	}
}
