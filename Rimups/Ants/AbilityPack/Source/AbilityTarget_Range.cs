using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
    public class AbilityTarget_Range : AbilityTarget
    {
        public float minimumRange;
        public float maximumRange;
        public bool closestFirst = false;
        public AbilityTarget target;

        public override IEnumerable<Thing> Targets(AbilityDef ability, Saveable_Caster saveablePawn)
        {
            float maximumSquared = this.maximumRange * this.maximumRange;
            float minimumSquared = this.minimumRange * this.minimumRange;

            if (closestFirst)
            {
                List<KeyValuePair<float, Thing>> ranges = new List<KeyValuePair<float, Thing>>();                
                
                foreach (Thing thing in this.target.Targets(ability, saveablePawn))
                {
                    float lengthHorizontalSquared = (saveablePawn.pawn.Position - thing.Position).LengthHorizontalSquared;

                    if ((maximumSquared > 0) &&
                        (maximumSquared < lengthHorizontalSquared))
                        continue;

                    if ((minimumSquared > 0) &&
                        (minimumSquared > lengthHorizontalSquared))
                        continue;

                    ranges.Add(new KeyValuePair<float, Thing>(lengthHorizontalSquared, thing));
                }
                ranges.Sort((x, y) => x.Key.CompareTo(y.Key));
                foreach (KeyValuePair<float, Thing> t in ranges)
                    yield return t.Value;
            }
            else
            {
                foreach (Thing thing in this.target.Targets(ability, saveablePawn))
                {
                    float lengthHorizontalSquared = (saveablePawn.pawn.Position - thing.Position).LengthHorizontalSquared;

                    if ((maximumSquared > 0) &&
                        (maximumSquared < lengthHorizontalSquared))
                        continue;

                    if ((minimumSquared > 0) &&
                        (minimumSquared > lengthHorizontalSquared))
                        continue;

                    yield return thing;
                }
            }
        }
    }
}
