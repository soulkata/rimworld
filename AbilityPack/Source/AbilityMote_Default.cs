using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AbilityPack
{
    public class AbilityMote_Default : AbilityMote
    {
        public Color color = Color.white;
        
        public override void AbilityStarted(Saveable_Caster caster, Thing target)
        {
            Saveable_Mote_Default mote = new Saveable_Mote_Default();
            mote.target = target;
            mote.currentColor = this.color;
            mote.alphaChange = 0.0f;
            mote.exactAlpha = 0.5f;
            mote.exactPosition = target.DrawPos;
            mote.exactScale = new Vector3(1, 1, 1);
            caster.AddMote(mote);
        }
    }

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
                ThingDef thing = DefDatabase<ThingDef>.GetNamed("AbilityMote_Default", false);
                mote = ThingMaker.MakeThing(thing);
                ((ThingAbilityMote_Default)mote).saveable = this;
                GenSpawn.Spawn(mote, this.target.Position);
            }
        }
        
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.LookReference(ref this.target, "target");
            Scribe_Values.LookValue(ref this.alphaChange, "alphaChange");
            Scribe_Values.LookValue(ref this.currentColor, "color");
            Scribe_Values.LookValue(ref this.exactPosition, "position");
            Scribe_Values.LookValue(ref this.exactAlpha, "alpha");
            Scribe_Values.LookValue(ref this.exactRotation, "rotation");
            Scribe_Values.LookValue(ref this.exactScale, "scale");
        }

        public override bool Tick()
        {
            this.exactAlpha = Mathf.Clamp(this.exactAlpha + this.alphaChange, 0.0f, 1.0f);
            if (this.exactAlpha == 0)
            {
                if (this.terminated != null)
                {
                    this.terminated(this);
                    this.terminated = null;                        
                }
                if (this.mote != null)
                {                    
                    if (!this.mote.Destroyed)
                        this.mote.Destroy();
                    this.mote = null;
                }
                return false;
            }
            else
            {
                if (this.target != null)
                {
                    this.exactPosition = this.target.DrawPos;
                    this.exactPosition.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
                }
                this.exactRotation += 3;
                return true;
            }
        }

        private Action<Saveable_Mote> terminated;

        public override void Completed(Saveable_Caster caster, bool sucess, Action<Saveable_Mote> terminated)
        {
            this.terminated = terminated;
            this.exactAlpha = 1.0f;
            this.alphaChange = -1.0f / 64.0f;
            if (sucess)
                this.currentColor = Color.green;
            else
                this.currentColor = Color.red;
        }
    }

    public class ThingAbilityMote_Default : Mote
    {
        public Saveable_Mote_Default saveable;

        public override void Tick()
        {
            this.exactPosition = this.saveable.exactPosition;
            this.exactRotation = this.saveable.exactRotation;
            this.exactScale = this.saveable.exactScale;
        }

        public override void RealtimeUpdate() { }

        public override void Draw()
        {
            Material material = MaterialPool.MatFrom(this.def.graphicData.texPath, ShaderDatabase.Transparent, this.saveable.currentColor);
            material = ThingAbilityMote_Default.FadedVersionOf(material, this.saveable.exactAlpha);

            UnityEngine.Matrix4x4 matrix = default(UnityEngine.Matrix4x4);
            matrix.SetTRS(this.exactPosition, UnityEngine.Quaternion.AngleAxis(this.exactRotation, UnityEngine.Vector3.up), this.exactScale);
            UnityEngine.Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
        }

        public static Material FadedVersionOf(Material sourceMat, float alpha)
        {
            if (sourceMat.color == Color.white)
                return FadedMaterialPool.FadedVersionOf(sourceMat, alpha);

            if (Debug.isDebugBuild && !sourceMat.HasProperty("_Color"))
                return sourceMat;
            int num = ThingAbilityMote_Default.IndexFromAlpha(alpha);
            if (num == 0)
                return BaseContent.ClearMat;

            Material material;
            material = new Material(sourceMat);
            material.color = new Color(sourceMat.color.r, sourceMat.color.g, sourceMat.color.b, (float)ThingAbilityMote_Default.IndexFromAlpha(alpha) / 100f);
            return material;
        }

        private static int IndexFromAlpha(float alpha)
        {
            int num = Mathf.FloorToInt(alpha * 100f);
            if (num == 100)
                num = 99;
            return num;
        }
    }
}
