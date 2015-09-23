using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AutoEquip
{
    public class Dialog_PawnApparelDetail : Window
    {
        private Pawn pawn;
        private Apparel apparel;

        public Dialog_PawnApparelDetail(Pawn pawn, Apparel apparel)
        {
            this.doCloseX = true;
            this.closeOnEscapeKey = true;
            this.doCloseButton = true;
            
            this.pawn = pawn;
            this.apparel = apparel;
        }

        public override Vector2 InitialWindowSize
        {
            get
            {
                return new Vector2(700f, 700f);
            }
        }

        private Vector2 scrollPosition;

        public override void DoWindowContents(Rect windowRect)
        {
            Saveable_PawnNextApparelConfiguration conf = MapComponent_AutoEquip.Get.GetCache(this.pawn);

            Rect groupRect = windowRect.ContractedBy(10f);
            groupRect.height -= 100;
            GUI.BeginGroup(groupRect);            

            float baseValue = 100f;
            float multiplierWidth = 100f;
            float finalValue = 120f;
            float labelWidth = groupRect.width - baseValue - multiplierWidth - finalValue - 8f - 8f;

            Rect itemRect = new Rect(groupRect.xMin + 4f, groupRect.yMin, groupRect.width - 8f, Text.LineHeight * 1.2f);

            this.DrawLine(ref itemRect,
                "Status", labelWidth,
                "Base", baseValue,
                "Strengh", multiplierWidth,
                "Final", finalValue);            

            groupRect.yMin += itemRect.height;
            Widgets.DrawLineHorizontal(groupRect.xMin, groupRect.yMin, groupRect.width);
            groupRect.yMin += 4f;
            groupRect.height -= 4f;
            groupRect.height -= Text.LineHeight * 1.2f * 3f;

            Rect viewRect = new Rect(groupRect.xMin, groupRect.yMin, groupRect.width - 16f, conf.calculedStatDef.Count * Text.LineHeight * 1.2f + 16f);
            if (viewRect.height < groupRect.height)
                groupRect.height = viewRect.height;

            Rect listRect = viewRect.ContractedBy(4f);

            Widgets.BeginScrollView(groupRect, ref scrollPosition, viewRect);

            foreach (Saveable_Outfit_StatDef stat in conf.calculedStatDef)
            {
                itemRect = new Rect(listRect.xMin, listRect.yMin, listRect.width, Text.LineHeight * 1.2f);
                if (Mouse.IsOver(itemRect))
                {
                    GUI.color = ITab_Pawn_AutoEquip.HighlightColor;
                    GUI.DrawTexture(itemRect, TexUI.HighlightTex);
                    GUI.color = Color.white;
                }

                this.DrawLine(ref itemRect,
                    stat.statDef.label, labelWidth, 
                    conf.GetStatValue(apparel, stat).ToString("N3"), baseValue, 
                    stat.strength.ToString("N2"), multiplierWidth, 
                    (conf.GetStatValue(this.apparel, stat) * stat.strength).ToString("N5"), finalValue);

                listRect.yMin = itemRect.yMax;
            }

            Widgets.EndScrollView();

            Widgets.DrawLineHorizontal(groupRect.xMin, groupRect.yMax, groupRect.width);

            itemRect = new Rect(listRect.xMin, groupRect.yMax, listRect.width, Text.LineHeight * 1.2f);
            this.DrawLine(ref itemRect,
                "Average", labelWidth,
                conf.calculedStatDef.Average(i => conf.GetStatValue(apparel, i)).ToString("N3"), baseValue,
                "", multiplierWidth,
                conf.ApparelScoreRawStats(apparel).ToString("N5"), finalValue);

            itemRect = new Rect(listRect.xMin, itemRect.yMax, listRect.width, Text.LineHeight * 1.2f);
            this.DrawLine(ref itemRect,
                "Hit Points", labelWidth,
                JobGiver_OptimizeApparelAutoEquip.ApparelScoreRawHitPointAjust(apparel).ToString("N3"), baseValue,
                "", multiplierWidth,
                (conf.ApparelScoreRawStats(apparel) * JobGiver_OptimizeApparelAutoEquip.ApparelScoreRawHitPointAjust(apparel)).ToString("N5"), finalValue);

            itemRect = new Rect(listRect.xMin, itemRect.yMax, listRect.width, Text.LineHeight * 1.2f);
            this.DrawLine(ref itemRect,
                "Temperature", labelWidth,
                conf.ApparalScoreRawInsulationColdAjust(apparel).ToString("N3"), baseValue,
                "", multiplierWidth,
                conf.ApparelScoreRaw(apparel).ToString("N5"), finalValue);

            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
        }

        private void DrawLine(ref Rect itemRect,
            string statDefLabelText, float statDefLabelWidth,
            string statDefValueText, float statDefValueWidth,
            string multiplierText, float multiplierWidth,
            string finalValueText, float finalValueWidth)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(new Rect(itemRect.xMin, itemRect.yMin, statDefLabelWidth, itemRect.height), statDefLabelText);
            itemRect.xMin += statDefLabelWidth;

            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label(new Rect(itemRect.xMin, itemRect.yMin, statDefValueWidth, itemRect.height), statDefValueText);
            itemRect.xMin += statDefValueWidth;

            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label(new Rect(itemRect.xMin, itemRect.yMin, multiplierWidth, itemRect.height), multiplierText);
            itemRect.xMin += multiplierWidth;

            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label(new Rect(itemRect.xMin, itemRect.yMin, finalValueWidth, itemRect.height), finalValueText);
            itemRect.xMin += finalValueWidth;
        }
    }
}
