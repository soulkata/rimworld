using System;
using UnityEngine;
using Verse;

namespace AbilityPack
{
	public class ThingAbilityMote_Default : Mote
	{
		public Saveable_Mote_Default saveable;

		public override void Tick()
		{
			this.exactPosition = this.saveable.exactPosition;
			this.exactRotation = this.saveable.exactRotation;
			this.exactScale = this.saveable.exactScale;
		}

		public override void RealtimeUpdate()
		{
		}

		public override void Draw()
		{
			Material material = MaterialPool.MatFrom(this.def.graphicData.texPath, ShaderDatabase.Transparent, this.saveable.currentColor);
			material = ThingAbilityMote_Default.FadedVersionOf(material, this.saveable.exactAlpha);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(this.exactPosition, Quaternion.AngleAxis(this.exactRotation, Vector3.up), this.exactScale);
			Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
		}

		public static Material FadedVersionOf(Material sourceMat, float alpha)
		{
			if (sourceMat.color == Color.white)
			{
				return FadedMaterialPool.FadedVersionOf(sourceMat, alpha);
			}
			if (Debug.isDebugBuild && !sourceMat.HasProperty("_Color"))
			{
				return sourceMat;
			}
			if (ThingAbilityMote_Default.IndexFromAlpha(alpha) == 0)
			{
				return BaseContent.ClearMat;
			}
			return new Material(sourceMat)
			{
				color = new Color(sourceMat.color.r, sourceMat.color.g, sourceMat.color.b, (float)ThingAbilityMote_Default.IndexFromAlpha(alpha) / 100f)
			};
		}

		private static int IndexFromAlpha(float alpha)
		{
			int num = Mathf.FloorToInt(alpha * 100f);
			if (num == 100)
			{
				num = 99;
			}
			return num;
		}
	}
}
