using Verse;

namespace FactionGauntlet
{
    public class LayerChooserArenaGround : Dialog_DebugOptionLister
    {
        protected override void DoListingItems()
        {
            this.listing.DoLabel("Choose fighting grounds!");

            this.DrawDebugAction("0% SandBag, 0% Wall (Clean Arena)", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 000; MapComponentFactionGauntlet.GetMapComponent().wallChance = 000; });
            this.DrawDebugAction("1% SandBag, 0% Wall", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 001; MapComponentFactionGauntlet.GetMapComponent().wallChance = 000; });
            this.DrawDebugAction("5% SandBag, 0% Wall", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 005; MapComponentFactionGauntlet.GetMapComponent().wallChance = 000; });
            this.DrawDebugAction("0% SandBag, 1% Wall", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 000; MapComponentFactionGauntlet.GetMapComponent().wallChance = 001; });
            this.DrawDebugAction("1% SandBag, 1% Wall", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 001; MapComponentFactionGauntlet.GetMapComponent().wallChance = 001; });
            this.DrawDebugAction("5% SandBag, 1% Wall", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 005; MapComponentFactionGauntlet.GetMapComponent().wallChance = 001; });
            this.DrawDebugAction("0% SandBag, 5% Wall", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 000; MapComponentFactionGauntlet.GetMapComponent().wallChance = 005; });
            this.DrawDebugAction("1% SandBag, 5% Wall", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 001; MapComponentFactionGauntlet.GetMapComponent().wallChance = 005; });
            this.DrawDebugAction("5% SandBag, 5% Wall", delegate() { MapComponentFactionGauntlet.GetMapComponent().sandBagChance = 005; MapComponentFactionGauntlet.GetMapComponent().wallChance = 005; });
        }
    }
}
