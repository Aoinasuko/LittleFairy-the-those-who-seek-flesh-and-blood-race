using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace LittleFairy_Race
{
	[StaticConstructorOnStartup]
	public class Gizmo_FPStatus : Gizmo
	{
		public Comp_LitF comp;

		private static readonly Texture2D FullNPBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f));

		private static readonly Texture2D EmptyNPBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

		public Gizmo_FPStatus()
		{
			Order = -100f;
		}

		public override float GetWidth(float maxWidth)
		{
			return 140f;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			Rect rect3 = rect2;
			rect3.height = rect.height / 2f;
			Text.Font = GameFont.Tiny;
			Widgets.Label(rect3, "LitF.UI.FP".Translate());
			Rect rect4 = rect2;
			rect4.yMin = rect2.y + rect2.height / 2f;
			float fillPercent = comp.NowFP / Mathf.Max(1f, comp.MaxFP);
			Widgets.FillableBar(rect4, fillPercent, FullNPBarTex, EmptyNPBarTex, doBorder: false);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect4, comp.NowFP.ToString() + "/" + comp.MaxFP.ToString());
			Text.Anchor = TextAnchor.UpperLeft;
			return new GizmoResult(GizmoState.Clear);
		}

	}

	public class CompProperties_LitF : CompProperties
	{
		public CompProperties_LitF()
		{
			compClass = typeof(Comp_LitF);
		}
	}

	public class Comp_LitF : ThingComp
	{
		private List<String> key;
		private List<bool> val;
		private List<String> key_cool;
		private List<int> val_int;

		// 最大FP
		public int MaxFP = 0;
		// 現在FP
		public int NowFP = 0;
		// クールダウンの減少量
		public int CoolDownRate = 1;
		// 所持経験値
		public int XP = 0;
		// 所持しているスキル派生
		public List<SkillStyleDef> SkillStyles = new List<SkillStyleDef>();
		// スキルが解放済みか？
		public Dictionary<String, bool> SkillActive = new Dictionary<String, bool>();
		// 所持しているスキルのクールダウン
		public Dictionary<String,int> SkillCoolDown = new Dictionary<String, int>();
		// 所持している追加スキル
		public List<SkillDef> CommonSkillDef = new List<SkillDef>();
		// 使用中のスキル
		public SkillDef UseSkill = null;
		// 最初のスポーン?
		public bool FirstSpawn = false;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref MaxFP, "MaxFP");
			Scribe_Values.Look(ref NowFP, "NowFP");
			Scribe_Values.Look(ref CoolDownRate, "CoolDownRate");
			Scribe_Values.Look(ref XP, "XP");
			Scribe_Collections.Look(ref SkillStyles, "SkillStyles");
			Scribe_Collections.Look(ref SkillActive, "SkillActive", LookMode.Value, LookMode.Value, ref key, ref val);
			Scribe_Collections.Look(ref SkillCoolDown, "SkillCoolDown", LookMode.Value, LookMode.Value, ref key_cool, ref val_int);
			Scribe_Collections.Look(ref CommonSkillDef, "CommonSkillDef");
			Scribe_Defs.Look(ref UseSkill, "UseSkill");
			Scribe_Values.Look(ref FirstSpawn, "FirstSpawn");
		}

		// FP最大値のセット
		public void SetFPMax()
        {
			int Cal_FPMax = 0;
			foreach (SkillStyleDef skillStyle in SkillStyles)
			{
				Cal_FPMax += skillStyle.FPMax;
				foreach (SkillDef skilldef in skillStyle.SkillDefs)
				{
					if (SkillActive.TryGetValue(skilldef.defName, false))
                    {
						Cal_FPMax += skilldef.FPMax;
					}
				}
			}
			foreach (SkillDef skilldef in CommonSkillDef)
			{
				Cal_FPMax += skilldef.FPMax;
			}
			MaxFP = Math.Max(Cal_FPMax, 1);
		}

		public int GetHealRate()
		{
			Pawn pawn = (Pawn)this.parent;
			int FPHealRate = 0;
			// スキルスタイル
			foreach (SkillStyleDef skillStyle in SkillStyles)
			{
				foreach (SkillDef skilldef in skillStyle.SkillDefs)
				{
					if (SkillActive.TryGetValue(skilldef.defName, false))
					{
						FPHealRate += skilldef.FPHealRate;
					}
				}
			}
			// 追加スキル
			foreach (SkillDef skilldef in CommonSkillDef)
			{
				FPHealRate += skilldef.FPHealRate;
			}
			return Math.Max(FPHealRate + 1, 0);
		}

		public int GetCoolDownRate()
		{
			Pawn pawn = (Pawn)this.parent;
			int CoolDownRate = 0;
			// スキルスタイル
			foreach (SkillStyleDef skillStyle in SkillStyles)
			{
				foreach (SkillDef skilldef in skillStyle.SkillDefs)
				{
					if (SkillActive.TryGetValue(skilldef.defName, false))
					{
						CoolDownRate += skilldef.CoolDownRate;
					}
				}
			}
			// 追加スキル
			foreach (SkillDef skilldef in CommonSkillDef)
			{
				CoolDownRate += skilldef.CoolDownRate;
			}
			return CoolDownRate + 1;
		}

		// FP回復
		public void FPHeal(int val)
		{
			NowFP = Math.Min(MaxFP, NowFP + val);
		}

		// スキル獲得
		public void GetSkill(SkillDef skill)
		{
			Pawn pawn = (Pawn)this.parent;
			XP -= skill.NeedXP;
			SkillActive.Add(skill.defName, true);
			if (skill.AddHediff != null)
            {
				pawn.health.AddHediff(skill.AddHediff);
			}
		}

		public override void CompTick()
		{
			Pawn pawn = (Pawn)this.parent;
			// Tickごとの処理(スキルスタイル)
			foreach (SkillStyleDef skillStyle in SkillStyles)
			{
				foreach (SkillDef skilldef in skillStyle.SkillDefs)
				{
					if (skilldef.SkillCategory == SkillCategory.Active || skilldef.SkillCategory == SkillCategory.Both)
					{
						int cooldown = SkillCoolDown.TryGetValue(skilldef.defName, 0);
						if (cooldown > 0)
						{
							cooldown = Math.Max(0, cooldown - CoolDownRate);
						}
						SkillCoolDown.SetOrAdd(skilldef.defName, cooldown);
					}
					if (skilldef.SkillCategory == SkillCategory.Active)
					{
						continue;
                    }
					if (SkillActive.TryGetValue(skilldef.defName, false) == false)
					{
						continue;
					}
					if (skilldef.SkillCalc != null)
                    {
						skilldef.SkillCalc.Tick(pawn);
					}
				}
			}
			// Tickごとの処理(追加スキル)
			foreach (SkillDef skilldef in CommonSkillDef)
			{
				if (skilldef.SkillCategory == SkillCategory.Active || skilldef.SkillCategory == SkillCategory.Both)
				{
					int cooldown = SkillCoolDown.TryGetValue(skilldef.defName, 0);
					if (cooldown > 0)
					{
						cooldown--;
					}
					SkillCoolDown.SetOrAdd(skilldef.defName, cooldown);
				}
				if (skilldef.SkillCategory == SkillCategory.Active)
				{
					continue;
				}
				if (SkillActive.TryGetValue(skilldef.defName, false) == false)
				{
					continue;
				}
				if (skilldef.SkillCalc != null)
				{
					skilldef.SkillCalc.Tick(pawn);
				}
			}
			// 1時間ごとの処理
			if (parent.IsHashIntervalTick(2500))
            {
				// Hediffの監視
				// スキルスタイル
				foreach (SkillStyleDef skillStyle in SkillStyles)
				{
					foreach (SkillDef skilldef in skillStyle.SkillDefs)
					{
						if (skilldef.SkillCategory == SkillCategory.Active)
						{
							continue;
						}
						if (SkillActive.TryGetValue(skilldef.defName, false) == false)
                        {
							continue;
						}
						if (skilldef.AddHediff != null)
						{
							if (!pawn.health.hediffSet.HasHediff(skilldef.AddHediff))
							{
								pawn.health.AddHediff(skilldef.AddHediff);
							}
						}
						if (skilldef.SkillCalc != null)
						{
							skilldef.SkillCalc.FPHealTick(pawn);
						}
					}
				}
				// 追加スキル
				foreach (SkillDef skilldef in CommonSkillDef)
				{
					if (skilldef.SkillCategory == SkillCategory.Active)
					{
						continue;
					}
					if (skilldef.AddHediff != null)
					{
						if (!pawn.health.hediffSet.HasHediff(skilldef.AddHediff))
						{
							pawn.health.AddHediff(skilldef.AddHediff);
						}
					}
					if (skilldef.SkillCalc != null)
					{
						skilldef.SkillCalc.FPHealTick(pawn);
					}
				}
				if (!pawn.health.hediffSet.HasHediff(Hediff_LitF.LitF_Consumption))
                {
					// FP回復
					FPHeal(GetHealRate());
				}
				CoolDownRate = GetCoolDownRate();
			}
			// 1日ごとの処理
			if (parent.IsHashIntervalTick(60000))
			{
				// 経験値増加
				XP += LitF_Config.GetDailyXPAmount;
			}
		}

		// スキルが使えるかのチェック
		public bool SkillUseCheck(SkillDef skillDef, out String disabledReason)
        {
			Pawn pawn = (Pawn)this.parent;
			if (pawn.ageTracker.CurLifeStage.developmentalStage.Child())
			{
				disabledReason = "LitF.UI.ChildCantUseFP".Translate();
				return false;
			}
			if (pawn.Downed)
			{
				disabledReason = "LitF.UI.NowDown".Translate();
				return false;
			}
			if (SkillCoolDown.TryGetValue(skillDef.defName, 0) > 0)
            {
				disabledReason = "LitF.UI.WaitCoolDown".Translate();
				return false;
			}
			if (NowFP <= 0)
			{
				disabledReason = "LitF.UI.LowFP".Translate();
				return false;
			}
			return skillDef.SkillCalc.SkillUseCheck(pawn, out disabledReason);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (FirstSpawn == false) {
				if (respawningAfterLoad == false)
                {
					// ランダムで一つスキルのスタイルを取得
					List<SkillStyleDef> StyleRand = DefDatabase<SkillStyleDef>.AllDefs.Where(x => x.CanGeneRandom && x.CommonRate > Rand.Range(0, 100)).ToList();
					SkillStyles.Add(StyleRand.RandomElement());
					// もし野生のリトルフェアリーなら、もう一つランダムで獲得し、パッシブをランダムで解除
					if (this.parent.Faction != null && this.parent.Faction == Find.FactionManager.FirstFactionOfDef(FactionDef.Named("LitF_BloodyWildLittleFairy")))
					{
						List<SkillStyleDef> addStyle = DefDatabase<SkillStyleDef>.AllDefs.Where(x => x.CanGeneRandom && !SkillStyles.Contains(x) && x.CommonRate > Rand.Range(0, 100)).ToList();
						SkillStyles.Add(addStyle.RandomElement());
						foreach (SkillStyleDef skillStyle in SkillStyles)
						{
							foreach (SkillDef skilldef in skillStyle.SkillDefs)
							{
								if (skilldef.SkillCategory == SkillCategory.Active)
								{
									continue;
								}
								if (Rand.Range(0, 3) == 0)
								{
									SkillActive.SetOrAdd(skilldef.defName, true);
								}
							}
						}
						// もし、ランダム継承が有効なら、ランダムで手に入れる
						if (LitF_Config.RandomSuccession)
						{
							if (Rand.Range(0, 100) < LitF_Config.RandomSuccession_Rate)
							{
								List<SkillDef> SkillSuccession = DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.defName.Contains("LitF_Succession") && !x.TargetPawn.NullOrEmpty() && (x.IsDuplication || !CommonSkillDef.Contains(x))).ToList();
								CommonSkillDef.Add(SkillSuccession.RandomElement());
							}
						}
					}
					// もし、親が継承スキルを持っている場合（or異種族の場合）能力継承
					Pawn pawn = (Pawn)this.parent;
					if (pawn.GetFather() != null)
                    {
						if (pawn.GetFather().RaceProps.Humanlike)
                        {
							List<SkillDef> SkillSuccession_Father = DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.defName.Contains("LitF_Succession") && x.TargetPawn.Contains(pawn.GetFather().def.defName) && (x.IsDuplication || !CommonSkillDef.Contains(x))).ToList();
							if (!SkillSuccession_Father.NullOrEmpty())
                            {
								CommonSkillDef.Add(SkillSuccession_Father.First());
							}							
						}
                    }
					if (pawn.GetMother() != null)
					{
						if (pawn.GetMother().def == pawn.def)
						{
							List<SkillDef> SkillSuccession_Mother = pawn.GetMother().GetComp<Comp_LitF>().CommonSkillDef.Where(x => x.defName.Contains("LitF_Succession") && (x.IsDuplication || !CommonSkillDef.Contains(x))).ToList();
							if (!SkillSuccession_Mother.NullOrEmpty())
							{
								CommonSkillDef.AddRange(SkillSuccession_Mother);
							}
						} else
                        {
							if (pawn.GetFather().RaceProps.Humanlike)
							{
								List<SkillDef> SkillSuccession_Mother = DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.defName.Contains("LitF_Succession") && x.TargetPawn.Contains(pawn.GetMother().def.defName) && (x.IsDuplication || !CommonSkillDef.Contains(x))).ToList();
								if (!SkillSuccession_Mother.NullOrEmpty())
								{
									CommonSkillDef.Add(SkillSuccession_Mother.First());
								}
							}
						}
					}
					// 1つから3つのランダムスキルを獲得
					for (int i = 0; i < Rand.Range(0, 3); i++)
					{
						List<SkillDef> SkillRand = DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.TargetPawn.NullOrEmpty() && x.CommonRate > Rand.Range(0, 100) && (x.IsDuplication || !CommonSkillDef.Contains(x))).ToList();
						CommonSkillDef.Add(SkillRand.RandomElement());
					}
					SetFPMax();
					FPHeal(9999);
					CoolDownRate = GetCoolDownRate();
					XP = 0;
					// Hediffの監視
					// スキルスタイル
					foreach (SkillStyleDef skillStyle in SkillStyles)
					{
						foreach (SkillDef skilldef in skillStyle.SkillDefs)
						{
							if (skilldef.SkillCategory == SkillCategory.Active)
							{
								continue;
							}
							if (SkillActive.TryGetValue(skilldef.defName, false) == false)
							{
								continue;
							}
							if (skilldef.AddHediff != null)
							{
								if (!pawn.health.hediffSet.HasHediff(skilldef.AddHediff))
								{
									pawn.health.AddHediff(skilldef.AddHediff);
								}
							}
							if (skilldef.SkillCalc != null)
							{
								skilldef.SkillCalc.FPHealTick(pawn);
							}
						}
					}
					// 追加スキル
					foreach (SkillDef skilldef in CommonSkillDef)
					{
						if (skilldef.SkillCategory == SkillCategory.Active)
						{
							continue;
						}
						if (skilldef.AddHediff != null)
						{
							if (!pawn.health.hediffSet.HasHediff(skilldef.AddHediff))
							{
								pawn.health.AddHediff(skilldef.AddHediff);
							}
						}
						if (skilldef.SkillCalc != null)
						{
							skilldef.SkillCalc.FPHealTick(pawn);
						}
					}
					/*
					if (DebugSettings.godMode)
					{
						XP = 99999;
					}
					*/
					FirstSpawn = true;
				}
			}
		}

		public void ResetSkill()
		{
			Pawn pawn = (Pawn)this.parent;
			SkillActive = new Dictionary<string, bool>();
			SkillCoolDown = new Dictionary<string, int>();
			SkillStyles = new List<SkillStyleDef>();
			CommonSkillDef = new List<SkillDef>();
			// コモンスキルのHediff削除
			foreach (SkillDef skill in DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.AddHediff != null))
            {
				Hediff hed = pawn.health.hediffSet.GetFirstHediffOfDef(skill.AddHediff);
				if (hed != null)
                {
					hed.Severity = 0.0f;
					hed.Heal(999.9f);
				}
			}
			SetFPMax();
			FPHeal(9999);
			CoolDownRate = GetCoolDownRate();
			XP = 0;
		}

		// スキルステータス生成
		public void MakeSkill(List<SkillStyleDef> skillStyleDefs, SkillStyleDef fix, List<SkillDef> skillDefs, int maxstyle)
        {
			// スキルスタイル決定
			int i = 0;
			if (fix != null)
            {
				SkillStyles.Add(fix);
				i = 1;
			}
			for (int j = i; j < maxstyle; j++)
            {
				List<SkillStyleDef> randskill = skillStyleDefs.Where(x => x.CommonRate > Rand.Range(0, 100) && x.CanGeneRandom && !SkillStyles.Contains(x)).ToList();
				SkillStyles.Add(randskill.RandomElement());
			}
			// 1つから3つのランダムスキルを獲得
			for (i = 0; i < maxstyle; i++)
			{
				List<SkillDef> SkillRand = skillDefs.Where(x => x.IsCommonSkill && x.CommonRate > Rand.Range(0, 100) && (x.IsDuplication || !CommonSkillDef.Contains(x))).ToList();
				CommonSkillDef.Add(SkillRand.RandomElement());
			}
		}

		public void RefreshFP()
        {
			SetFPMax();
			FPHeal(9999);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Pawn comppawn = (Pawn)this.parent;
			if (Find.Selector.SelectedPawns.Contains(comppawn))
			{
				Gizmo_FPStatus gizmo_FPStatus = new Gizmo_FPStatus();
				gizmo_FPStatus.comp = this;
				yield return gizmo_FPStatus;
				if (DebugSettings.godMode)
				{
					Command_Action command_Dev_ReState = new Command_Action();
					command_Dev_ReState.defaultLabel = "Dev:SetState";
					command_Dev_ReState.icon = Graphic_LitF.texture_stats;
					command_Dev_ReState.action = delegate
					{
						ResetSkill();
						List<SkillStyleDef> skillStyleDefs = DefDatabase<SkillStyleDef>.AllDefs.ToList();
						List<SkillDef> skillDefs = DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.TargetPawn.NullOrEmpty()).ToList();
						MakeSkill(skillStyleDefs, null, skillDefs, 4);
						RefreshFP();
					};
					yield return command_Dev_ReState;
					Command_Action command_Dev_MaxXP = new Command_Action();
					command_Dev_MaxXP.defaultLabel = "Dev:MaxXP";
					command_Dev_MaxXP.icon = Graphic_LitF.texture_stats;
					command_Dev_MaxXP.action = delegate
					{
						XP = 99999;
					};
					yield return command_Dev_MaxXP;
				}
				if ((comppawn.Faction?.IsPlayer ?? false) == false)
				{
					yield break;
				} else
                {
					List<SkillDef> Skills = new List<SkillDef>();
					// スタイルのスキルをリストに追加
					foreach (SkillStyleDef skillStyle in SkillStyles)
					{
						foreach (SkillDef skilldef in skillStyle.SkillDefs)
						{
							if (skilldef.SkillCategory == SkillCategory.Passive)
							{
								continue;
							}
							if (!SkillActive.TryGetValue(skilldef.defName, false))
							{
								continue;
							}
							Skills.Add(skilldef);
						}
					}
					Skills.AddRange(CommonSkillDef);
					// 追加スキルのスキルコマンド追加
					foreach (SkillDef skilldef in Skills)
					{
						if (skilldef.SkillCategory == SkillCategory.Passive)
						{
							continue;
						}
						if (!SkillActive.TryGetValue(skilldef.defName, false))
						{
							continue;
						}
						String cooldown = SkillCoolDown.TryGetValue(skilldef.defName, 0).TicksToSeconds().ToString("F1");
						String reason = "";
						Gizmo Gizmo = null;
						if (skilldef.Target == SkillTarget.Self)
						{
							Gizmo = new Command_Action
							{
								defaultLabel = skilldef.label + "(" + cooldown + ")",
								icon = Graphic_LitF.texture_skill.TryGetValue(skilldef.defName),
								defaultDesc = Calc_UI.ShowSkillState(skilldef, false),
								disabled = !SkillUseCheck(skilldef, out reason),
								disabledReason = reason,
								action = delegate
								{
									UseSkill = skilldef;
									JobDef jobDef = Job_LitF.LitF_UseSkill;
									Job job = JobMaker.MakeJob(jobDef, comppawn);
									comppawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
								},
								onHover = delegate
								{
									if (skilldef.TargetAreaRange != -1f)
									{
										if (comppawn.Position.IsValid)
										{
											bool nonwall = skilldef.PenetratingWall == PenetratingWall.Range || skilldef.PenetratingWall == PenetratingWall.Both ? true : false;
											Calc_UI.DrawRadiusRing(comppawn.Position, skilldef.TargetAreaRange, Color.white, nonwall, null);
										}
									}
								}
							};
						}
						else
						{
							Gizmo = new Command_Target
							{
								defaultLabel = skilldef.label + "(" + cooldown + ")",
								icon = Graphic_LitF.texture_skill.TryGetValue(skilldef.defName),
								defaultDesc = Calc_UI.ShowSkillState(skilldef, false),
								disabled = !SkillUseCheck(skilldef, out reason),
								disabledReason = reason,
								targetingParams = new TargetingParameters()
								{
									canTargetSelf = skilldef.Target == SkillTarget.Pawn ? true : false,
									canTargetPawns = skilldef.Target == SkillTarget.Pawn ? true : false,
									canTargetBuildings = skilldef.Target == SkillTarget.Build ? true : false,
									canTargetLocations = skilldef.Target == SkillTarget.Locate ? true : false,
									validator = delegate (TargetInfo targ)
									{
										if (skilldef.TargetRange != -1f)
										{
											if (comppawn.Position.IsValid)
											{
												bool nonwall = skilldef.PenetratingWall == PenetratingWall.ShootLine || skilldef.PenetratingWall == PenetratingWall.Both ? true : false;
												Calc_UI.DrawRadiusRing(comppawn.Position, skilldef.TargetRange, Color.white, nonwall, null);
											}
										}
										if (skilldef.TargetRange == -1f)
										{
											return true;
										}
										if (skilldef.Target == SkillTarget.Locate)
										{
											if (!targ.Cell.IsValid)
											{
												return false;
											}
											if (comppawn.Position.DistanceTo(targ.Cell) > skilldef.TargetRange || (!GenSight.LineOfSight(comppawn.Position, targ.Cell, Find.CurrentMap) && (skilldef.PenetratingWall == PenetratingWall.None || skilldef.PenetratingWall == PenetratingWall.Range)))
											{
												return false;
											}
											if (skilldef.TargetAreaRange != -1f)
											{
												if (targ.Cell.IsValid)
												{
													bool nonwall = skilldef.PenetratingWall == PenetratingWall.Range || skilldef.PenetratingWall == PenetratingWall.Both ? true : false;
													Calc_UI.DrawRadiusRing(targ.Cell, skilldef.TargetAreaRange, Color.white, nonwall, null);
												}
											}
											return true;
										}
										if (targ.Thing != null)
										{
											if (skilldef.Target == SkillTarget.Pawn)
											{
												Pawn pawn = targ.Thing as Pawn;
												if (pawn != null)
												{
													if (pawn.Position.DistanceTo(comppawn.Position) > skilldef.TargetRange || (!GenSight.LineOfSight(comppawn.Position, targ.Cell, Find.CurrentMap) && (skilldef.PenetratingWall == PenetratingWall.None || skilldef.PenetratingWall == PenetratingWall.Range)))
													{
														return false;
													}
													if (skilldef.TargetAreaRange != -1f)
													{
														if (targ.Cell.IsValid)
														{
															bool nonwall = skilldef.PenetratingWall == PenetratingWall.Range || skilldef.PenetratingWall == PenetratingWall.Both ? true : false;
															Calc_UI.DrawRadiusRing(targ.Cell, skilldef.TargetAreaRange, Color.white, nonwall, null);
														}
													}
													return true;
												}
											}
											if (skilldef.Target == SkillTarget.Build)
											{
												if (targ.Thing.Position.DistanceTo(comppawn.Position) > skilldef.TargetRange || (!GenSight.LineOfSight(comppawn.Position, targ.Cell, Find.CurrentMap) && (skilldef.PenetratingWall == PenetratingWall.None || skilldef.PenetratingWall == PenetratingWall.Range)))
												{
													return false;
												}
												if (skilldef.TargetAreaRange != -1f)
												{
													if (targ.Cell.IsValid)
													{
														bool nonwall = skilldef.PenetratingWall == PenetratingWall.Range || skilldef.PenetratingWall == PenetratingWall.Both ? true : false;
														Calc_UI.DrawRadiusRing(targ.Cell, skilldef.TargetAreaRange, Color.white, nonwall, null);
													}
												}
												return true;
											}
										}
										return false;
									},
								},
								action = delegate (LocalTargetInfo target)
								{
									UseSkill = skilldef;
									JobDef jobDef = Job_LitF.LitF_UseSkill;
									Job job = JobMaker.MakeJob(jobDef, target);
									comppawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
								},
							};
						}
						yield return Gizmo;
					}
				}
			}
		}

		public void UseFP(int amount)
        {
			NowFP = Math.Max(0, NowFP - amount);
			if (NowFP <= 0)
			{
				MoteMaker.ThrowText(this.parent.TrueCenter(), this.parent.Map, "Break!");
				Sound_LitF.LitF_Break.PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map, false));
				Pawn pawn = (Pawn)this.parent;
				pawn.health.AddHediff(Hediff_LitF.LitF_Consumption);
			}
		}

		public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			base.PostPreApplyDamage(dinfo, out absorbed);
			if (absorbed == false)
            {
				if (NowFP > 0)
                {
					Pawn pawn = (Pawn)this.parent;
					int DamageAmount = Math.Max((int)(dinfo.Amount * pawn.GetStatValue(StatDefOf.IncomingDamageFactor)), 1);
					if (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer))
                    {
						DamageAmount = (int)((float)DamageAmount * LitF_Config.EnemyFairy_Damage);
					}
					NowFP = Math.Max(0, NowFP - DamageAmount);
					XP += DamageAmount;
					MoteMaker.MakeStaticMote(this.parent.TrueCenter(), this.parent.Map, Mote_LitF.LitF_Mote_FPGuard, 0.75f);
					if (NowFP <= 0)
                    {
						MoteMaker.ThrowText(this.parent.TrueCenter(), this.parent.Map, "Break!");
						Sound_LitF.LitF_Break.PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map, false));
						pawn.health.AddHediff(Hediff_LitF.LitF_Consumption);
					} else
                    {
						MoteMaker.ThrowText(this.parent.TrueCenter(), this.parent.Map, "Protect!");
						Sound_LitF.LitF_Parry.PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map, false));
					}
					absorbed = true;
				}
            }
		}
	}

}
