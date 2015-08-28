using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace AbilityPack
{
	public class ITab_Pawn_Ability : ITab
	{
		private static Texture2D manaColor = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.25f, 0.35f));

		private static Color abilityLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);

		public override bool IsVisible
		{
			get
			{
				Pawn pawn = this.HabilityPawn();
				if (pawn == null)
				{
					return false;
				}
				Saveable_Caster pawnHabilty = MapComponent_Ability.GetOrCreate().GetPawnHabilty(pawn);
				return this.ShowMana(pawnHabilty) || this.ShowAbility(pawnHabilty);
			}
		}

		public ITab_Pawn_Ability()
		{
			this.size = new Vector2(350f, 250f);
			this.labelKey = "AbilityPack.TabAbility";
		}

		private Pawn HabilityPawn()
		{
			Pawn pawn = base.SelThing as Pawn;
			if (pawn == null)
			{
				Corpse corpse = base.SelThing as Corpse;
				if (corpse != null)
				{
					pawn = corpse.innerPawn;
				}
			}
			return pawn;
		}

		private bool ShowMana(Saveable_Caster saveable)
		{
			return saveable.manaDef != null && saveable.manaDef.replenish.Visible(saveable);
		}

		private bool ShowAbility(Saveable_Caster saveable)
		{
			return (from i in saveable.executionLogs
			where i.isValid && i.ability.visible
			select i).Any<Saveable_ExecutionLog>();
		}

		protected override void FillTab()
		{
			Saveable_Caster pawnHabilty = MapComponent_Ability.GetOrCreate().GetPawnHabilty(this.HabilityPawn());
			float num = 10f;
			Text.Font = GameFont.Small;
			if (this.ShowMana(pawnHabilty))
			{
				Rect position = new Rect(10f, num, this.size.x - 20f - 30f, 22f);
				Text.Anchor = TextAnchor.MiddleLeft;
				GUI.DrawTexture(position, BaseContent.BlackTex);
				float manaValue = pawnHabilty.manaValue;
				GUI.DrawTexture(new Rect(position.xMin, position.yMin, position.width * manaValue / 100f, position.height), ITab_Pawn_Ability.manaColor);
				GUI.Label(new Rect(position.xMin + 5f, position.yMin, position.width - 10f, position.height), Translator.Translate("AbilityPack.Mana") + manaValue.ToString("N1"));
				num += 32f;
			}
			if (this.ShowAbility(pawnHabilty))
			{
				Rect position = new Rect(10f, num, this.size.x - 20f, this.size.y - 10f - num);
				GUI.color = ITab_Pawn_Ability.abilityLabelColor;
				Text.Anchor = TextAnchor.LowerLeft;
				Widgets.Label(new Rect(15f, num, this.size.x - 20f, 22f), Translator.Translate("AbilityPack.Abilities"));
				num += 22f;
				this.WriteLine(ref num, Translator.Translate("AbilityPack.ExecutionNumber"), Translator.Translate("AbilityPack.TicksSinseExecution"), Translator.Translate("AbilityPack.AbilityName"));
				foreach (Saveable_ExecutionLog current in from i in pawnHabilty.executionLogs
				where i.isValid && i.ability.visible
				select i)
				{
					this.WriteLine(ref num, current.numberOfExecution.ToString(), current.ticksSinceExecution.ToString(), current.ability.label);
				}
			}
		}

		public void WriteLine(ref float num, string count, string ticks, string name)
		{
			float num2 = 15f;
			float num3 = 40f;
			float num4 = num2 + num3 + 5f;
			float num5 = 60f;
			float num6 = num4 + num5 + 5f;
			float width = this.size.x - 10f - num6;
			Text.Anchor = TextAnchor.LowerRight;
			Widgets.Label(new Rect(num2, num, num3, 28f), count);
			Widgets.Label(new Rect(num4, num, num5, 28f), ticks);
			Text.Anchor = TextAnchor.LowerLeft;
			Widgets.Label(new Rect(num6, num, width, 28f), name);
			num += 28f;
		}
	}
}
