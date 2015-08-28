using System;
using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
	public class AbilityTarget_Range : AbilityTarget
	{
		public float minimumRange;

		public float maximumRange;

		public bool closestFirst;

		public AbilityTarget target;

		public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster saveablePawn)
		{
			float num = this.maximumRange * this.maximumRange;
			float num2 = this.minimumRange * this.minimumRange;
			if (this.closestFirst)
			{
				List<KeyValuePair<float, Thing>> list = new List<KeyValuePair<float, Thing>>();
				foreach (Thing current in this.target.Targets(ability, saveablePawn))
				{
					float lengthHorizontalSquared = (saveablePawn.pawn.Position - current.Position).LengthHorizontalSquared;
					if ((num <= 0f || num >= lengthHorizontalSquared) && (num2 <= 0f || num2 <= lengthHorizontalSquared))
					{
						list.Add(new KeyValuePair<float, Thing>(lengthHorizontalSquared, current));
					}
				}
				list.Sort((KeyValuePair<float, Thing> x, KeyValuePair<float, Thing> y) => x.Key.CompareTo(y.Key));
				foreach (KeyValuePair<float, Thing> current2 in list)
				{
					KeyValuePair<float, Thing> keyValuePair = current2;
					yield return keyValuePair.Value;
				}
			}
			else
			{
				foreach (Thing current3 in this.target.Targets(ability, saveablePawn))
				{
					float lengthHorizontalSquared2 = (saveablePawn.pawn.Position - current3.Position).LengthHorizontalSquared;
					if ((num <= 0f || num >= lengthHorizontalSquared2) && (num2 <= 0f || num2 <= lengthHorizontalSquared2))
					{
						yield return current3;
					}
				}
			}
			yield break;
		}
	}
}
