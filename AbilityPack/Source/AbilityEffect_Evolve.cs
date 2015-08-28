using RimWorld;
using RimWorld.SquadAI;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
    public class AbilityEffect_Evolve : AbilityEffect_Cast
    {
        public List<AbilityEffect_UtilityChangeKind> items;

        public override bool TryStart(AbilityDef ability, Saveable_Caster caster, ref List<Thing> target, ref IExposable effectState)
        {
            if (!base.TryStart(ability, caster, ref target, ref effectState))
                return false;

            if ((target == null) ||
                (this.items == null) ||
                (!this.items.Any()))
                return false;

            target = target.OfType<Pawn>().Where(i => this.items.SelectMany(j => j.from).Contains(i.def)).OfType<Thing>().ToList();

            return target.Any();
        }

        public override void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, IExposable effectState)
        {
            MapComponent_Ability component = MapComponent_Ability.GetOrCreate();
            foreach (Pawn target in targets)
            {
                AbilityEffect_UtilityChangeKind evolveItem = this.items.First(i => i.from.Contains(target.def));


                Brain brain = target.GetSquadBrain();
                foreach (PawnKindDef kind in evolveItem.to)
                {
                    Pawn newPawn = AbilityEffect_Revive.Copy(caster.pawn, kind, target.Faction);
                    GenSpawn.Spawn(newPawn, target.Position);

                    if (brain != null)
                        brain.AddPawn(newPawn);
                }

                Building building = StoreUtility.StoringBuilding(target);
                if (building != null)
                    ((Building_Storage)building).Notify_LostThing(target);
                target.Destroy(DestroyMode.Vanish);
            }
        }
    }

    public class AbilityEffect_UtilityChangeKind
    {
        public List<ThingDef> from;
        public List<PawnKindDef> to;
    }
}
