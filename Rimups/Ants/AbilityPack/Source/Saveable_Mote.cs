using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AbilityPack
{
    public abstract class Saveable_Mote : Saveable
    {
        public Thing mote;

        public virtual void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                this.InitializeMote(ref this.mote);
        }

        public abstract void InitializeMote(ref Thing mote);
        public abstract bool Tick();
        public abstract void Completed(Saveable_Caster caster, bool sucess);
    }
}
