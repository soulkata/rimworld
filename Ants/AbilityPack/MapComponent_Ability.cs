using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

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
				Saveable_Caster[] array = this.pawnCache.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					Saveable_Caster saveable_Caster = array[i];
					Pawn pawn = saveable_Caster.pawn;
					if (pawn == null || pawn.Destroyed)
					{
						this.RemovePawn(saveable_Caster);
					}
					else if (pawn.Dead)
					{
						if (saveable_Caster.corpse == null)
						{
							List<Thing> list = Find.ListerThings.ThingsMatching(new ThingRequest
							{
								group = ThingRequestGroup.Corpse
							});
							if (list == null)
							{
								this.RemovePawn(saveable_Caster);
								goto IL_E7;
							}
							using (List<Thing>.Enumerator enumerator = list.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									Corpse corpse = (Corpse)enumerator.Current;
									if (corpse.innerPawn == pawn)
									{
										saveable_Caster.corpse = corpse;
										break;
									}
								}
							}
							if (saveable_Caster.corpse == null)
							{
								this.RemovePawn(saveable_Caster);
								goto IL_E7;
							}
						}
						if (saveable_Caster.corpse.Destroyed)
						{
							this.RemovePawn(saveable_Caster);
						}
					}
					IL_E7:;
				}
			}
			base.ExposeData();
			Scribe_Collections.LookList<Saveable_Caster>(ref this.pawnCache, "pawns", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				Saveable_Caster[] array2 = this.pawnCache.ToArray();
				for (int j = 0; j < array2.Length; j++)
				{
					Saveable_Caster saveable_Caster2 = array2[j];
					if (saveable_Caster2.pawn == null)
					{
						this.RemovePawn(saveable_Caster2);
					}
					else
					{
						this.completingMotes.AddRange(saveable_Caster2.currentMotes);
					}
				}
			}
		}

		public static MapComponent_Ability GetOrCreate()
		{
			MapComponent_Ability mapComponent_Ability = Find.Map.components.OfType<MapComponent_Ability>().FirstOrDefault<MapComponent_Ability>();
			if (mapComponent_Ability == null)
			{
				mapComponent_Ability = new MapComponent_Ability();
				Find.Map.components.Add(mapComponent_Ability);
			}           
			return mapComponent_Ability;
		}

		public override void MapComponentTick()
		{
			base.MapComponentTick();
			if (this.pawnCache == null)
			{
				this.pawnCache = new List<Saveable_Caster>();
			}
			if (Find.TickManager.TicksGame % 64 == 0)
			{
				Saveable_Caster[] array = this.pawnCache.ToArray();
				for (int k = 0; k < array.Length; k++)
				{
					Saveable_Caster saveable_Caster = array[k];
					try
					{
						Pawn pawn = saveable_Caster.pawn;
						if (pawn == null)
						{
							this.RemovePawn(saveable_Caster);
						}
						else
						{
							if (pawn.Dead)
							{
								if (saveable_Caster.corpse == null)
								{
									List<Thing> list = Find.ListerThings.ThingsMatching(new ThingRequest
									{
										group = ThingRequestGroup.Corpse
									});
									if (list == null)
									{
										this.RemovePawn(saveable_Caster);
										goto IL_22F;
									}
									using (List<Thing>.Enumerator enumerator = list.GetEnumerator())
									{
										while (enumerator.MoveNext())
										{
											Corpse corpse = (Corpse)enumerator.Current;
											if (corpse.innerPawn == pawn)
											{
												saveable_Caster.corpse = corpse;
												this.InjectTab(saveable_Caster.corpse.def);
												break;
											}
										}
									}
									if (saveable_Caster.corpse == null)
									{
										this.RemovePawn(saveable_Caster);
										goto IL_22F;
									}
								}
								if (saveable_Caster.corpse.Destroyed)
								{
									this.RemovePawn(saveable_Caster);
									goto IL_22F;
								}
							}
							else
							{
								this.InjectTab(saveable_Caster.pawn.def);
								saveable_Caster.corpse = null;
							}
							this.NormalTick(saveable_Caster);
							AbilityDef currentAbility;
							List<Thing> list2;
							IExposable effectState2;
							if (pawn.Dead || pawn.Downed)
							{
								AbilityDef abilityDef;
								List<Thing> targets;
								IExposable effectState;
								if (this.TryStartNextAbility(saveable_Caster, out abilityDef, out targets, out effectState))
								{
									Saveable_ExecutionLog log = saveable_Caster.GetLog(abilityDef);
									log.numberOfExecution++;
									log.ticksSinceExecution = 0;
									abilityDef.effect.ExecuteWhileIncapacitated(saveable_Caster, targets, effectState);
								}
							}
							else if ((pawn.CurJob == null || pawn.CurJob.def.defName != "AbilityEffect_JobDef") && this.TryStartNextAbility(saveable_Caster, out currentAbility, out list2, out effectState2))
							{
								saveable_Caster.whaitingForThinkNode = true;
								saveable_Caster.currentAbility = currentAbility;
								Saveable_Caster arg_209_0 = saveable_Caster;
								List<Saveable_Target> arg_209_1;
								if (list2 != null)
								{
									arg_209_1 = (from i in list2
									select new Saveable_Target
									{
										target = i
									}).ToList<Saveable_Target>();
								}
								else
								{
									arg_209_1 = null;
								}
								arg_209_0.currentTargets = arg_209_1;
								saveable_Caster.effectState = effectState2;
								pawn.jobs.EndCurrentJob(Verse.AI.JobCondition.InterruptOptional);
							}
						}
					}
					catch (Exception ex)
					{
						Log.Notify_Exception(ex);
					}
					IL_22F:;
				}
			}
			else
			{
				foreach (Saveable_Caster current in this.pawnCache)
				{
					try
					{
						this.NormalTick(current);
					}
					catch (Exception ex2)
					{
						Log.Notify_Exception(ex2);
					}
				}
			}
			int j = 0;
			while (j < this.completingMotes.Count)
			{
				if (this.completingMotes[j].Tick())
				{
					j++;
				}
				else
				{
					this.completingMotes.RemoveAt(j);
				}
			}
		}

		private void InjectTab(ThingDef thingDef)
		{
			if (thingDef.inspectorTabsResolved == null)
			{
				thingDef.inspectorTabsResolved = new List<ITab>();
				foreach (Type current in thingDef.inspectorTabs)
				{
					thingDef.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(current));
				}
			}
			if (!thingDef.inspectorTabsResolved.OfType<ITab_Pawn_Ability>().Any<ITab_Pawn_Ability>())
			{
				thingDef.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(typeof(ITab_Pawn_Ability)));
			}
		}

		private void NormalTick(Saveable_Caster value)
		{
			Pawn pawn = value.pawn;
			if (value.manaDef != null)
			{
				value.manaValue += value.manaDef.replenish.Replenish(value);
				value.manaValue = Mathf.Clamp(value.manaValue, 0f, 100f);
			}
			AbilityDef abilityDef;
			if (pawn.CurJob != null && pawn.CurJob.def.defName == "AbilityEffect_JobDef")
			{
				abilityDef = value.currentAbility;
			}
			else
			{
				abilityDef = null;
			}
			foreach (Saveable_ExecutionLog current in value.executionLogs)
			{
				if (current.ability == abilityDef)
				{
					current.ticksSinceExecution = 0;
				}
				else if (current.isValid)
				{
					current.ticksSinceExecution++;
				}
			}
		}

		public Saveable_Caster GetPawnHabilty(Pawn pawn)
		{
			Saveable_Caster saveable_Caster;
			if (!this.TryGetPawnHability(pawn, out saveable_Caster))
			{
				if (this.manaCache == null)
				{
					this.manaCache = new Dictionary<ThingDef, ManaDef>();
					foreach (ManaDef current in DefDatabase<ManaDef>.AllDefs)
					{
						foreach (ThingDef current2 in current.races)
						{
							this.manaCache.Add(current2, current);
						}
					}
				}
				saveable_Caster = new Saveable_Caster();
				saveable_Caster.pawn = pawn;
				this.pawnCache.Add(saveable_Caster);
				this.InjectTab(pawn.def);
				if (this.manaCache.TryGetValue(pawn.def, out saveable_Caster.manaDef))
				{
					saveable_Caster.manaValue = saveable_Caster.manaDef.initial;
				}
				foreach (AbilityDef current3 in this.GetAllowedAbilities(saveable_Caster.pawn))
				{
					saveable_Caster.GetLog(current3);
				}
			}
			return saveable_Caster;
		}

		internal void ReplacePawnAbility(Saveable_Caster oldSavedPawn, Pawn newPawn)
		{
			this.pawnCache.Remove(oldSavedPawn);
			Saveable_Caster pawnHabilty = this.GetPawnHabilty(newPawn);
			foreach (Saveable_ExecutionLog current in oldSavedPawn.executionLogs)
			{
				bool flag = false;
				foreach (Saveable_ExecutionLog current2 in pawnHabilty.executionLogs)
				{
					if (current.ability == current2.ability)
					{
						flag = true;
						current2.numberOfExecution = current.numberOfExecution;
						current2.ticksSinceExecution = current.ticksSinceExecution;
						break;
					}
				}
				if (!flag)
				{
					pawnHabilty.executionLogs.Add(current);
				}
			}
		}

		private void RemovePawn(Saveable_Caster savePawn)
		{
			this.pawnCache.Remove(savePawn);
		}

		public bool TryGetPawnHability(Pawn pawn, out Saveable_Caster value)
		{
			foreach (Saveable_Caster current in this.pawnCache)
			{
				if (current.pawn == pawn)
				{
					value = current;
					return true;
				}
			}
			value = null;
			return false;
		}

		public IEnumerable<AbilityDef> GetAllowedAbilities(Pawn pawn)
		{
			List<AbilityDef> list;
			if (this.abilityCache == null)
			{
				this.abilityCache = new Dictionary<ThingDef, List<AbilityDef>>();
				foreach (AbilityDef current in DefDatabase<AbilityDef>.AllDefs)
				{
					foreach (ThingDef current2 in current.races)
					{
						if (!this.abilityCache.TryGetValue(current2, out list))
						{
							list = new List<AbilityDef>();
							this.abilityCache.Add(current2, list);
						}
						list.Add(current);
					}
				}
			}
			if (this.abilityCache.TryGetValue(pawn.def, out list))
			{
				return list;
			}
			return this.emptyAbilities;
		}

		private bool TryStartNextAbility(Saveable_Caster value, out AbilityDef ability, out List<Thing> targets, out IExposable effectState)
		{
			List<KeyValuePair<int, AbilityDef>> list = new List<KeyValuePair<int, AbilityDef>>();
			foreach (Saveable_ExecutionLog current in value.executionLogs)
			{
				if (!current.ability.races.Contains(value.pawn.def) || current.ability.validity == null || !current.ability.validity.Sucess(current.ability, value))
				{
					current.isValid = false;
				}
				else
				{
					current.isValid = true;
					if (current.ability.requeriment == null || current.ability.requeriment.Sucess(current.ability, value))
					{
						list.Add(new KeyValuePair<int, AbilityDef>(current.ability.priority.GetPriority(current.ability, value.pawn), current.ability));
					}
				}
			}
			if (list.Any<KeyValuePair<int, AbilityDef>>())
			{
				list.Sort((KeyValuePair<int, AbilityDef> x, KeyValuePair<int, AbilityDef> y) => y.Key.CompareTo(x.Key));
				foreach (KeyValuePair<int, AbilityDef> current2 in list)
				{
					effectState = null;
					targets = null;
					if (current2.Value.target != null)
					{
						targets = current2.Value.target.Targets(current2.Value, value).ToList<Thing>();
					}
					if (current2.Value.effect.TryStart(current2.Value, value, ref targets, ref effectState))
					{
						ability = current2.Value;
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
