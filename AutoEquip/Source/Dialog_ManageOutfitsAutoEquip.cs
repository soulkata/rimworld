using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace AutoEquip
{
    public class Dialog_ManageOutfitsAutoEquip : Window
	{
		private const float TopAreaHeight = 40f;
		private const float TopButtonHeight = 35f;
		private const float TopButtonWidth = 150f;

		private Vector2 scrollPosition;
        private Vector2 scrollPositionStats;
		private Outfit selOutfitInt;

		private static ThingFilter apparelGlobalFilter;

		private static Regex validNameRegex = new Regex("^[a-zA-Z0-9 '\\-]*$");

		private Outfit SelectedOutfit
		{
			get
			{
				return this.selOutfitInt;
			}
			set
			{
				this.CheckSelectedOutfitHasName();
				this.selOutfitInt = value;
			}
		}

		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(700f, 700f);
			}
		}

        public Dialog_ManageOutfitsAutoEquip(Outfit selectedOutfit)
		{
			this.forcePause = true;
			this.doCloseX = true;
			this.closeOnEscapeKey = true;
			this.doCloseButton = true;
			this.closeOnClickedOutside = true;
			this.absorbInputAroundWindow = true;
            if (Dialog_ManageOutfitsAutoEquip.apparelGlobalFilter == null)
			{
                Dialog_ManageOutfitsAutoEquip.apparelGlobalFilter = new ThingFilter();
                Dialog_ManageOutfitsAutoEquip.apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
			}
			this.SelectedOutfit = selectedOutfit;
		}

		private void CheckSelectedOutfitHasName()
		{
			if (this.SelectedOutfit != null && this.SelectedOutfit.label.NullOrEmpty())
			{
				this.SelectedOutfit.label = "Unnamed";
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			float num = 0f;
			Rect rect = new Rect(0f, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.TextButton(rect, "SelectOutfit".Translate(), true, false))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (Outfit current in Find.Map.outfitDatabase.AllOutfits)
				{
					Outfit localOut = current;
					list.Add(new FloatMenuOption(localOut.label, delegate
					{
						this.SelectedOutfit = localOut;
					}, MenuOptionPriority.Medium, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			}
			num += 10f;
			rect = new Rect(num, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.TextButton(rect, "NewOutfit".Translate(), true, false))
			{
				this.SelectedOutfit = Find.Map.outfitDatabase.MakeNewOutfit();
			}
			num += 10f;
			rect = new Rect(num, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.TextButton(rect, "DeleteOutfit".Translate(), true, false))
			{
				List<FloatMenuOption> list2 = new List<FloatMenuOption>();
				foreach (Outfit current2 in Find.Map.outfitDatabase.AllOutfits)
				{
					Outfit localOut = current2;
					list2.Add(new FloatMenuOption(localOut.label, delegate
					{
						AcceptanceReport acceptanceReport = Find.Map.outfitDatabase.TryDelete(localOut);
                        if (!acceptanceReport.Accepted)
                        {
                            Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
                        }
                        else
                        {
                            if (localOut == this.SelectedOutfit)
                            {
                                this.SelectedOutfit = null;
                            }
                            foreach (Saveable_Outfit s in MapComponent_AutoEquip.Get.outfitCache.Where(i => i.outfit == localOut).ToArray())
                                MapComponent_AutoEquip.Get.outfitCache.Remove(s);
                        }                        
					}, MenuOptionPriority.Medium, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list2, false));
			}
			rect = new Rect(0f, 40f, 300f, inRect.height - 40f - this.CloseButSize.y).ContractedBy(10f);
			if (this.SelectedOutfit == null)
			{
				GUI.color = Color.grey;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect, "NoOutfitSelected".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
				return;
			}
			GUI.BeginGroup(rect);
			Rect rect1 = new Rect(0f, 0f, 200f, 30f);
            Dialog_ManageOutfitsAutoEquip.DoNameInputRect(rect1, ref this.SelectedOutfit.label, 30);
			rect1 = new Rect(0f, 40f, rect.width, rect.height - 45f - 10f);
            ThingFilterUI.DoThingFilterConfigWindow(rect1, ref this.scrollPosition, this.SelectedOutfit.filter, Dialog_ManageOutfitsAutoEquip.apparelGlobalFilter, 16);
            GUI.EndGroup();

            rect = new Rect(300f, 40f, inRect.width - 300f, inRect.height - 40f - this.CloseButSize.y).ContractedBy(10f);
            GUI.BeginGroup(rect);

            rect1 = new Rect(0f, 0f, rect.width, 30f);
            Widgets.LabelCheckbox(rect1, "AutoEquipAppendIndividualPawnStatus".Translate(), ref MapComponent_AutoEquip.Get.GetOutfit(this.SelectedOutfit).appendIndividualPawnStatus);

            rect1 = new Rect(0f, 40f, rect.width, rect.height - 45f - 10f);
            Dialog_ManageOutfitsAutoEquip.DoStatsInput(rect1, ref this.scrollPositionStats, MapComponent_AutoEquip.Get.GetOutfit(this.SelectedOutfit).stats);
            GUI.EndGroup();
		}

		public override void PreClose()
		{
			base.PreClose();
			this.CheckSelectedOutfitHasName();
		}

		public static void DoNameInputRect(Rect rect, ref string name, int maxLength)
		{
			string text = Widgets.TextField(rect, name);
            if (text.Length <= maxLength && Dialog_ManageOutfitsAutoEquip.validNameRegex.IsMatch(text))
			{
				name = text;
			}
		}

        public static void DoStatsInput(Rect rect, ref Vector2 scrollPosition, List<Saveable_StatDef> stats)
        {
            Widgets.DrawMenuSection(rect, true);
            Text.Font = GameFont.Tiny;
            float num = rect.width - 2f;
            Rect rect2 = new Rect(rect.x + 1f, rect.y + 1f, num / 2f, 24f);
            if (Widgets.TextButton(rect2, "ClearAll".Translate(), true, false))
            {
                MapComponent_AutoEquip.Get.nextOptimization = 0;
                stats.Clear();
            }

            rect.yMin = rect2.yMax;
            rect2 = new Rect(rect.x + 5f, rect.y + 1f, rect.width - 2f - 16f - 8f, 20f);

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(rect2, "-100%");

            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect2, "0%");

            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label(rect2, "100%");

            rect.yMin = rect2.yMax;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect position = new Rect(rect2.xMin + rect2.width / 2, rect.yMin + 5f, 1f, rect.height - 10f);
            GUI.DrawTexture(position, BaseContent.GreyTex);

            StatDef[] allStatDefs = DefDatabase<StatDef>.AllDefs
                .Where(i => 
                    (i.category.defName == "Apparel") ||
                    (i.category.defName == "PawnCombat") ||
                    (i.category.defName == "BasicsPawn") ||
                    (i.category.defName == "PawnSocial") ||
                    (i.category.defName == "PawnWork") ||
                    (i.category.defName == "Weapon") ||
                    (i.category.defName == "StuffStatFactors"))
                .ToArray();

            rect = rect.ContractedBy(2);
            Rect viewRect = new Rect(rect.xMin, rect.yMin, rect.width - 16f, allStatDefs.Length * Text.LineHeight * 1.2f + stats.Count * 40);
            
            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);

            foreach (StatDef stat in allStatDefs)
            {
                Rect itemRect = new Rect(viewRect.xMin, viewRect.yMin, viewRect.width, Text.LineHeight * 1.2f);
                if (Mouse.IsOver(itemRect))
                {
                    GUI.color = ITab_Pawn_AutoEquip.HighlightColor;
                    GUI.DrawTexture(itemRect, TexUI.HighlightTex);
                    GUI.color = Color.white;
                }

                TooltipHandler.TipRegion(itemRect, stat.description);

                Saveable_StatDef outfitStat = stats.FirstOrDefault(i => i.statDef == stat);
                bool active = outfitStat != null;
                Widgets.LabelCheckbox(itemRect, stat.LabelCap, ref active);

                viewRect.yMin += itemRect.height;

                if (active)
                {
                    if (outfitStat == null)
                    {
                        outfitStat = new Saveable_StatDef();
                        outfitStat.statDef = stat;
                        outfitStat.strength = 0;
                    }
                    if (!stats.Contains(outfitStat))
                    {
                        stats.Add(outfitStat);
                        MapComponent_AutoEquip.Get.nextOptimization = 0;
                    }

                    itemRect = new Rect(viewRect.xMin, viewRect.yMin, viewRect.width, 30f);                    
                    GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
                    float n = GUI.HorizontalSlider(itemRect, outfitStat.strength, -1f, 1f);
                    viewRect.yMin += 40f;

                    if (n != outfitStat.strength)
                    {
                        MapComponent_AutoEquip.Get.nextOptimization = 0;
                        outfitStat.strength = n;
                    }
                }
                else
                {
                    if (stats.Contains(outfitStat))
                    {
                        MapComponent_AutoEquip.Get.nextOptimization = 0;
                        stats.Remove(outfitStat);
                    }
                    outfitStat = null;
                }
            }

            Widgets.EndScrollView();
        }
	}
}
