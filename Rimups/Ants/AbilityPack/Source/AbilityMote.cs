using UnityEngine;
using Verse;

namespace AbilityPack
{
    public abstract class AbilityMote
    {
        public abstract void AbilityStarted(Saveable_Caster caster, Thing target);
    }

    //public abstract class AbilityMote : Mote
    //{
    //    public Thing owner;
    //    public float exactAlpha;
    //    public Color currentColor;

    //    public virtual void AbilityStarted(Thing owner) { this.owner = owner; }
    //    public virtual void AbilityCompleted(bool sucess) { this.Destroy(DestroyMode.Vanish); }

    //    public virtual UnityEngine.Vector3 UpdatePosition() { return this.owner.DrawPos; }

    //    public override void Draw()
    //    {
    //        this.exactPosition = this.UpdatePosition();
    //        this.exactPosition.y = Altitudes.AltitudeFor(this.def.altitudeLayer);

    //        Material material = MaterialPool.MatFrom(this.def.graphicPath, ShaderDatabase.Transparent, this.currentColor);
    //        material = AbilityMote.FadedVersionOf(material, this.exactAlpha);

    //        UnityEngine.Matrix4x4 matrix = default(UnityEngine.Matrix4x4);
    //        matrix.SetTRS(this.exactPosition, UnityEngine.Quaternion.AngleAxis(this.exactRotation, UnityEngine.Vector3.up), this.exactScale);
    //        UnityEngine.Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
    //    }
}
