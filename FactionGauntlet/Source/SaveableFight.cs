using RimWorld;
using System.Collections.Generic;
using Verse;

namespace FactionGauntlet
{
    public class SaveableFight : IExposable
    {
        public float points;
        public Faction factionTwo;
        public Faction factionOne;
        public List<Faction> winners = new List<Faction>();

        public void ExposeData()
        {
            
        }
    }
}
