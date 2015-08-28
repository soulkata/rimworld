using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AbilityPack
{
    public class ManaDef : Def
    {
        public List<ThingDef> races;
        public float initial;
        public ManaReplenish replenish;
    }
}
