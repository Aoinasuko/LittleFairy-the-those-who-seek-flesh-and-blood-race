using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LittleFairy_Race
{
    public class LitF_ITab : ITab
    {
        private Texture2D deleteX;

        private static readonly Color ThingLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);

        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        public override bool IsVisible
        {
            get
            {
                return this.SelPawn.Faction != null && this.SelPawn.Faction.IsPlayer;
            }
        }

        private Comp_LitF comp
        {
            get
            {
                return this.SelPawn.GetComp<Comp_LitF>();
            }
        }

        public void LoadGraphics()
        {
            this.deleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
        }

        public LitF_ITab()
        {
            this.size = new Vector2(650f, 550f);
            this.labelKey = "LitF.UI.ITab";
            this.tutorTag = "LitF";
            LongEventHandler.ExecuteWhenFinished(LoadGraphics);
        }

        protected override void FillTab()
        {
            float SizeX = 500f;
            Text.Font = GameFont.Small;
            Rect rect3 = new Rect(0f, 5f, SizeX, this.size.y - 20f);
            Rect rect4 = new Rect(145f, 480f, 40f, 40f);
            Rect rectstat = new Rect(480f, 78f, 200f, 20f);

            GUI.BeginGroup(rect3);
            if (!comp.SkillStyles.NullOrEmpty() && comp.SkillStyles.Count > 0)
            {
                GUI.color = comp.SkillStyles.ElementAt(0).SkillColor;
                Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_back_1, 0.8f);
                GUI.color = Color.white;
                Widgets.DrawTextureFitted(rect4, Graphic_LitF.texture_skillstyle.TryGetValue(comp.SkillStyles.ElementAt(0).defName), 1.0f);
                TooltipHandler.TipRegion(rect4, comp.SkillStyles.ElementAt(0).label + "\n\n" + "LitF.UI.StartFP".Translate() + ":" + comp.SkillStyles.ElementAt(0).FPMax + "\n\n" + comp.SkillStyles.ElementAt(0).description);
                rect4.x += 60;
            }
            else
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_back_1, 0.8f);
                rect4.x += 60;
            }
            if (!comp.SkillStyles.NullOrEmpty() && comp.SkillStyles.Count > 1)
            {
                GUI.color = comp.SkillStyles.ElementAt(1).SkillColor;
                Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_back_2, 0.8f);
                GUI.color = Color.white;
                Widgets.DrawTextureFitted(rect4, Graphic_LitF.texture_skillstyle.TryGetValue(comp.SkillStyles.ElementAt(1).defName), 1.0f);
                TooltipHandler.TipRegion(rect4, comp.SkillStyles.ElementAt(1).label + "\n\n" + "LitF.UI.StartFP".Translate() + ":" + comp.SkillStyles.ElementAt(1).FPMax + "\n\n" + comp.SkillStyles.ElementAt(1).description);
                rect4.x += 60;
            }
            else
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_back_2, 0.8f);
            }
            if (!comp.SkillStyles.NullOrEmpty() && comp.SkillStyles.Count > 2)
            {
                GUI.color = comp.SkillStyles.ElementAt(2).SkillColor;
                Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_back_3, 0.8f);
                GUI.color = Color.white;
                Widgets.DrawTextureFitted(rect4, Graphic_LitF.texture_skillstyle.TryGetValue(comp.SkillStyles.ElementAt(2).defName), 1.0f);
                TooltipHandler.TipRegion(rect4, comp.SkillStyles.ElementAt(2).label + "\n\n" + "LitF.UI.StartFP".Translate() + ":" + comp.SkillStyles.ElementAt(2).FPMax + "\n\n" + comp.SkillStyles.ElementAt(2).description);
                rect4.x += 60;
            }
            else
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_back_3, 0.8f);
            }
            if (!comp.SkillStyles.NullOrEmpty() && comp.SkillStyles.Count > 3)
            {
                GUI.color = comp.SkillStyles.ElementAt(3).SkillColor;
                Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_back_4, 0.8f);
                GUI.color = Color.white;
                Widgets.DrawTextureFitted(rect4, Graphic_LitF.texture_skillstyle.TryGetValue(comp.SkillStyles.ElementAt(3).defName), 1.0f);
                TooltipHandler.TipRegion(rect4, comp.SkillStyles.ElementAt(3).label + "\n\n" + "LitF.UI.StartFP".Translate() + ":" + comp.SkillStyles.ElementAt(3).FPMax + "\n\n" + comp.SkillStyles.ElementAt(3).description);
                rect4.x += 60;
            }
            else
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_back_4, 0.8f);
            }
            GUI.color = Color.white;
            Widgets.DrawTextureFitted(rect3, Graphic_LitF.texture_tentacle, 0.8f);
            // スキルボタンの描画
            if (!comp.SkillStyles.NullOrEmpty() && comp.SkillStyles.Count > 0)
            {
                GUI.color = Color.white;
                Rect rect_skill_001 = new Rect(SizeX / 2 - 25f, (this.size.y - 50f) / 2, 40f, 40f);
                foreach (SkillDef skill in comp.SkillStyles.ElementAt(0).SkillDefs)
                {
                    rect_skill_001.x -= 55;
                    if (comp.SkillActive.TryGetValue(skill.defName, false))
                    {
                        Widgets.DrawTextureFitted(rect_skill_001, Graphic_LitF.texture_skill.TryGetValue(skill.defName), 1.0f);
                        TooltipHandler.TipRegion(rect_skill_001, skill.label + "\n\n" + Calc_UI.ShowSkillState(skill, true));
                    }
                    else
                    {
                        if (Widgets.ButtonImageFitted(rect_skill_001, Graphic_LitF.texture_lock, new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)))
                        {
                            if (skill.NeedXP <= comp.XP)
                            {
                                comp.GetSkill(skill);
                                comp.SetFPMax();
                                comp.CoolDownRate = comp.GetCoolDownRate();
                            }
                            else
                            {
                                Messages.Message("LitF.UI.NeedMoreXP".Translate(), null, MessageTypeDefOf.RejectInput, false);
                            }
                        }
                        TooltipHandler.TipRegion(rect_skill_001, skill.label + "\n\n" + "LitF.UI.NeedXP".Translate() + ":" + skill.NeedXP + "\n\n" + Calc_UI.ShowSkillState(skill, true));
                    }
                }
            }
            if (!comp.SkillStyles.NullOrEmpty() && comp.SkillStyles.Count > 1)
            {
                GUI.color = Color.white;
                Rect rect_skill_002 = new Rect(SizeX / 2 - 20f, (this.size.y - 58f) / 2, 40f, 40f);
                foreach (SkillDef skill in comp.SkillStyles.ElementAt(1).SkillDefs)
                {
                    rect_skill_002.y -= 55;
                    if (comp.SkillActive.TryGetValue(skill.defName, false))
                    {
                        Widgets.DrawTextureFitted(rect_skill_002, Graphic_LitF.texture_skill.TryGetValue(skill.defName), 1.0f);
                        TooltipHandler.TipRegion(rect_skill_002, skill.label + "\n\n" + Calc_UI.ShowSkillState(skill, true));
                    }
                    else
                    {
                        if (Widgets.ButtonImageFitted(rect_skill_002, Graphic_LitF.texture_lock, new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)))
                        {
                            if (skill.NeedXP <= comp.XP)
                            {
                                comp.GetSkill(skill);
                                comp.SetFPMax();
                                comp.CoolDownRate = comp.GetCoolDownRate();
                            }
                            else
                            {
                                Messages.Message("LitF.UI.NeedMoreXP".Translate(), null, MessageTypeDefOf.RejectInput, false);
                            }
                        }
                        TooltipHandler.TipRegion(rect_skill_002, skill.label + "\n\n" + "LitF.UI.NeedXP".Translate() + ":" + skill.NeedXP + "\n\n" + Calc_UI.ShowSkillState(skill, true));
                    }
                }
            }
            if (!comp.SkillStyles.NullOrEmpty() && comp.SkillStyles.Count > 2)
            {
                GUI.color = Color.white;
                Rect rect_skill_003 = new Rect(SizeX / 2 - 15f, (this.size.y - 50f) / 2, 40f, 40f);
                foreach (SkillDef skill in comp.SkillStyles.ElementAt(2).SkillDefs)
                {
                    rect_skill_003.x += 55;
                    if (comp.SkillActive.TryGetValue(skill.defName, false))
                    {
                        Widgets.DrawTextureFitted(rect_skill_003, Graphic_LitF.texture_skill.TryGetValue(skill.defName), 1.0f);
                        TooltipHandler.TipRegion(rect_skill_003, skill.label + "\n\n" + Calc_UI.ShowSkillState(skill, true));
                    }
                    else
                    {
                        if (Widgets.ButtonImageFitted(rect_skill_003, Graphic_LitF.texture_lock, new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)))
                        {
                            if (skill.NeedXP <= comp.XP)
                            {
                                comp.GetSkill(skill);
                                comp.SetFPMax();
                                comp.CoolDownRate = comp.GetCoolDownRate();
                            }
                            else
                            {
                                Messages.Message("LitF.UI.NeedMoreXP".Translate(), null, MessageTypeDefOf.RejectInput, false);
                            }
                        }
                        TooltipHandler.TipRegion(rect_skill_003, skill.label + "\n\n" + "LitF.UI.NeedXP".Translate() + ":" + skill.NeedXP + "\n\n" + Calc_UI.ShowSkillState(skill, true));
                    }
                }
            }
            if (!comp.SkillStyles.NullOrEmpty() && comp.SkillStyles.Count > 3)
            {
                GUI.color = Color.white;
                Rect rect_skill_004 = new Rect(SizeX / 2 - 20f, (this.size.y - 35f) / 2, 40f, 40f);
                foreach (SkillDef skill in comp.SkillStyles.ElementAt(3).SkillDefs)
                {
                    rect_skill_004.y += 55;
                    if (comp.SkillActive.TryGetValue(skill.defName, false))
                    {
                        Widgets.DrawTextureFitted(rect_skill_004, Graphic_LitF.texture_skill.TryGetValue(skill.defName), 1.0f);
                        TooltipHandler.TipRegion(rect_skill_004, skill.label + "\n\n" + Calc_UI.ShowSkillState(skill, true));
                    }
                    else
                    {
                        if (Widgets.ButtonImageFitted(rect_skill_004, Graphic_LitF.texture_lock, new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)))
                        {
                            if (skill.NeedXP <= comp.XP)
                            {
                                comp.GetSkill(skill);
                                comp.SetFPMax();
                                comp.CoolDownRate = comp.GetCoolDownRate();
                            }
                            else
                            {
                                Messages.Message("LitF.UI.NeedMoreXP".Translate(), null, MessageTypeDefOf.RejectInput, false);
                            }
                        }
                        TooltipHandler.TipRegion(rect_skill_004, skill.label + "\n\n" + "LitF.UI.NeedXP".Translate() + ":" + skill.NeedXP + "\n\n" + Calc_UI.ShowSkillState(skill, true));
                    }
                }
            }
            GUI.EndGroup();
            GUI.color = Color.white;
            Text.Font = GameFont.Tiny;
            GUI.Label(rectstat, "LitF.UI.MaxFP".Translate() + ":" + comp.MaxFP);
            rectstat.y += 16f;
            GUI.Label(rectstat, "LitF.UI.FPHealRate".Translate() + ":" + comp.GetHealRate());
            rectstat.y += 16f;
            GUI.Label(rectstat, "LitF.UI.XP".Translate() + ":" + comp.XP);
            rectstat.y += 32f;
            GUI.Label(rectstat, "LitF.UI.AddSkill".Translate());
            Rect rectadskill = new Rect(rectstat.x, rectstat.y + 16f, 40f, 40f);
            foreach (var skill in comp.CommonSkillDef.Select((Value, Index) => new { Value, Index }))
            {
                Widgets.DrawTextureFitted(rectadskill, Graphic_LitF.texture_skill.TryGetValue(skill.Value.defName), 1.0f);
                TooltipHandler.TipRegion(rectadskill, skill.Value.label + "\n\n" + Calc_UI.ShowSkillState(skill.Value, true));
                if ((skill.Index + 1) % 3 == 0)
                {
                    rectadskill.x -= 90f;
                    rectadskill.y += 40f;
                } else
                {
                    rectadskill.x += 45f;
                }
            }
        }
    }
}
