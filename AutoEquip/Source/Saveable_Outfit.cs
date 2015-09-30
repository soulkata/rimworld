using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutoEquip
{
    public class Saveable_Outfit : IExposable
    {
        public Outfit outfit;
        public List<Saveable_StatDef> stats = new List<Saveable_StatDef>();
        public bool appendIndividualPawnStatus;

        public void ExposeData()
        {
            Scribe_References.LookReference(ref this.outfit, "outfit");
            Scribe_Collections.LookList(ref this.stats, "stats", LookMode.Deep);
            Scribe_Values.LookValue(ref this.appendIndividualPawnStatus, "IndividualStatus", true);
        }
    }
}
