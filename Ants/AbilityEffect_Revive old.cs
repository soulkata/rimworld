using RimWorld;
using RimWorld.SquadAI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AbilityPack
{
	public class AbilityEffect_Revive : AbilityEffect_Cast
	{
		internal static class AgeInjuryUtility
		{
			public static void GenerateRandomOldAgeInjuries(Pawn pawn)
			{
				int num = 0;
				for (int i = 10; i < pawn.ageTracker.get_AgeBiologicalYears(); i += 10)
				{
					if (Rand.get_Value() < 0.15f)
					{
						num++;
					}
				}
				for (int j = 0; j < num; j++)
				{
					DamageDef damageDef = AbilityEffect_Revive.AgeInjuryUtility.RandomOldInjuryDamageType();
					int num2 = Rand.RangeInclusive(2, 6);
					IEnumerable<BodyPartRecord> enumerable = from x in pawn.health.hediffSet.GetNotMissingParts(null, null)
					where x.depth == 2 && !Mathf.Approximately(x.def.oldInjuryBaseChance, 0f) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(x)
					select x;
					if (enumerable.Any<BodyPartRecord>())
					{
						BodyPartRecord bodyPartRecord = GenCollection.RandomElementByWeight<BodyPartRecord>(enumerable, (BodyPartRecord x) => x.absoluteFleshCoverage);
						HediffDef hediffDefFromDamage = HealthUtility.GetHediffDefFromDamage(damageDef, pawn, bodyPartRecord);
						if (bodyPartRecord.def.oldInjuryBaseChance > 0f && hediffDefFromDamage.CompPropsFor(typeof(HediffComp_GetsOld)) != null)
						{
							Hediff_Injury hediff_Injury = (Hediff_Injury)HediffMaker.MakeHediff(hediffDefFromDamage, pawn);
							hediff_Injury.set_Severity((float)num2);
							HediffUtility.TryGetComp<HediffComp_GetsOld>(hediff_Injury).isOld = true;
							pawn.health.AddHediff(hediff_Injury, bodyPartRecord, null);
						}
					}
				}
				for (int k = 1; k < pawn.ageTracker.get_AgeBiologicalYears(); k++)
				{
					foreach (HediffDef current in AbilityEffect_Revive.AgeInjuryUtility.HediffsToGainOnBirthday(pawn, k))
					{
						current.get_Worker().TryApplyBecauseOfAge(pawn);
					}
				}
			}

			private static DamageDef RandomOldInjuryDamageType()
			{
				switch (Rand.RangeInclusive(0, 3))
				{
				case 0:
					return DamageDefOf.Bullet;
				case 1:
					return DamageDefOf.Scratch;
				case 2:
					return DamageDefOf.Bite;
				case 3:
					return DamageDefOf.Stab;
				default:
					throw new Exception();
				}
			}

			public static IEnumerable<HediffDef> HediffsToGainOnBirthday(Pawn pawn, int age)
			{
				return AbilityEffect_Revive.AgeInjuryUtility.HediffsToGainOnBirthday(pawn.thingIDNumber, pawn.ageTracker.get_AgeBiologicalYears());
			}

			private static IEnumerable<HediffDef> HediffsToGainOnBirthday(int seed, int age)
			{
				foreach (HediffDef current in DefDatabase<HediffDef>.get_AllDefsListForReading())
				{
					if (current.ageGainCurve != null && Rand.get_Value() < current.ageGainCurve.PeriodProbabilityFromCumulative((float)age, 1f))
					{
						yield return current;
					}
				}
				yield break;
			}
		}

		public float healUntil = 1f;

		public bool changeFaction;

		public List<AbilityEffect_UtilityChangeKind> changes;

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

		public static List<Thing> SelectCorpses(List<Thing> targets)
		{
			List<Thing> list = new List<Thing>();
			List<Corpse> list2 = null;
			foreach (Thing current in targets)
			{
				Corpse corpse = current as Corpse;
				if (corpse == null)
				{
					Pawn pawn = current as Pawn;
					if (pawn == null || !pawn.get_Dead())
					{
						continue;
					}
					if (list2 == null)
					{
						list2 = Find.get_ListerThings().ThingsMatching(ThingRequest.ForGroup(8)).OfType<Corpse>().ToList<Corpse>();
					}
					foreach (Corpse current2 in list2)
					{
						if (current2.innerPawn == current)
						{
							corpse = current2;
							break;
						}
					}
				}
				if (corpse != null)
				{
					list.Add(corpse);
				}
			}
			return list;
		}

		public override void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
		{
			MapComponent_Ability orCreate = MapComponent_Ability.GetOrCreate();
			Brain squadBrain = BrainUtility.GetSquadBrain(caster.pawn);
			using (IEnumerator<Thing> enumerator = targets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Corpse corpse = (Corpse)enumerator.Current;
					List<PawnKindDef> list = new List<PawnKindDef>();
					if (this.changes != null)
					{
						AbilityEffect_UtilityChangeKind abilityEffect_UtilityChangeKind = this.changes.FirstOrDefault((AbilityEffect_UtilityChangeKind i) => i.from.Contains(corpse.innerPawn.def));
						if (abilityEffect_UtilityChangeKind != null)
						{
							list.AddRange(abilityEffect_UtilityChangeKind.to);
						}
						else
						{
							list.Add(corpse.innerPawn.kindDef);
						}
					}
					else
					{
						list.Add(corpse.innerPawn.kindDef);
					}
					foreach (PawnKindDef current in list)
					{
						Pawn pawn = AbilityEffect_Revive.Copy(caster.pawn, current, this.changeFaction ? caster.pawn.get_Faction() : corpse.innerPawn.get_Faction(), false, false, false);
						if (corpse.innerPawn == caster.pawn)
						{
							orCreate.ReplacePawnAbility(caster, pawn);
						}
						IntVec3 position = corpse.get_Position();
						GenSpawn.Spawn(pawn, position);
						if (pawn.get_Faction() == caster.pawn.get_Faction() && squadBrain != null)
						{
							squadBrain.AddPawn(pawn);
						}
					}
					Building building = StoreUtility.StoringBuilding(corpse);
					if (building != null)
					{
						((Building_Storage)building).Notify_LostThing(corpse);
					}
					corpse.Destroy(0);
				}
			}
		}

		private static IEnumerable<BodyPartRecord> HittablePartsViolence(HediffSet bodyModel)
		{
			return from x in bodyModel.GetNotMissingParts(null, null)
			where x.depth == 2 || (x.depth == 1 && x.def.IsSolid(x, bodyModel.hediffs))
			select x;
		}

		public static Pawn Copy(Pawn sourcePawn, PawnKindDef kindDef, Faction faction, bool forceBodyVisual = false, bool forceApparel = false, bool forceWeapon = false)
		{
			Pawn pawn = (Pawn)ThingMaker.MakeThing(kindDef.race, null);
			pawn.kindDef = kindDef;
			pawn.SetFactionDirect(faction);
			pawn.pather = new Pawn_PathFollower(pawn);
			pawn.ageTracker = new Pawn_AgeTracker(pawn);
			pawn.health = new Pawn_HealthTracker(pawn);
			pawn.jobs = new Pawn_JobTracker(pawn);
			pawn.mindState = new Pawn_MindState(pawn);
			pawn.filth = new Pawn_FilthTracker(pawn);
			pawn.needs = new Pawn_NeedsTracker(pawn);
			if (pawn.get_RaceProps().get_ToolUser())
			{
				pawn.equipment = new Pawn_EquipmentTracker(pawn);
				pawn.carryHands = new Pawn_CarryHands(pawn);
				pawn.apparel = new Pawn_ApparelTracker(pawn);
				pawn.inventory = new Pawn_InventoryTracker(pawn);
			}
			if (pawn.get_RaceProps().get_Humanlike())
			{
				pawn.ownership = new Pawn_Ownership(pawn);
				pawn.skills = new Pawn_SkillTracker(pawn);
				pawn.talker = new Pawn_TalkTracker(pawn);
				pawn.story = new Pawn_StoryTracker(pawn);
				pawn.workSettings = new Pawn_WorkSettings(pawn);
			}
			if (pawn.get_RaceProps().intelligence <= 1)
			{
				pawn.caller = new Pawn_CallTracker(pawn);
			}
			PawnUtility.AddAndRemoveComponentsAsAppropriate(pawn);
			if (pawn.get_RaceProps().hasGenders)
			{
				if (sourcePawn != null && sourcePawn.get_RaceProps().hasGenders && sourcePawn.gender != null)
				{
					pawn.gender = sourcePawn.gender;
				}
				else if (Rand.get_Value() < 0.5f)
				{
					pawn.gender = 1;
				}
				else
				{
					pawn.gender = 2;
				}
			}
			else
			{
				pawn.gender = 0;
			}
			AbilityEffect_Revive.GenerateRandomAge_Coping(pawn, sourcePawn);
			AbilityEffect_Revive.GenerateInitialHediffs_Coping(pawn, sourcePawn);
			if (pawn.get_RaceProps().get_Humanlike())
			{
				if (sourcePawn != null && (forceBodyVisual || sourcePawn.def == sourcePawn.def))
				{
					pawn.story.skinColor = sourcePawn.story.skinColor;
					pawn.story.crownType = sourcePawn.story.crownType;
					pawn.story.headGraphicPath = sourcePawn.story.headGraphicPath;
					pawn.story.hairColor = sourcePawn.story.hairColor;
					AbilityEffect_Revive.GiveAppropriateBioTo_Coping(pawn, sourcePawn);
					pawn.story.hairDef = sourcePawn.story.hairDef;
					AbilityEffect_Revive.GiveRandomTraitsTo_Coping(pawn, sourcePawn);
					pawn.story.GenerateSkillsFromBackstory();
				}
				else
				{
					pawn.story.skinColor = PawnSkinColors.RandomSkinColor();
					pawn.story.crownType = ((Rand.get_Value() >= 0.5f) ? 2 : 1);
					pawn.story.headGraphicPath = GraphicDatabaseHeadRecords.GetHeadRandom(pawn.gender, pawn.story.skinColor, pawn.story.crownType).get_GraphicPath();
					pawn.story.hairColor = PawnHairColors.RandomHairColor(pawn.story.skinColor, pawn.ageTracker.get_AgeBiologicalYears());
					PawnBioGenerator.GiveAppropriateBioTo(pawn, faction.def);
					pawn.story.hairDef = PawnHairChooser.RandomHairDefFor(pawn, faction.def);
					AbilityEffect_Revive.GiveRandomTraitsTo(pawn);
					pawn.story.GenerateSkillsFromBackstory();
				}
			}
			AbilityEffect_Revive.GenerateStartingApparelFor_Coping(pawn, sourcePawn, forceApparel);
			AbilityEffect_Revive.TryGenerateWeaponFor_Coping(pawn, sourcePawn, forceWeapon);
			AbilityEffect_Revive.GenerateInventoryFor_Coping(pawn, sourcePawn);
			PawnUtility.AddAndRemoveComponentsAsAppropriate(pawn);
			return pawn;
		}

		private static void GenerateInventoryFor_Coping(Pawn pawn, Pawn sourcePawn)
		{
			if (sourcePawn == null || ((!sourcePawn.get_RaceProps().get_Humanlike() || !pawn.get_RaceProps().get_Humanlike()) && sourcePawn.def != pawn.def))
			{
				PawnInventoryGenerator.GenerateInventoryFor(pawn);
				return;
			}
			if (sourcePawn.inventory == null)
			{
				return;
			}
			while (sourcePawn.inventory.container.get_Contents().Any<Thing>())
			{
				Thing thing;
				sourcePawn.inventory.TryDrop(sourcePawn.inventory.container.get_Contents().First<Thing>(), ref thing);
				pawn.inventory.container.TryAdd(thing);
			}
		}

		private static void TryGenerateWeaponFor_Coping(Pawn pawn, Pawn sourcePawn, bool forceWeapon)
		{
			if (sourcePawn == null || (!forceWeapon && (!sourcePawn.get_RaceProps().get_Humanlike() || !pawn.get_RaceProps().get_Humanlike()) && sourcePawn.def != pawn.def))
			{
				PawnWeaponGenerator.TryGenerateWeaponFor(pawn);
				return;
			}
			if (sourcePawn.equipment == null)
			{
				return;
			}
			while (sourcePawn.equipment.get_AllEquipment().Any<ThingWithComps>())
			{
				ThingWithComps thingWithComps;
				sourcePawn.equipment.TryDropEquipment(sourcePawn.equipment.get_AllEquipment().First<ThingWithComps>(), ref thingWithComps, sourcePawn.get_Position(), true);
				pawn.equipment.AddEquipment(thingWithComps);
			}
		}

		private static void GenerateStartingApparelFor_Coping(Pawn pawn, Pawn sourcePawn, bool forceApparel)
		{
			if (sourcePawn == null || (!forceApparel && (!sourcePawn.get_RaceProps().get_Humanlike() || !pawn.get_RaceProps().get_Humanlike()) && sourcePawn.def != pawn.def))
			{
				PawnApparelGenerator.GenerateStartingApparelFor(pawn);
				return;
			}
			if (sourcePawn.apparel == null || sourcePawn.apparel.get_WornApparelCount() == 0)
			{
				return;
			}
			while (sourcePawn.apparel.get_WornApparel().Any<Apparel>())
			{
				Apparel apparel;
				sourcePawn.apparel.TryDrop(sourcePawn.apparel.get_WornApparel().First<Apparel>(), ref apparel);
				pawn.apparel.Wear(apparel, true);
			}
		}

		private static void GiveAppropriateBioTo_Coping(Pawn pawn, Pawn sourcePawn)
		{
			PawnName name = default(PawnName);
			name.first = sourcePawn.story.name.first;
			name.last = sourcePawn.story.name.last;
			name.nick = sourcePawn.story.name.nick;
			name.ResolveMissingPieces();
			pawn.story.name = name;
			pawn.story.childhood = sourcePawn.story.childhood;
			pawn.story.adulthood = sourcePawn.story.adulthood;
			pawn.story.adulthood.bodyTypeGlobal = sourcePawn.story.adulthood.bodyTypeGlobal;
			pawn.story.adulthood.bodyTypeFemale = sourcePawn.story.adulthood.bodyTypeFemale;
			pawn.story.adulthood.bodyTypeMale = sourcePawn.story.adulthood.bodyTypeMale;
			pawn.story.childhood.bodyTypeGlobal = sourcePawn.story.childhood.bodyTypeGlobal;
			pawn.story.childhood.bodyTypeFemale = sourcePawn.story.childhood.bodyTypeFemale;
			pawn.story.childhood.bodyTypeMale = sourcePawn.story.childhood.bodyTypeMale;
			pawn.story.childhood.PostLoad();
			pawn.story.adulthood.PostLoad();
		}

		private static void GiveRandomTraitsTo_Coping(Pawn pawn, Pawn sourcePawn)
		{
			foreach (Trait current in sourcePawn.story.traits.allTraits)
			{
				pawn.story.traits.GainTrait(current);
			}
		}

		private static void GenerateInitialHediffs_Coping(Pawn pawn, Pawn sourcePawn)
		{
			AbilityEffect_Revive.GenerateInitialHediffs(pawn);
		}

		private static void GenerateInitialHediffs(Pawn pawn)
		{
			int num = 0;
			do
			{
				pawn.health.hediffSet.Clear();
				AbilityEffect_Revive.AgeInjuryUtility.GenerateRandomOldAgeInjuries(pawn);
				PawnArtificialBodyPartsGenerator.GeneratePartsFor(pawn);
				num++;
				if (num > 10)
				{
					goto IL_3F;
				}
			}
			while (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving));
			return;
			IL_3F:
			Log.Error("Could not generate old age injuries that allow pawn to move: " + pawn);
		}

		private static void GenerateRandomAge_Coping(Pawn pawn, Pawn sourcePawn)
		{
			AbilityEffect_Revive.GenerateRandomAge(pawn);
		}

		private static void GenerateRandomAge(Pawn pawn)
		{
			int num = 0;
			int num2;
			do
			{
				if (pawn.get_RaceProps().ageGenerationCurve != null)
				{
					num2 = Mathf.RoundToInt(Rand.ByCurve(pawn.get_RaceProps().ageGenerationCurve, 200));
				}
				else if (pawn.get_RaceProps().mechanoid)
				{
					num2 = Rand.Range(0, 2500);
				}
				else
				{
					if (!pawn.get_RaceProps().get_Animal())
					{
						goto IL_84;
					}
					num2 = Rand.Range(1, 10);
				}
				num++;
				if (num > 100)
				{
					goto IL_95;
				}
			}
			while (num2 > pawn.kindDef.maxGenerationAge || num2 < pawn.kindDef.minGenerationAge);
			goto IL_A5;
			IL_84:
			Log.Warning("Didn't get age for " + pawn);
			return;
			IL_95:
			Log.Error("Tried 100 times to generate age for " + pawn);
			IL_A5:
			pawn.ageTracker.set_AgeBiologicalTicks((long)num2 * 3600000L + (long)Rand.Range(0, 3600000));
			int num3 = (int)((Game.get_Mode() != 2) ? (MapInitData.startingMonth * 300000) : ((byte)Find.get_TickManager().get_TicksAbs()));
			long num4 = (long)num3 - pawn.ageTracker.get_AgeBiologicalTicks();
			int num5 = GenDate.CalendarYearAt(num4);
			int num6 = GenDate.DayOfYearAt(num4);
			int num7;
			if (Rand.get_Value() < pawn.kindDef.backstoryCryptosleepCommonality)
			{
				float value = UnityEngine.Random.value;
				if (value < 0.7f)
				{
					num7 = UnityEngine.Random.Range(0, 100);
				}
				else if (value < 0.95f)
				{
					num7 = UnityEngine.Random.Range(100, 1000);
				}
				else
				{
					int max = GenDate.get_CurrentYear() - 2026 - pawn.ageTracker.get_AgeBiologicalYears();
					num7 = UnityEngine.Random.Range(1000, max);
				}
			}
			else
			{
				num7 = 0;
			}
			num5 -= num7;
			pawn.ageTracker.SetChronologicalBirthDate(num5, num6);
		}

		private static void GiveRandomTraitsTo(Pawn pawn)
		{
			if (pawn.story == null)
			{
				return;
			}
			int num = Rand.RangeInclusive(2, 3);
			while (pawn.story.traits.allTraits.Count < num)
			{
				TraitDef newTraitDef = GenCollection.RandomElementByWeight<TraitDef>(DefDatabase<TraitDef>.get_AllDefsListForReading(), (TraitDef tr) => tr.commonality);
				if (!pawn.story.traits.HasTrait(newTraitDef))
				{
					if (!pawn.story.traits.allTraits.Any((Trait tr) => newTraitDef.ConflictsWith(tr)))
					{
						if (newTraitDef.conflictingTraits != null)
						{
							if (newTraitDef.conflictingTraits.Any((TraitDef tr) => pawn.story.traits.HasTrait(tr)))
							{
								continue;
							}
						}
						if ((newTraitDef.requiredWorkTypes == null || !pawn.story.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes)) && !pawn.story.WorkTagIsDisabled(newTraitDef.requiredWorkTags))
						{
							Trait trait = new Trait(newTraitDef);
							trait.degree = PawnGenerator.RandomTraitDegree(trait.def);
							if (pawn.mindState.breaker.get_HardBreakThreshold() + trait.OffsetOfStat(StatDefOf.MentalBreakThreshold) <= 40f)
							{
								pawn.story.traits.GainTrait(trait);
							}
						}
					}
				}
			}
		}
	}
}
