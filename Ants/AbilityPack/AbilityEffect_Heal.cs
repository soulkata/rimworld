using RimWorld.SquadAI;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
	public class AbilityEffect_Heal : AbilityEffect_Cast
	{
		public int treatLocalInjuryPowerUse;

		public float healLocalInjuryPowerUse;

		public float totalPower;

		public float targetPower;

		public float healthTrashHold;

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
			targets = (from i in targets.OfType<Pawn>()
			where i.MaxHitPoints > 0
			where i.health.summaryHealth.SummaryHealthPercent < this.healthTrashHold
			select i).Cast<Thing>().ToList<Thing>();
			return targets.Any<Thing>();
		}

		public override void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
		{
			float num = this.totalPower;
			foreach (Pawn current in targets.OfType<Pawn>())
			{
				float num2 = this.targetPower;
				if (num2 + num >= (float)this.treatLocalInjuryPowerUse)
				{
					foreach (Hediff_Injury current2 in from i in current.health.hediffSet.GetInjuriesTreatable()
					orderby i.Severity descending
					select i)
					{
						num2 -= (float)this.treatLocalInjuryPowerUse;
                        HediffComp_Tendable hediffComp_Treatable = HediffUtility.TryGetComp<HediffComp_Tendable>(current2);
						if (hediffComp_Treatable == null)
						{
							Log.Error("Tried to treat " + current2 + " which does not have a HediffComp_Treatable");
						}
						else
						{
							hediffComp_Treatable.CompTreated(0.3f, 1);
						}
						Brain squadBrain = BrainUtility.GetSquadBrain(caster.pawn);
						if (squadBrain != null && !current.Downed && BrainUtility.GetSquadBrain(current) == null)
						{
							squadBrain.AddPawn(current);
						}
						if (num2 < 0f)
						{
							num += num2;
							num2 = 0f;
						}
						if (num2 + num < (float)this.treatLocalInjuryPowerUse)
						{
							break;
						}
					}
				}
				if (num2 + num >= this.healLocalInjuryPowerUse)
				{
					foreach (Hediff_Injury current3 in from i in current.health.hediffSet.hediffs.OfType<Hediff_Injury>()
					where HediffUtility.IsTendedAndHealing(i) && i.Severity != 0f
					orderby i.Severity descending
					select i)
					{
						float num3 = (num2 + num) / this.healLocalInjuryPowerUse;
						num3 = Math.Min(num3, current3.Severity);
						current3.DirectHeal(num3);
						num2 -= num3 * this.healLocalInjuryPowerUse;
						if (num2 < 0f)
						{
							num += num2;
							num2 = 0f;
						}
						if (num2 + num < this.healLocalInjuryPowerUse)
						{
							break;
						}
					}
				}
			}
		}
	}
}
