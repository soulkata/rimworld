using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AbilityPack
{
    public class AbilityEffect_Workaround_MakeCorpseProvideCover : AbilityEffect_Cast
    {
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

        public override void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, Saveable effectState)
        {
            foreach (Corpse corpse in targets)
            {
                corpse.def.fillPercent = 0.75f;
                Find.CoverGrid.Register(corpse);
            }
        }
    }
}
