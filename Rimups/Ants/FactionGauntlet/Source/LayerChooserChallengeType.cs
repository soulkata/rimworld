using Verse;

namespace FactionGauntlet
{
    public class LayerChooserChallengeType : Dialog_DebugOptionLister
    {
        protected override void DoListingItems()
        {
            this.listing.DoLabel("Wellcome to te faction gauntlet!");

            this.listing.DoLabel("Choose one of the challenges types below:");

            this.DrawDebugAction("Fight another Faction", delegate() { MapComponentFactionGauntlet.GetMapComponent().competitorTable = ChallengeType.AnotherFaction; });
            this.DrawDebugAction("Challenge Core Factions", delegate() { MapComponentFactionGauntlet.GetMapComponent().competitorTable = ChallengeType.AllCoreFactions; });
            this.DrawDebugAction("Challenge All Factions", delegate() { MapComponentFactionGauntlet.GetMapComponent().competitorTable = ChallengeType.AllOtherFactions; });            
        }
    }
}
