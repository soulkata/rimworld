using System;
using System.Collections.Generic;

namespace AbilityPack
{
	public class AbilityRequeriment_All : AbilityRequeriment
	{
		public List<AbilityRequeriment> items;

		public override bool Sucess(AbilityDef ability, Saveable_Caster pawn)
		{
			if (this.items == null)
			{
				return false;
			}
			foreach (AbilityRequeriment current in this.items)
			{
				if (!current.Sucess(ability, pawn))
				{
					return false;
				}
			}
			return true;
		}
	}
}
