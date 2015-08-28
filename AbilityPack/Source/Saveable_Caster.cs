using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
    public class Saveable_Caster : Saveable
    {
        public Corpse corpse = null;
        public bool corpseTabInjected = false;

        public Pawn pawn;

        public ManaDef manaDef;
        public float manaValue;

        public bool whaitingForThinkNode;
        public AbilityDef currentAbility;
        public Saveable effectState;
        public List<Saveable_Target> currentTargets;
        public List<Saveable_Mote> currentMotes = new List<Saveable_Mote>();        
        public List<Saveable_ExecutionLog> executionLogs = new List<Saveable_ExecutionLog>();        
        
        public Saveable_ExecutionLog GetLog(AbilityDef ability)
        {
            foreach (Saveable_ExecutionLog l in this.executionLogs)
            {
                if (l.ability == ability)
                    return l;
            }

            Saveable_ExecutionLog newLog = new Saveable_ExecutionLog();
            newLog.ability = ability;
            this.executionLogs.Add(newLog);
            return newLog;
        }

        public void ExposeData()
        {
            Scribe_References.LookReference(ref this.pawn, "thing");
            Scribe_Defs.LookDef(ref this.manaDef, "manaDef");
            Scribe_Values.LookValue(ref this.manaValue, "manaValue");
            Scribe_Values.LookValue(ref this.whaitingForThinkNode, "whaitingForThinkNode");
            Scribe_Defs.LookDef(ref this.currentAbility, "abilityDef");
            Scribe_Deep.LookDeep(ref this.effectState, "effectState");
            Scribe_Collections.LookList(ref this.currentTargets, "targets", LookMode.Deep);
            Scribe_Collections.LookList(ref this.executionLogs, "executionLogs", LookMode.Deep);
            Scribe_Collections.LookList(ref this.currentMotes, "currentMotes", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)                
            {                
                if (this.currentTargets != null)
                {
                    foreach (Saveable_Target value in this.currentTargets.ToArray())
                    {
                        if (value.target == null)
                            this.currentTargets.Remove(value);
                    }
                }
            }
        }

        public void NotifyCompleted(bool sucess)
        {
            foreach (Saveable_Mote mote in this.currentMotes)
                mote.Completed(this, sucess);
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

        public IEnumerable<Thing> Targets 
        {
            get
            {
                return this.currentTargets.Select(i => i.target).Where(i => i != null); 
            }
        }
    }

    public class Saveable_Target : Saveable
    {
        public Thing target;

        public void ExposeData()
        {
            Scribe_References.LookReference(ref this.target, "target");
        }
    }
}
