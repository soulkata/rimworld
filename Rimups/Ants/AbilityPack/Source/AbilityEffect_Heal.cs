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

        public override bool TryStart(AbilityDef ability, Saveable_Caster caster, ref List<Thing> targets, ref Saveable effectState)
        {
            if (!base.TryStart(ability, caster, ref targets, ref effectState))
                return false;

            if (targets == null)
                return false;

            targets = targets
                .OfType<Pawn>()
                .Where(i => i.MaxHitPoints > 0)
                .Where(i => i.health.summaryHealth.SummaryHealthPercent < this.healthTrashHold)
                .Cast<Thing>()
                .ToList();

            if (targets.Any())
                return true;
            else
                return false;
        }

        public override void OnSucessfullCast(Saveable_Caster caster, IEnumerable<Thing> targets, Saveable effectState)
        {
            float exceedPower = this.totalPower;
            foreach (Pawn target in targets.OfType<Pawn>())
            {
                float pawnAvailPower = this.targetPower;

                if (pawnAvailPower + exceedPower >= this.treatLocalInjuryPowerUse)
                {
                    foreach (Hediff_Injury toTreat in target.health.hediffSet.GetInjuriesLocalTreatable().OrderByDescending(i => i.Severity))
                    {
                        pawnAvailPower -= this.treatLocalInjuryPowerUse;

                        HediffComp_Treatable hediffComp_Treatable = toTreat.TryGetComp<HediffComp_Treatable>();
                        if (hediffComp_Treatable == null)
                            Log.Error("Tried to treat " + toTreat + " which does not have a HediffComp_Treatable");
                        else
                            hediffComp_Treatable.NewlyTreated(0.3f, null);                        

                        Brain brain = caster.pawn.GetSquadBrain();
                        if ((brain != null) &&
                            (!target.Downed) &&
                            (target.GetSquadBrain() == null))
                            brain.AddPawn(target);

                        if (pawnAvailPower < 0)
                        {
                            exceedPower += pawnAvailPower;
                            pawnAvailPower = 0;
                        }

                        if (pawnAvailPower + exceedPower < this.treatLocalInjuryPowerUse)
                            break;
                    }
                }

                if (pawnAvailPower + exceedPower >= this.healLocalInjuryPowerUse)
                {
                    foreach (Hediff_Injury toHeal in target.health.hediffSet.hediffs.OfType<Hediff_Injury>().Where(i => i.IsTreatedAndHealing() && (i.Severity != 0.0f)).OrderByDescending(i => i.Severity))
                    {
                        float maximum = ((pawnAvailPower + exceedPower) / this.healLocalInjuryPowerUse);
                        maximum = Math.Min(maximum, toHeal.Severity);

                        toHeal.DirectHeal(maximum);

                        pawnAvailPower -= maximum * this.healLocalInjuryPowerUse;
                        if (pawnAvailPower < 0)
                        {
                            exceedPower += pawnAvailPower;
                            pawnAvailPower = 0;
                        }

                        if (pawnAvailPower + exceedPower < this.healLocalInjuryPowerUse)
                            break;
                    }
                }
            }
        }
    }
}
