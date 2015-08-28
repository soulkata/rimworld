﻿using RimWorld;
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
        public float healUntil = 1;
        public bool changeFaction;
        public List<AbilityEffect_UtilityChangeKind> changes;

        public override bool TryStart(AbilityDef ability, Saveable_Caster caster, ref List<Thing> targets, ref Saveable effectState)
        {
            if (!base.TryStart(ability, caster, ref targets, ref effectState))
                return false;

            if (targets == null)
                return false;

            List<Thing> corpses = AbilityEffect_Revive.SelectCorpses(targets);

            if (corpses.Any())
            {
                targets = corpses;
                return true;
            }
            else
                return false;
        }

        public static List<Thing> SelectCorpses(List<Thing> targets)
        {
            List<Thing> corpses = new List<Thing>();
            List<Corpse> findCorpses = null;
            foreach (Thing t in targets)
            {
                Corpse corpse = t as Corpse;
                if (corpse == null)
                {
                    Pawn targetPawn = t as Pawn;
                    if (targetPawn == null)
                        continue;
                    if (!targetPawn.Dead)
                        continue;

                    if (findCorpses == null)
                        findCorpses = Find.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Corpse)).OfType<Corpse>().ToList();

                    foreach (Corpse findCorpse in findCorpses)
                    {
                        if (findCorpse.innerPawn == t)
                        {
                            corpse = findCorpse;
                            break;
                        }
                    }
                }

                if (corpse == null)
                    continue;

                corpses.Add(corpse);
            }
            return corpses;
        }

        public override void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, Saveable effectState)
        {
            MapComponent_Ability component = MapComponent_Ability.GetOrCreate();

            Brain brain = caster.pawn.GetSquadBrain();

            foreach (Corpse corpse in targets)
            {
                List<PawnKindDef> newKinds = new List<PawnKindDef>();
                if (this.changes != null)
                {
                    AbilityEffect_UtilityChangeKind kind = this.changes.FirstOrDefault(i => i.from.Contains(corpse.innerPawn.def));
                    if (kind != null)
                        newKinds.AddRange(kind.to);
                    else
                        newKinds.Add(corpse.innerPawn.kindDef);
                }
                else
                    newKinds.Add(corpse.innerPawn.kindDef);

                foreach (PawnKindDef kindDef in newKinds)
                {
                    Pawn newPawn = AbilityEffect_Revive.Copy(caster.pawn, kindDef, this.changeFaction ? caster.pawn.Faction : corpse.innerPawn.Faction);

                    if (corpse.innerPawn == caster.pawn)
                        component.ReplacePawnAbility(caster, newPawn);
                    
                    IntVec3 position = corpse.Position;
                    
                    GenSpawn.Spawn(newPawn, position);

                    if ((newPawn.Faction == caster.pawn.Faction) &&
                        (brain != null))
                        brain.AddPawn(newPawn);
                }

                Building building = StoreUtility.StoringBuilding(corpse);
                if (building != null)
                    ((Building_Storage)building).Notify_LostThing(corpse);
                corpse.Destroy(DestroyMode.Vanish);

                //if (this.healUntil > 0)
                //{
                //    HediffSet hediffSet = newPawn.healthTracker.hediffSet;
                //    while ((newPawn.healthTracker.summaryHealth.SummaryHealthPercent >= this.healUntil) &&
                //        (AbilityEffect_Revive.HittablePartsViolence(hediffSet).Any<BodyPartRecord>()))
                //    {
                //        BodyPartRecord bodyPartRecord = AbilityEffect_Revive.HittablePartsViolence(hediffSet).RandomElementByWeight((BodyPartRecord x) => x.absoluteFleshCoverage);
                //        int amount = Rand.RangeInclusive(8, 25);
                //        DamageDef def;
                //        if (bodyPartRecord.depth == BodyPartDepth.Outside)
                //            def = HealthUtility.RandomViolenceDamageType();
                //        else
                //            def = DamageDefOf.Blunt;
                //        DamageInfo dinfo = new DamageInfo(def, amount, null, new BodyPartDamageInfo?(new BodyPartDamageInfo(bodyPartRecord, false, (List<HediffDef>)null)), null);
                //        newPawn.TakeDamage(dinfo);
                //    }
                //}                
            }
        }

        private static IEnumerable<BodyPartRecord> HittablePartsViolence(HediffSet bodyModel)
        {
            return
                from x in bodyModel.GetNotMissingParts(null, null)
                where x.depth == BodyPartDepth.Outside || (x.depth == BodyPartDepth.Inside && x.def.IsSolid(x, bodyModel.hediffs))
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
            if (pawn.RaceProps.ToolUser)
            {
                pawn.equipment = new Pawn_EquipmentTracker(pawn);
                pawn.carryHands = new Pawn_CarryHands(pawn);
                pawn.apparel = new Pawn_ApparelTracker(pawn);
                pawn.inventory = new Pawn_InventoryTracker(pawn);
            }
            if (pawn.RaceProps.Humanlike)
            {
                pawn.ownership = new Pawn_Ownership(pawn);
                pawn.skills = new Pawn_SkillTracker(pawn);
                pawn.talker = new Pawn_TalkTracker(pawn);
                pawn.story = new Pawn_StoryTracker(pawn);
                pawn.workSettings = new Pawn_WorkSettings(pawn);
            }
            if (pawn.RaceProps.intelligence <= Intelligence.ToolUser)
            {
                pawn.caller = new Pawn_CallTracker(pawn);
            }
            PawnUtility.AddAndRemoveComponentsAsAppropriate(pawn);
            if (pawn.RaceProps.hasGenders)
            {
                if ((sourcePawn != null) &&
                    (sourcePawn.RaceProps.hasGenders) &&
                    (sourcePawn.gender != Gender.None))
                    pawn.gender = sourcePawn.gender;
                else
                {
                    if (Rand.Value < 0.5f)
                        pawn.gender = Gender.Male;
                    else
                        pawn.gender = Gender.Female;
                }
            }
            else
                pawn.gender = Gender.None;

            AbilityEffect_Revive.GenerateRandomAge_Coping(pawn, sourcePawn);
            AbilityEffect_Revive.GenerateInitialHediffs_Coping(pawn, sourcePawn);
            if (pawn.RaceProps.Humanlike)
            {
                if ((sourcePawn != null) &&
                    ((forceBodyVisual) ||
                     (sourcePawn.def == sourcePawn.def)))
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
                    pawn.story.crownType = ((Rand.Value >= 0.5f) ? CrownType.Narrow : CrownType.Average);
                    pawn.story.headGraphicPath = GraphicDatabaseHeadRecords.GetHeadRandom(pawn.gender, pawn.story.skinColor, pawn.story.crownType).GraphicPath;
                    pawn.story.hairColor = PawnHairColors.RandomHairColor(pawn.story.skinColor, pawn.ageTracker.AgeBiologicalYears);

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
            if ((sourcePawn != null) &&
                ((sourcePawn.RaceProps.Humanlike && pawn.RaceProps.Humanlike) ||
                 (sourcePawn.def == pawn.def)))
            {
                if (sourcePawn.inventory == null)
                    return;

                while (sourcePawn.inventory.container.Contents.Any())
                {
                    Thing equipment;
                    sourcePawn.inventory.TryDrop(sourcePawn.inventory.container.Contents.First(), out equipment);
                    pawn.inventory.container.TryAdd(equipment);
                }
            }
            else
                PawnInventoryGenerator.GenerateInventoryFor(pawn);
        }

        private static void TryGenerateWeaponFor_Coping(Pawn pawn, Pawn sourcePawn, bool forceWeapon)
        {
            if ((sourcePawn != null) &&
                ((forceWeapon) ||
                 (sourcePawn.RaceProps.Humanlike && pawn.RaceProps.Humanlike) ||
                 (sourcePawn.def == pawn.def)))
            {
                // TODO: Verificar pela arma que foi derrubada

                if (sourcePawn.equipment == null)
                    return;

                while (sourcePawn.equipment.AllEquipment.Any())
                {
                    ThingWithComps equipment;
                    sourcePawn.equipment.TryDropEquipment(sourcePawn.equipment.AllEquipment.First(), out equipment, sourcePawn.Position, true);
                    pawn.equipment.AddEquipment(equipment);
                }
            }
            else
                PawnWeaponGenerator.TryGenerateWeaponFor(pawn);
        }

        private static void GenerateStartingApparelFor_Coping(Pawn pawn, Pawn sourcePawn, bool forceApparel)
        {
            if ((sourcePawn != null) &&
                ((forceApparel) ||
                 (sourcePawn.RaceProps.Humanlike && pawn.RaceProps.Humanlike) ||
                 (sourcePawn.def == pawn.def)))
            {
                if (sourcePawn.apparel == null || sourcePawn.apparel.WornApparelCount == 0)
                    return;

                while (sourcePawn.apparel.WornApparel.Any())
                {
                    Apparel ap;
                    sourcePawn.apparel.TryDrop(sourcePawn.apparel.WornApparel.First(),
                        out ap);
                    pawn.apparel.Wear(ap);
                }
                
                //using (IEnumerator<Apparel> enumerator = .GetEnumerator())
                //{
                //    while (enumerator.MoveNext())
                //    {
                //        Apparel apparel = (Apparel)enumerator.Current;
                //        Apparel apparel2;
                //        if (apparel.def.MadeFromStuff)
                //            apparel2 = (Apparel)ThingMaker.MakeThing(apparel.def, apparel.Stuff);
                //        else
                //            apparel2 = (Apparel)ThingMaker.MakeThing(apparel.def, null);
                //        apparel2.DrawColor = new Color(apparel.DrawColor.r, apparel.DrawColor.g, apparel.DrawColor.b, apparel.DrawColor.a);
                //        pawn.apparel.Wear(apparel2, true);
                //    }
                //}
            }
            else
                PawnApparelGenerator.GenerateStartingApparelFor(pawn);
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
                pawn.story.traits.GainTrait(current);
        }

        private static void GenerateInitialHediffs_Coping(Pawn pawn, Pawn sourcePawn)
        {
            GenerateInitialHediffs(pawn);
        }

        private static void GenerateInitialHediffs(Pawn pawn)
        {
            int num = 0;
            while (true)
            {
                pawn.health.hediffSet.Clear();
                AgeInjuryUtility.GenerateRandomOldAgeInjuries(pawn);
                PawnArtificialBodyPartsGenerator.GeneratePartsFor(pawn);
                num++;
                if (num > 10)
                {
                    break;
                }
                if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
                {
                    return;
                }
            }
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
            while (true)
            {
                if (pawn.RaceProps.ageGenerationCurve != null)
                {
                    num2 = Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.ageGenerationCurve, 200));
                }
                else if (pawn.RaceProps.mechanoid)
                {
                    num2 = Rand.Range(0, 2500);
                }
                else
                {
                    if (!pawn.RaceProps.Animal)
                    {
                        break;
                    }
                    num2 = Rand.Range(1, 10);
                }
                num++;
                if (num > 100)
                {
                    goto Block_4;
                }
                if (num2 <= pawn.kindDef.maxGenerationAge && num2 >= pawn.kindDef.minGenerationAge)
                {
                    goto IL_C7;
                }
            }
            Log.Warning("Didn't get age for " + pawn);
            return;
        Block_4:
            Log.Error("Tried 100 times to generate age for " + pawn);
        IL_C7:
            pawn.ageTracker.AgeBiologicalTicks = (long)num2 * 3600000L + (long)Rand.Range(0, 3600000);
            int num3 = (int)((Game.Mode != GameMode.MapPlaying) ? (((Month)((int)MapInitData.startingMonth * 300000))) : ((Month)Find.TickManager.TicksAbs));
            long absTicks = (long)num3 - pawn.ageTracker.AgeBiologicalTicks;
            int num4 = GenDate.CalendarYearAt(absTicks);
            int birthDayOfYear = GenDate.DayOfYearAt(absTicks);
            int num5;
            if (Rand.Value < pawn.kindDef.backstoryCryptosleepCommonality)
            {
                float value = UnityEngine.Random.value;
                if (value < 0.7f)
                {
                    num5 = UnityEngine.Random.Range(0, 100);
                }
                else if (value < 0.95f)
                {
                    num5 = UnityEngine.Random.Range(100, 1000);
                }
                else
                {
                    int max = GenDate.CurrentYear - 2026 - pawn.ageTracker.AgeBiologicalYears;
                    num5 = UnityEngine.Random.Range(1000, max);
                }
            }
            else
            {
                num5 = 0;
            }
            num4 -= num5;
            pawn.ageTracker.SetChronologicalBirthDate(num4, birthDayOfYear);
        }

        private static void GiveRandomTraitsTo(Pawn pawn)
        {
            if (pawn.story == null)
                return;

            int num = Rand.RangeInclusive(2, 3);
            while (pawn.story.traits.allTraits.Count < num)
            {
                TraitDef newTraitDef = DefDatabase<TraitDef>.AllDefsListForReading.RandomElementByWeight((TraitDef tr) => tr.commonality);
                if (!pawn.story.traits.HasTrait(newTraitDef))
                {
                    if (!pawn.story.traits.allTraits.Any((Trait tr) => newTraitDef.ConflictsWith(tr)) && (newTraitDef.conflictingTraits == null || !newTraitDef.conflictingTraits.Any((TraitDef tr) => pawn.story.traits.HasTrait(tr))))
                    {
                        if (newTraitDef.requiredWorkTypes == null || !pawn.story.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes))
                        {
                            if (!pawn.story.WorkTagIsDisabled(newTraitDef.requiredWorkTags))
                            {
                                Trait trait = new Trait(newTraitDef);
                                trait.degree = PawnGenerator.RandomTraitDegree(trait.def);
                                if (pawn.mindState.breaker.HardBreakThreshold + trait.OffsetOfStat(StatDefOf.MentalBreakThreshold) <= 40f)
                                {
                                    pawn.story.traits.GainTrait(trait);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static class AgeInjuryUtility
        {
            public static void GenerateRandomOldAgeInjuries(Pawn pawn)
            {
                int num = 0;
                for (int i = 10; i < pawn.ageTracker.AgeBiologicalYears; i += 10)
                {
                    if (Rand.Value < 0.15f)
                    {
                        num++;
                    }
                }
                for (int j = 0; j < num; j++)
                {
                    DamageDef dam = AgeInjuryUtility.RandomOldInjuryDamageType();
                    int num2 = Rand.RangeInclusive(2, 6);
                    IEnumerable<BodyPartRecord> source =
                        from x in pawn.health.hediffSet.GetNotMissingParts(null, null)
                        where x.depth == BodyPartDepth.Outside && !Mathf.Approximately(x.def.oldInjuryBaseChance, 0f) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(x)
                        select x;
                    if (source.Any<BodyPartRecord>())
                    {
                        BodyPartRecord bodyPartRecord = source.RandomElementByWeight((BodyPartRecord x) => x.absoluteFleshCoverage);
                        HediffDef hediffDefFromDamage = HealthUtility.GetHediffDefFromDamage(dam, pawn, bodyPartRecord);
                        if (bodyPartRecord.def.oldInjuryBaseChance > 0f && hediffDefFromDamage.CompPropsFor(typeof(HediffComp_GetsOld)) != null)
                        {
                            Hediff_Injury hediff_Injury = (Hediff_Injury)HediffMaker.MakeHediff(hediffDefFromDamage, pawn);
                            hediff_Injury.Severity = (float)num2;
                            hediff_Injury.TryGetComp<HediffComp_GetsOld>().isOld = true;
                            pawn.health.AddHediff(hediff_Injury, bodyPartRecord, null);
                        }
                    }
                }
                for (int k = 1; k < pawn.ageTracker.AgeBiologicalYears; k++)
                {
                    foreach (HediffDef current in AgeInjuryUtility.HediffsToGainOnBirthday(pawn, k))
                    {
                        current.Worker.TryApplyBecauseOfAge(pawn);
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
                return AgeInjuryUtility.HediffsToGainOnBirthday(pawn.thingIDNumber, pawn.ageTracker.AgeBiologicalYears);
            }

            private static IEnumerable<HediffDef> HediffsToGainOnBirthday(int seed, int age)
            {
                foreach (HediffDef itm in DefDatabase<HediffDef>.AllDefsListForReading)
                {
                    if (itm.ageGainCurve != null && Rand.Value < itm.ageGainCurve.PeriodProbabilityFromCumulative((float)age, 1f))
                        yield return itm;
                }
            }
        }
    }
}