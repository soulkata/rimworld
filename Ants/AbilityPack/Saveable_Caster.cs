using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
	public class Saveable_Caster : IExposable
	{
		public Corpse corpse;

		public bool corpseTabInjected;

		public Pawn pawn;

		public ManaDef manaDef;

		public float manaValue;

		public bool whaitingForThinkNode;

		public AbilityDef currentAbility;

		public IExposable effectState;

		public List<Saveable_Target> currentTargets;

		public List<Saveable_Mote> currentMotes = new List<Saveable_Mote>();

		public List<Saveable_ExecutionLog> executionLogs = new List<Saveable_ExecutionLog>();

		public IEnumerable<Thing> Targets
		{
			get
			{
				return from i in this.currentTargets
				select i.target into i
				where i != null
				select i;
			}
		}

		public Saveable_ExecutionLog GetLog(AbilityDef ability)
		{
			foreach (Saveable_ExecutionLog current in this.executionLogs)
			{
				if (current.ability == ability)
				{
					return current;
				}
			}
			Saveable_ExecutionLog saveable_ExecutionLog = new Saveable_ExecutionLog();
			saveable_ExecutionLog.ability = ability;
			this.executionLogs.Add(saveable_ExecutionLog);
			return saveable_ExecutionLog;
		}

		public void ExposeData()
		{
			Scribe_References.LookReference<Pawn>(ref this.pawn, "thing");
			Scribe_Defs.LookDef<ManaDef>(ref this.manaDef, "manaDef");
			Scribe_Values.LookValue<float>(ref this.manaValue, "manaValue", 0f, false);
			Scribe_Values.LookValue<bool>(ref this.whaitingForThinkNode, "whaitingForThinkNode", false, false);
			Scribe_Defs.LookDef<AbilityDef>(ref this.currentAbility, "abilityDef");
			Scribe_Deep.LookDeep<IExposable>(ref this.effectState, "effectState", new object[0]);
			Scribe_Collections.LookList<Saveable_Target>(ref this.currentTargets, "targets", LookMode.Deep, new object[0]);
			Scribe_Collections.LookList<Saveable_ExecutionLog>(ref this.executionLogs, "executionLogs", LookMode.Deep, new object[0]);
			Scribe_Collections.LookList<Saveable_Mote>(ref this.currentMotes, "currentMotes", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.currentTargets != null)
			{
				Saveable_Target[] array = this.currentTargets.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					Saveable_Target saveable_Target = array[i];
					if (saveable_Target.target == null)
					{
						this.currentTargets.Remove(saveable_Target);
					}
				}
			}
		}

		public void NotifyCompleted(bool sucess)
		{
			foreach (Saveable_Mote current in this.currentMotes)
			{
				current.Completed(this, sucess);
			}
			this.currentMotes.Clear();
			this.currentAbility = null;
			this.currentTargets = null;
			this.effectState = null;
		}

		public void AddMote(Saveable_Mote mote)
		{
			mote.InitializeMote(ref mote.mote);
			this.currentMotes.Add(mote);
			MapComponent_Ability.GetOrCreate().completingMotes.Add(mote);
		}
	}
}
