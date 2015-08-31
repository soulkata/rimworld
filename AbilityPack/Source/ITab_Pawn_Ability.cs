using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AbilityPack
{
    public class ITab_Pawn_Ability : ITab
    {        
        private static Texture2D manaColor = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.25f, 0.35f));
        private static Color abilityLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);

        public ITab_Pawn_Ability()
        {
            this.size = new Vector2(350f, 250f);
            this.labelKey = "AbilityPack.TabAbility";
        }

        public override bool IsVisible
        {
            get
            {
                Pawn p = HabilityPawn();

                if (p == null)
                    return false;

                Saveable_Caster saveable = MapComponent_Ability.GetOrCreate().GetPawnHabilty(p);

                return this.ShowMana(saveable) || this.ShowAbility(saveable);
            }
        }

        private Pawn HabilityPawn()
        {
            Pawn p = this.SelThing as Pawn;
            if (p == null)
            {
                Corpse c = this.SelThing as Corpse;
                if (c != null)
                    p = c.innerPawn;
            }
            return p;
        }

        private bool ShowMana(Saveable_Caster saveable)
        {
            return (saveable.manaDef != null) &&
                (saveable.manaDef.replenish.Visible(saveable));
        }

        private bool ShowAbility(Saveable_Caster saveable) { return saveable.executionLogs.Where(i => i.isValid && i.ability.visible).Any(); }

        protected override void FillTab()
        {
            Saveable_Caster pawn = MapComponent_Ability.GetOrCreate().GetPawnHabilty(this.HabilityPawn());
            float num = 10;
            Rect rect;

            Text.Font = GameFont.Small;

            if (this.ShowMana(pawn))
            {
                rect = new Rect(10, num, this.size.x - 20 - 30, 22);
                Text.Anchor = TextAnchor.MiddleLeft;
                GUI.DrawTexture(rect, BaseContent.BlackTex);
                float mana = pawn.manaValue;
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width * mana / 100, rect.height), ITab_Pawn_Ability.manaColor);
                GUI.Label(new Rect(rect.xMin + 5, rect.yMin, rect.width - 10, rect.height), "AbilityPack.Mana".Translate() + mana.ToString("N1"));
                num += 22 + 10;
            }

            if (this.ShowAbility(pawn))
            {
                rect = new Rect(10, num, this.size.x - 20, this.size.y - 10 - num);

                GUI.color = abilityLabelColor;

                Text.Anchor = TextAnchor.LowerLeft;
                Widgets.Label(new Rect(15, num, this.size.x - 20, 22f), "AbilityPack.Abilities".Translate());
                num += 22;
                this.WriteLine(ref num, "AbilityPack.ExecutionNumber".Translate(), "AbilityPack.TicksSinseExecution".Translate(), "AbilityPack.AbilityName".Translate());

                foreach (Saveable_ExecutionLog log in pawn.executionLogs.Where(i => i.isValid && i.ability.visible))
                    this.WriteLine(ref num, log.numberOfExecution.ToString(), log.ticksSinceExecution.ToString(), log.ability.label);

                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        public void WriteLine(ref float num, string count, string ticks, string name)
        {
            float countLeft = 15;
            float countWidth = 40;
            float ticksLeft = countLeft + countWidth + 5;
            float ticksWidth = 60;
            float nameLeft = ticksLeft + ticksWidth + 5;
            float nameWidth = this.size.x - 10 - nameLeft;

            Text.Anchor = TextAnchor.LowerRight;
            Widgets.Label(new Rect(countLeft, num, countWidth, 28f), count);
            Widgets.Label(new Rect(ticksLeft, num, ticksWidth, 28f), ticks);

            Text.Anchor = TextAnchor.LowerLeft;
            Widgets.Label(new Rect(nameLeft, num, nameWidth, 28f), name);
            num += 28;
        }
    }
}
