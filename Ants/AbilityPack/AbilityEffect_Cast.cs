using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AbilityPack
{
	public abstract class AbilityEffect_Cast : AbilityEffect
	{
		private class JobDriverHolder
		{
			public AbilityEffect_Cast owner;

			public JobDriver_AbilityEffect jobDriver;

			public IEnumerable<Thing> targets;

			public IExposable effectState;

			public Saveable_Caster caster;

			public JobDriverHolder(AbilityEffect_Cast owner, JobDriver_AbilityEffect jobDriver, Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
			{
				this.owner = owner;
				this.jobDriver = jobDriver;
				this.effectState = effectState;
				this.caster = caster;
				this.targets = targets;
			}

			public void OnStartCast()
			{
				this.jobDriver.pawn.pather.StopDead();
				this.owner.StartCast(this.caster, this.targets);
			}

			public void OnDamageTaken(DamageInfo damage)
			{
				if (damage.Def.canInterruptJobs)
				{
					this.jobDriver.EndJobWith(JobCondition.InterruptOptional);
					this.caster.NotifyCompleted(false);
				}
			}

			public void OnFinishCast()
			{
				this.owner.OnSucessfullCast(this.caster, this.targets, this.effectState);
				this.caster.NotifyCompleted(true);
			}
		}

		public bool canBeInterrupted;

		public int castTime;

		public float manaCost;

		public AbilityMote casterMote;

		public AbilityMote targetMote;

		public override bool TryStart(AbilityDef ability, Saveable_Caster caster, ref List<Thing> target, ref IExposable effectState)
		{
			return caster.manaValue >= this.manaCost;
		}

		public override IEnumerable<Toil> MakeNewToils(JobDriver_AbilityEffect jobDriver, Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
		{
			AbilityEffect_Cast.JobDriverHolder @object = new AbilityEffect_Cast.JobDriverHolder(this, jobDriver, caster, targets, effectState);
			Toil toil = new Toil();
			if (this.canBeInterrupted)
			{
				toil.initAction = new Action(@object.OnStartCast);
			}
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.defaultDuration = this.castTime;
			yield return toil;
			toil = new Toil();
			toil.initAction = new Action(@object.OnFinishCast);
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return toil;
			yield break;
		}

		public override void ExecuteWhileIncapacitated(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
		{
			this.StartCast(caster, targets);
			this.OnSucessfullCast(caster, targets, effectState);
			caster.NotifyCompleted(true);
		}

		public void StartCast(Saveable_Caster caster, IEnumerable<Thing> targets)
		{
			caster.manaValue -= this.manaCost;
			if (this.casterMote != null)
			{
				this.casterMote.AbilityStarted(caster, caster.pawn);
			}
			if (this.targetMote != null)
			{
				foreach (Thing current in targets)
				{
					this.targetMote.AbilityStarted(caster, current);
				}
			}
		}

		public abstract void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState);
	}
}
