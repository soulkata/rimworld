using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutoEquip
{
    public class Saveable_Outfit : IExposable
    {
        public Outfit outfit;
        public List<Saveable_Outfit_StatDef> stats = new List<Saveable_Outfit_StatDef>();

        public void ExposeData()
        {
            Scribe_References.LookReference(ref this.outfit, "outfit");
            Scribe_Collections.LookList(ref this.stats, "stats", LookMode.Deep);
        }
    }
}
