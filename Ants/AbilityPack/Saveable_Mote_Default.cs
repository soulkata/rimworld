using System;
using UnityEngine;
using Verse;

namespace AbilityPack
{
	public class Saveable_Mote_Default : Saveable_Mote
	{
		public Thing target;

		public float alphaChange;

		public Vector3 exactPosition;

		public float exactAlpha;

		public float exactRotation;

		public Vector3 exactScale;

		public Color currentColor;

		public override void InitializeMote(ref Thing mote)
		{
			if (mote == null)
			{
				ThingDef named = DefDatabase<ThingDef>.GetNamed("AbilityMote_Default", false);
				mote = ThingMaker.MakeThing(named, null);
				((ThingAbilityMote_Default)mote).saveable = this;
				GenSpawn.Spawn(mote, this.target.Position);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.LookReference<Thing>(ref this.target, "target");
			Scribe_Values.LookValue<float>(ref this.alphaChange, "alphaChange", 0f, false);
			Scribe_Values.LookValue<Color>(ref this.currentColor, "color", default(Color), false);
			Scribe_Values.LookValue<Vector3>(ref this.exactPosition, "position", default(Vector3), false);
			Scribe_Values.LookValue<float>(ref this.exactAlpha, "alpha", 0f, false);
			Scribe_Values.LookValue<float>(ref this.exactRotation, "rotation", 0f, false);
			Scribe_Values.LookValue<Vector3>(ref this.exactScale, "scale", default(Vector3), false);
		}

		public override bool Tick()
		{
			this.exactAlpha = Mathf.Clamp(this.exactAlpha + this.alphaChange, 0f, 1f);
			if (this.exactAlpha == 0f)
			{
				if (this.mote != null)
				{
					if (!this.mote.Destroyed)
					{
						this.mote.Destroy(0);
					}
					this.mote = null;
				}
				return false;
			}
			if (this.target != null)
			{
				this.exactPosition = this.target.DrawPos;
				this.exactPosition.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
			}
			this.exactRotation += 3f;
			return true;
		}

		public override void Completed(Saveable_Caster caster, bool sucess)
		{
			this.exactAlpha = 1f;
			this.alphaChange = -0.015625f;
			if (sucess)
			{
				this.currentColor = Color.green;
				return;
			}
			this.currentColor = Color.red;
		}
	}
}
