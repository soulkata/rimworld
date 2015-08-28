using Verse;

namespace FactionGauntlet
{
    public class LayerChooserRaidScale : Dialog_DebugOptionLister
    {
        protected override void DoListingItems()
        {
            this.listing.DoLabel("Choose how big the squads will be:");

            this.DrawDebugAction((100.0).ToString("N0") + "pts", delegate() { MapComponentFactionGauntlet.GetMapComponent().raidScales.Add(100.0f); });
            this.DrawDebugAction((500.0).ToString("N0") + "pts", delegate() { MapComponentFactionGauntlet.GetMapComponent().raidScales.Add(500.0f); });
            this.DrawDebugAction((1000.0).ToString("N0") + "pts", delegate() { MapComponentFactionGauntlet.GetMapComponent().raidScales.Add(1000.0f); });
            this.DrawDebugAction((2000.0).ToString("N0") + "pts", delegate() { MapComponentFactionGauntlet.GetMapComponent().raidScales.Add(2000.0f); });
            this.DrawDebugAction((5000.0).ToString("N0") + "pts", delegate() { MapComponentFactionGauntlet.GetMapComponent().raidScales.Add(5000.0f); });
            this.DrawDebugAction("All of Above!", delegate() { MapComponentFactionGauntlet.GetMapComponent().raidScales.AddRange(new float[] { 100.0f, 500.0f, 1000.0f, 2000.0f, 5000.0f }); });
        }
    }
}
