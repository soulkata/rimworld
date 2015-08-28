using RimWorld;
using System.Linq;
using Verse;

namespace FactionGauntlet
{
    public class LayerChooserFaction : Dialog_DebugOptionLister
    {
        protected override void DoListingItems()
        {
            if (MapComponentFactionGauntlet.GetMapComponent().toRaid.Count == 0)
                this.listing.DoLabel("Choose the faction to enter the gauntlet!");
            else
                this.listing.DoLabel("Choose the faction to fight aganist the " + MapComponentFactionGauntlet.GetMapComponent().toRaid[0].name + "!");
            
            foreach (Faction current in Find.FactionManager.AllFactions.Where(i => i.HostileTo(Faction.OfColony)))
            {
                if ((MapComponentFactionGauntlet.GetMapComponent().toRaid.Count == 0) ||
                    (MapComponentFactionGauntlet.GetMapComponent().toRaid[0] != current))
                {
                    this.DrawDebugAction(current.name, delegate() { MapComponentFactionGauntlet.GetMapComponent().toRaid.Add(current); });
                }
            }
        }
    }
}
