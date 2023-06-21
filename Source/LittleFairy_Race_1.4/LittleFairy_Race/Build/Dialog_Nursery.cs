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
	public class Dialog_Nursery : Window
	{
		public override Vector2 InitialSize => new Vector2(700f, 700f);

		public Pawn pawn;

		public Thing_BloodNursery thing;

		public Dialog_Nursery(Thing_BloodNursery thing)
		{
			forcePause = true;
			doCloseX = true;
			absorbInputAroundWindow = true;
			closeOnAccept = false;
			closeOnClickedOutside = false;
			this.thing = thing;
		}

		public override void DoWindowContents(Rect inRect)
		{
			List<Thing> inner = thing.innerContainer.Where(x => x as Pawn != null && x.def.defName != "LittleFairy_Pawn").ToList();
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			inRect.y += 20f;
			Widgets.Label(inRect, "LitF.UI.Nursery_Lv".Translate() + ": " + Calc_UI.CalcLv(thing.XP));
			TooltipHandler.TipRegion(new Rect(inRect.x, inRect.y, inRect.width, 18), "LitF.UI.Tips_Nursery_Lv".Translate());
			inRect.y += 40f;
			Widgets.Label(inRect, "LitF.UI.Nursery_XP".Translate() + ": " + thing.XP);
			TooltipHandler.TipRegion(new Rect(inRect.x, inRect.y, inRect.width, 18), "LitF.UI.Tips_Nursery_XP".Translate());
			inRect.y += 20f;
			Widgets.Label(inRect, "LitF.UI.Nursery_NextLvXP".Translate() + ": " + Calc_UI.NeedXP(thing.XP));
			TooltipHandler.TipRegion(new Rect(inRect.x, inRect.y, inRect.width, 18), "LitF.UI.Tips_Nursery_NextLvXP".Translate());
			inRect.y += 40f;
			Widgets.Label(inRect, "LitF.UI.Nursery_NeedSkill".Translate());
			TooltipHandler.TipRegion(new Rect(inRect.x, inRect.y, inRect.width, 18), "LitF.UI.Tips_Nursery_NeedSkill".Translate());
			inRect.y += 20f;
			Rect rect_need = new Rect(inRect.x, inRect.y, 40, 40);
			if (Widgets.ButtonImageFitted(rect_need, thing.NeedSkill == null ? Graphic_LitF.texture_lock : Graphic_LitF.texture_skillstyle.TryGetValue(thing.NeedSkill.defName), new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)))
			{
				if (Calc_UI.CalcLv(thing.XP) < 2)
                {
					Messages.Message("LitF.UI.NeedMoreLv".Translate(), null, MessageTypeDefOf.RejectInput, false);
				} else
                {
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					List<SkillStyleDef> skillsel = DefDatabase<SkillStyleDef>.AllDefsListForReading.Where(x => x.TargetPawn.EnumerableNullOrEmpty() && x.CanGeneRandom).ToList();
					// 苗床にしている種族の可能性を追加
					if (!inner.EnumerableNullOrEmpty())
					{
						foreach (Pawn pawn in inner)
						{
							// 追加出現の可能性があるスキル
							List<SkillStyleDef> skillstyles = DefDatabase<SkillStyleDef>.AllDefs.Where(x => x.CanGeneRandom && x.TargetPawn.Contains(pawn.def.defName)).ToList();
							if (!skillstyles.EnumerableNullOrEmpty())
							{
								foreach (SkillStyleDef addstyle in skillstyles)
								{
									if (!skillsel.Contains(addstyle))
									{
										skillsel.Add(addstyle);
									}
								}
							}
						}
					}
					foreach (SkillStyleDef skillstyle in skillsel)
					{
						FloatMenuOption item = new FloatMenuOption(skillstyle.label, delegate
						{
							thing.NeedSkill = skillstyle;
						});
						list.Add(item);
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			}
			if (thing.NeedSkill != null)
            {
				TooltipHandler.TipRegion(rect_need, thing.NeedSkill.label + "\n\n" + "LitF.UI.StartFP".Translate() + ":" + thing.NeedSkill.FPMax + "\n\n" + thing.NeedSkill.description);
			}
			inRect.y += 60f;
			Widgets.Label(inRect, "LitF.UI.Nursery_AddSkill".Translate());
			TooltipHandler.TipRegion(new Rect(inRect.x, inRect.y, inRect.width, 18), "LitF.UI.Tips_Nursery_AddSkill".Translate());
			if (!inner.EnumerableNullOrEmpty())
            {
				foreach (Pawn pawn in inner)
				{
					inRect.y += 20f;
					Widgets.Label(inRect, pawn.Name + " : " + pawn.def.label);
					inRect.y += 20f;
					// 追加出現の可能性があるスキル
					List<SkillStyleDef> skillstyles = DefDatabase<SkillStyleDef>.AllDefs.Where(x => x.CanGeneRandom && x.TargetPawn.Contains(pawn.def.defName)).ToList();
					Rect rect = new Rect(inRect.x, inRect.y, 40, 40);
					foreach (var skill in skillstyles.Select((Value, Index) => new { Value, Index }))
					{
						Widgets.DrawTextureFitted(rect, Graphic_LitF.texture_skillstyle.TryGetValue(skill.Value.defName), 1.0f);
						TooltipHandler.TipRegion(rect, skill.Value.label + "\n\n" + "LitF.UI.StartFP".Translate() + ":" + skill.Value.FPMax + "\n\n" + skill.Value.description);
						rect.x += 45f;
					}
					List<SkillDef> addskills = DefDatabase<SkillDef>.AllDefs.Where(x => x.TargetPawn.Contains(pawn.def.defName)).ToList();
					foreach (var skill in addskills.Select((Value, Index) => new { Value, Index }))
					{
						Widgets.DrawTextureFitted(rect, Graphic_LitF.texture_skill.TryGetValue(skill.Value.defName), 1.0f);
						TooltipHandler.TipRegion(rect, skill.Value.label + "\n\n" + Calc_UI.ShowSkillState(skill.Value, true));
						rect.x += 45f;
					}
					inRect.y += 50f;
				}
			}
		}
	}



}
