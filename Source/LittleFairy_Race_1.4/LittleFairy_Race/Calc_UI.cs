using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LittleFairy_Race
{
	public static class Calc_UI
	{
		public static int CalcLv(int xp)
		{
			if (xp >= 15000000)
			{
				return 4;
			}
			if (xp >= 5000000)
			{
				return 3;
			}
			if (xp >= 1000000)
			{
				return 2;
			}
			return 1;
		}

		public static int NeedXP(int xp)
		{			
			if (xp < 1000000)
			{
				return 1000000 - xp;
			}
			if (xp < 5000000)
			{
				return 5000000 - xp;
			}
			if (xp < 15000000)
			{
				return 15000000 - xp;
			}
			return 0;
		}

		public static void DrawRadiusRing(IntVec3 center, float radius, Color color, bool nonwall, Func<IntVec3, bool> predicate = null)
		{
			if (radius > GenRadial.MaxRadialPatternRadius)
			{
				return;
			}
			List<IntVec3> ringDrawCells = new List<IntVec3>();
			ringDrawCells.Clear();
			int num = GenRadial.NumCellsInRadius(radius);
			for (int i = 0; i < num; i++)
			{
				IntVec3 intVec = center + GenRadial.RadialPattern[i];
				if (predicate == null || predicate(intVec))
				{
					if (nonwall || GenSight.LineOfSight(center, intVec, Find.CurrentMap))
					{
						ringDrawCells.Add(intVec);
					}
				}
			}
			GenDraw.DrawFieldEdges(ringDrawCells, color);
		}

		// スキルの説明文表示
		public static string ShowSkillState(SkillDef def, bool addPassive) {
			string output = "";
			// フレーバーテキスト

			// パッシブ付きならパッシブ
			if (addPassive)
            {
				if (def.SkillCategory != SkillCategory.Active)
                {
					output += "\n" + "LitF.UI.PassiveTitle".Translate();
					if (def.FPMax != 0)
					{
						output += "\n" + "LitF.UI.MaxFP".Translate() + ": " + def.FPMax.ToString("+###0;-###0");
					}
					if (def.FPHealRate != 0)
					{
						output += "\n" + "LitF.UI.FPHealRate".Translate() + ": " + def.FPHealRate.ToString("+###0;-###0");
					}
					if (def.CoolDownRate != 0)
					{
						output += "\n" + "LitF.UI.CoolDownRate".Translate() + ": " + (def.CoolDownRate * 100).ToString("+###0;-###0") + "%";
					}
					if (def.AddHediff != null)
					{
						IEnumerable<StatDrawEntry> entry = HediffStatsUtility.SpecialDisplayStats(def.AddHediff.stages.FirstOrFallback(), null);
						foreach (StatDrawEntry stat in entry)
						{
							output += "\n" + stat.LabelCap + ": " + stat.ValueString;
						}
					}
					if (def.PassiveDescription != "")
					{
						output += "\n" + def.PassiveDescription;
					}
				}
			}
			if (def.SkillCategory != SkillCategory.Passive)
			{
				if (def.description != null && def.description != "")
                {
					if (addPassive)
					{
						output += "\n" + "LitF.UI.ActiveTitle".Translate() + "\n" + def.description;
					}
					else
					{
						output += "\n" + def.description;
					}
					if (def.SkillStayTime > 0)
					{
						output += "\n" + "LitF.UI.SkillStayTime".Translate() + ": " + def.SkillStayTime.TicksToSeconds().ToString("F1");
					}
					if (def.SkillCoolDown > 0)
					{
						output += "\n" + "LitF.UI.SkillCoolDown".Translate() + ": " + def.SkillCoolDown.TicksToSeconds().ToString("F1");
					}
					if (def.TargetRange != -1f)
                    {
						output += "\n" + "LitF.UI.Range".Translate() + ": " + def.TargetRange;
					}
					if (def.TargetAreaRange != -1f)
					{
						output += "\n" + "LitF.UI.AreaRange".Translate() + ": " + def.TargetAreaRange;
					}
				}
			}
			return output;
		}
	}
}
