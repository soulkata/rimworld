using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AbilityPack
{
    public class MapComponent_Ability : MapComponent
    {
        public const string JobDefName = "AbilityEffect_JobDef";
        public const int longTickOffet = 64;

        private AbilityDef[] emptyAbilities = new AbilityDef[0];
        private Dictionary<ThingDef, List<AbilityDef>> abilityCache;
        private Dictionary<ThingDef, ManaDef> manaCache;
        internal List<Saveable_Mote> completingMotes = new List<Saveable_Mote>();

        private List<Saveable_Caster> pawnCache = new List<Saveable_Caster>();

        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                foreach (Saveable_Caster value in this.pawnCache.ToArray())
                {
                    Pawn pawn = value.pawn;

                    if ((pawn == null) ||
                        (pawn.Destroyed))
                    {
                        this.RemovePawn(value);
                        continue;
                    }

                    if (pawn.Dead)
                    {
                        if (value.corpse == null)
                        {
                            List<Thing> things = Find.ListerThings.ThingsMatching(new ThingRequest() { group = ThingRequestGroup.Corpse });
                            if (things == null)
                            {
                                this.RemovePawn(value);
                                continue;
                            }

                            foreach (Corpse c in things)
                            {
                                if (c.innerPawn == pawn)
                                {
                                    value.corpse = c;
                                    break;
                                }
                            }

                            if (value.corpse == null)
                            {
                                this.RemovePawn(value);
                                continue;
                            }
                        }

                        if (value.corpse.Destroyed)
                        {
                            this.RemovePawn(value);
                            continue;
                        }
                    }
                }
            }

            base.ExposeData();

            Scribe_Collections.LookList(ref this.pawnCache, "pawns", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (Saveable_Caster value in this.pawnCache.ToArray())
                {
                    Pawn pawn = value.pawn;

                    if (pawn == null)
                        this.RemovePawn(value);
                    else
                        this.completingMotes.AddRange(value.currentMotes);
                }
            }
        }

        public static MapComponent_Ability GetOrCreate()
        {            
            MapComponent_Ability ret = Find.Map.components.OfType<MapComponent_Ability>().FirstOrDefault();
            if (ret == null)
            {
                ret = new MapComponent_Ability();
                Find.Map.components.Add(ret);
            }

            return ret;
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (this.pawnCache == null)
                this.pawnCache = new List<Saveable_Caster>();

            if (Find.TickManager.TicksGame % MapComponent_Ability.longTickOffet == 0)
            {
                foreach (Saveable_Caster value in this.pawnCache.ToArray())
                {
                    try
                    {
                        Pawn pawn = value.pawn;

                        if (pawn == null)
                        {
                            this.RemovePawn(value);
                            continue;
                        }

                        if (pawn.Dead)
                        {
                            if (value.corpse == null)
                            {
                                List<Thing> things = Find.ListerThings.ThingsMatching(new ThingRequest() { group = ThingRequestGroup.Corpse });
                                if (things == null)
                                {
                                    this.RemovePawn(value);
                                    continue;
                                }

                                foreach (Corpse c in things)
                                {
                                    if (c.innerPawn == pawn)
                                    {
                                        value.corpse = c;
                                        this.InjectTab(value.corpse.def);
                                        break;
                                    }
                                }

                                if (value.corpse == null)
                                {
                                    this.RemovePawn(value);
                                    continue;
                                }
                            }

                            if (value.corpse.Destroyed)
                            {
                                this.RemovePawn(value);
                                continue;
                            }
                        }
                        else
                        {
                            this.InjectTab(value.pawn.def);
                            value.corpse = null;
                        }
                        this.NormalTick(value);

                        if ((pawn.Dead) ||
                            (pawn.Downed))
                        {
                            AbilityDef toStartAbility;
                            List<Thing> targets;
                            Saveable state;
                            if (this.TryStartNextAbility(value, out toStartAbility, out targets, out state))
                            {
                                Saveable_ExecutionLog log = value.GetLog(toStartAbility);
                                log.numberOfExecution++;
                                log.ticksSinceExecution = 0;
                                toStartAbility.effect.ExecuteWhileIncapacitated(value, targets, state);
                            }
                        }
                        else
                            if ((pawn.CurJob == null) ||
                                (pawn.CurJob.def.defName != MapComponent_Ability.JobDefName))
                            {
                                AbilityDef toStartAbility;
                                List<Thing> targets;
                                Saveable state;
                                if (this.TryStartNextAbility(value, out toStartAbility, out targets, out state))
                                {
                                    value.whaitingForThinkNode = true;
                                    value.currentAbility = toStartAbility;
                                    value.currentTargets = targets == null ? null : targets.Select(i => new Saveable_Target() { target = i }).ToList();
                                    value.effectState = state;
                                    pawn.jobs.EndCurrentJob(JobCondition.OptionalInterrupt);
                                }
                            }
                    }
                    catch (Exception e)
                    {
                        Log.Notify_Exception(e);
                    }
                }                
            }
            else
            {
                foreach (Saveable_Caster value in this.pawnCache)
                {
                    try
                    {
                        this.NormalTick(value);
                    }
                    catch (Exception e)
                    {
                        Log.Notify_Exception(e);
                    }
                }
            }

            int indexMote = 0;
            while (indexMote < this.completingMotes.Count)
            {
                if (this.completingMotes[indexMote].Tick())
                    indexMote++;
                else
                {
                    this.completingMotes.RemoveAt(indexMote);
                }
            }
        }

        private void InjectTab(ThingDef thingDef)
        {
            if (thingDef.inspectorTabsResolved == null)
            {
                thingDef.inspectorTabsResolved = new List<ITab>();
                foreach (Type current in thingDef.inspectorTabs)
                    thingDef.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(current));
            }

            if (!thingDef.inspectorTabsResolved.OfType<ITab_Pawn_Ability>().Any())
                thingDef.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(typeof(ITab_Pawn_Ability)));
        }

        private void NormalTick(Saveable_Caster value)
        {
            Pawn pawn = value.pawn;

            if (value.manaDef != null)
            {
                value.manaValue += value.manaDef.replenish.Replenish(value);
                value.manaValue = Mathf.Clamp(value.manaValue, 0, 100);
            }

            AbilityDef executingAbility;
            if ((pawn.CurJob != null) &&
                (pawn.CurJob.def.defName == MapComponent_Ability.JobDefName))
                executingAbility = value.currentAbility;
            else
                executingAbility = null;

            foreach (Saveable_ExecutionLog log in value.executionLogs)
            {
                if (log.ability == executingAbility)
                    log.ticksSinceExecution = 0;
                else
                    if (log.isValid)
                        log.ticksSinceExecution++;
            }            
        }

        public Saveable_Caster GetPawnHabilty(Pawn pawn)
        {
            Saveable_Caster value;

            if (!this.TryGetPawnHability(pawn, out value))
            {
                if (this.manaCache == null)
                {
                    this.manaCache = new Dictionary<ThingDef, ManaDef>();
                    foreach (ManaDef ability in DefDatabase<ManaDef>.AllDefs)
                    {
                        foreach (ThingDef race in ability.races)
                            this.manaCache.Add(race, ability);
                    }
                }


                value = new Saveable_Caster();
                value.pawn = pawn;
                this.pawnCache.Add(value);
                this.InjectTab(pawn.def);

                if (this.manaCache.TryGetValue(pawn.def, out value.manaDef))
                    value.manaValue = value.manaDef.initial;

                foreach (AbilityDef ability in this.GetAllowedAbilities(value.pawn))
                    value.GetLog(ability);
            }

            return value;
        }

        internal void ReplacePawnAbility(Saveable_Caster oldSavedPawn, Pawn newPawn)
        {
            this.pawnCache.Remove(oldSavedPawn);

            Saveable_Caster newSavedPawn = this.GetPawnHabilty(newPawn);
            foreach (Saveable_ExecutionLog oldLog in oldSavedPawn.executionLogs)
            {
                bool equals = false;
                foreach (Saveable_ExecutionLog newLog in newSavedPawn.executionLogs)
                {
                    if (oldLog.ability == newLog.ability)
                    {
                        equals = true;
                        newLog.numberOfExecution = oldLog.numberOfExecution;
                        newLog.ticksSinceExecution = oldLog.ticksSinceExecution;
                        break;
                    }
                }

                if (!equals)
                    newSavedPawn.executionLogs.Add(oldLog);
            }
        }

        private void RemovePawn(Saveable_Caster savePawn)
        {
            this.pawnCache.Remove(savePawn); 
        }

        public bool TryGetPawnHability(Pawn pawn, out Saveable_Caster value)
        {
            foreach (Saveable_Caster caster in this.pawnCache)
            {
                if (caster.pawn == pawn)
                {
                    value = caster;
                    return true;
                }
            }

            value = null;
            return false;
        }

        public IEnumerable<AbilityDef> GetAllowedAbilities(Pawn pawn)
        {
            List<AbilityDef> raceAbilities;

            if (this.abilityCache == null)
            {
                this.abilityCache = new Dictionary<ThingDef, List<AbilityDef>>();

                foreach (AbilityDef ability in DefDatabase<AbilityDef>.AllDefs)
                {
                    foreach (ThingDef race in ability.races)
                    {
                        if (!this.abilityCache.TryGetValue(race, out raceAbilities))
                        {
                            raceAbilities = new List<AbilityDef>();
                            this.abilityCache.Add(race, raceAbilities);
                        }
                        raceAbilities.Add(ability);
                    }
                }
            }

            if (this.abilityCache.TryGetValue(pawn.def, out raceAbilities))
                return raceAbilities;
            else
                return this.emptyAbilities;
        }

        private bool TryStartNextAbility(Saveable_Caster value, out AbilityDef ability, out List<Thing> targets, out Saveable effectState)
        {
            List<KeyValuePair<int, AbilityDef>> priorities = new List<KeyValuePair<int, AbilityDef>>();
            foreach (Saveable_ExecutionLog log in value.executionLogs)
            {
                if ((!log.ability.races.Contains(value.pawn.def)) ||
                    (log.ability.validity == null) ||
                    (!log.ability.validity.Sucess(log.ability, value)))
                {
                    log.isValid = false;
                    continue;
                }

                log.isValid = true;

                if ((log.ability.requeriment != null) &&
                    (!log.ability.requeriment.Sucess(log.ability, value)))
                    continue;

                priorities.Add(new KeyValuePair<int, AbilityDef>(log.ability.priority.GetPriority(log.ability, value.pawn), log.ability));
            }

            if (priorities.Any())
            {
                priorities.Sort((x, y) => y.Key.CompareTo(x.Key));

                foreach (KeyValuePair<int, AbilityDef> itm in priorities)
                {
                    effectState = null;
                    targets = null;
                    if (itm.Value.target != null)
                        targets = itm.Value.target.Targets(itm.Value, value).ToList();

                    if (itm.Value.effect.TryStart(itm.Value, value, ref targets, ref effectState))
                    {
                        ability = itm.Value;
                        return true;
                    }
                }
            }

            ability = null;
            effectState = null;
            targets = null;
            return false;
        }        
    }
}
