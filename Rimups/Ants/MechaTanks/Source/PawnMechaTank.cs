using System.Linq;
using Verse;

namespace MechaTanks
{
    public class PawnMechaTank : Pawn
    {
        public override void SpawnSetup()
        {
            base.SpawnSetup();

            RimWorld.Backstory back = RimWorld.BackstoryDatabase.allBackstories.Values.Where(i => !i.AllowsWorkType(RimWorld.WorkTypeDefOf.Construction)).First();
            this.story = new RimWorld.Pawn_StoryTracker(this);
            this.story.adulthood = back;
            this.story.childhood = back;
            this.workSettings = new RimWorld.Pawn_WorkSettings(this);

            this.def.race.corpseDef.fillPercent = 0.75f;
        }
    }
}
