using System.Collections.Generic;
using Verse;

namespace AbilityPack
{
    public class AbilityDef : Def
    {
        public bool visible = true;
        public List<ThingDef> races;
        public AbilityRequeriment validity;
        public AbilityRequeriment requeriment;
        public AbilityEffect effect;
        public AbilityPriority priority;
        public AbilityTarget target;
    }
}
