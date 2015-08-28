using Verse;

namespace FactionGauntlet
{
    public class LayerChooserFightStyle : Dialog_DebugOptionLister
    {
        protected override void DoListingItems()
        {
            this.listing.DoLabel("Choose the Gauntlet type:");
            
            this.DrawDebugAction("1 Sudden Death", delegate()
            {
                MapComponentFactionGauntlet.GetMapComponent().style = FightStyle.SuddendDeath;
                MapComponentFactionGauntlet.GetMapComponent().roundNumbers = 1;
            });
            this.DrawDebugAction("3 Sudden Death", delegate()
            {
                MapComponentFactionGauntlet.GetMapComponent().style = FightStyle.SuddendDeath;
                MapComponentFactionGauntlet.GetMapComponent().roundNumbers = 3;
            });
            this.DrawDebugAction("5 Sudden Death", delegate()
            {
                MapComponentFactionGauntlet.GetMapComponent().style = FightStyle.SuddendDeath;
                MapComponentFactionGauntlet.GetMapComponent().roundNumbers = 5;
            });
            this.DrawDebugAction("12 Sudden Death", delegate()
            {
                MapComponentFactionGauntlet.GetMapComponent().style = FightStyle.SuddendDeath;
                MapComponentFactionGauntlet.GetMapComponent().roundNumbers = 12;
            });
            this.DrawDebugAction("3 Rounds Fight", delegate()
            {
                MapComponentFactionGauntlet.GetMapComponent().style = FightStyle.TimedRounds;
                MapComponentFactionGauntlet.GetMapComponent().roundNumbers = 3;
            });
            this.DrawDebugAction("5 Rounds Fight", delegate()
            {
                MapComponentFactionGauntlet.GetMapComponent().style = FightStyle.TimedRounds;
                MapComponentFactionGauntlet.GetMapComponent().roundNumbers = 5;
            });
            this.DrawDebugAction("12 Rounds Fight", delegate()
            {
                MapComponentFactionGauntlet.GetMapComponent().style = FightStyle.TimedRounds;
                MapComponentFactionGauntlet.GetMapComponent().roundNumbers = 12;
            });

            this.listing.DoLabel("Sudden Death: Fight until all unit dies");
            this.listing.DoLabel("Rounds: Fight for " + MapComponentFactionGauntlet.roundLenght.ToString("N0") + " ticks");
        }
    }
}
