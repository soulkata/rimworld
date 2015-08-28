using System;
using UnityEngine;
using Verse;

namespace AbilityPack
{
	public class AbilityMote_Default : AbilityMote
	{
		public Color color = Color.white;

		public override void AbilityStarted(Saveable_Caster caster, Thing target)
		{
			caster.AddMote(new Saveable_Mote_Default
			{
				target = target,
				currentColor = this.color,
				alphaChange = 0f,
				exactAlpha = 0.5f,
				exactPosition = target.DrawPos,
				exactScale = new Vector3(1f, 1f, 1f)
			});
		}
	}
}
