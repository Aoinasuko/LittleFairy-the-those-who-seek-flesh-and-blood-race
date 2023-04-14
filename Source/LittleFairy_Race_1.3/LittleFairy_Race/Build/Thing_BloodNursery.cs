using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace LittleFairy_Race
{
    public class Thing_BloodNursery : Building, IThingHolder
	{
		public CompForbiddable forbiddable;

		// 苗床のコアになっているリトルフェアリー
		public Pawn Nursery_Core = null;
		// 苗床の種になっているリトルフェアリー
		public ThingOwner innerContainer;

		// 苗床が持っているスキルの経験値
		public Dictionary<RimWorld.SkillDef, int> SkillXP = new Dictionary<RimWorld.SkillDef, int>();

		// 現在のコマ
		int Move_Left = 0;
		int Move_Right = 0;

		// 苗床が生まれるまでの時間
		int Nursery_Tick = 0;

		// 苗床のコアの容量
		public int Nursery_Life = 0;

		// 苗床が持っている経験値総量
		public int XP = 0;

		// 苗床が求めているスキル
		public SkillStyleDef NeedSkill;

		public Thing_BloodNursery()
		{
			innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			Scribe_References.Look(ref Nursery_Core, "Nursery_Core");
			Scribe_Collections.Look(ref SkillXP, "SkillXP");
			Scribe_Values.Look(ref Move_Left, "Move_Left");
			Scribe_Values.Look(ref Move_Right, "Move_Right");
			Scribe_Values.Look(ref Nursery_Tick, "Nursery_Tick");
			Scribe_Values.Look(ref Nursery_Life, "Nursery_Life");
			Scribe_Values.Look(ref XP, "XP");
			Scribe_Defs.Look(ref NeedSkill, "NeedSkill");
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			forbiddable = GetComp<CompForbiddable>();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			Command_Action command_Nursery = new Command_Action();
			command_Nursery.defaultLabel = "LitF.UI.StatNursery".Translate();
			command_Nursery.icon = Graphic_LitF.texture_stats;
			command_Nursery.action = delegate
			{
				Dialog_Nursery dialog = new Dialog_Nursery(this);
				Find.WindowStack.Add(dialog);
			};
			yield return command_Nursery;
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public bool Accepts(Thing thing)
		{
			return innerContainer.CanAcceptAnyOf(thing);
		}

		public static Thing_BloodNursery FindNurseryFor(Pawn pawn_job, Pawn traveler, bool ignoreOtherReservations = false)
		{
			IEnumerable<ThingDef> enumerable = DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => typeof(Thing_BloodNursery).IsAssignableFrom(def.thingClass));
			foreach (ThingDef singleDef in enumerable)
			{
				Thing_BloodNursery building_Nursery = (Thing_BloodNursery)GenClosest.ClosestThingReachable(pawn_job.Position, pawn_job.Map, ThingRequest.ForDef(singleDef), PathEndMode.InteractionCell, TraverseParms.For(traveler), 9999f, delegate (Thing x)
				{
					if (pawn_job.def.defName != "LittleFairy_Pawn" || ((Thing_BloodNursery)x).Nursery_Life <= 0)
					{
						Pawn pawn_job2 = traveler;
						LocalTargetInfo target = x;
						bool ignoreOtherReservations2 = ignoreOtherReservations;
						return pawn_job2.CanReserve(target, 1, -1, null, ignoreOtherReservations2);
					}
					return false;
				});
				if (building_Nursery != null && !building_Nursery.forbiddable.Forbidden)
				{
					return building_Nursery;
				}
			}
			return null;
		}

		public virtual bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
		{
			if (!Accepts(thing))
			{
				return false;
			}
			bool flag = false;
			if (thing.holdingOwner != null)
			{
				thing.holdingOwner.TryTransferToContainer(thing, innerContainer, thing.stackCount);
				flag = true;
			}
			else
			{
				flag = innerContainer.TryAdd(thing);
			}
			if (flag)
			{
				return true;
			}
			return false;
		}


		// 苗床の追加
		public void AddNursery(Pawn pawn)
        {
			// (保険)死体除去
			innerContainer.RemoveAll(x => x as Pawn == null);
			// リトルフェアリーなら、コアにする
			if (pawn.def.defName == "LittleFairy_Pawn")
            {
				// 既にリトルフェアリーが入っているなら、コアを削除する
				if (Nursery_Core != null)
                {
					if (innerContainer.Contains(Nursery_Core))
                    {
						Nursery_Core.Destroy();
						innerContainer.Remove(Nursery_Core);
					}
				} else
                {
					this.Nursery_Tick = 300000;
				}
				// コアを加える
				Nursery_Core = pawn;
				TryAcceptThing(pawn);
				pawn.SetFactionDirect(null);
				Nursery_Life = 5;
				return;
			}
			// それ以外の種族なら、経験値を加える
			if (pawn.skills != null)
            {
				foreach (RimWorld.SkillDef skill in DefDatabase<RimWorld.SkillDef>.AllDefs)
                {
					SkillRecord record = pawn.skills.skills.Where(x => x.def == skill).FirstOrDefault();
					if (record != null)
                    {
						SkillXP.SetOrAdd(skill, SkillXP.TryGetValue(skill,0) + (int)record.XpTotalEarned);
						XP = Math.Min(20000000, XP + (int)record.XpTotalEarned);
					}
                }
            } else
            {
				XP = Math.Min(20000000, XP + ((int)pawn.MarketValue * 5));
            }
			// コアに5つ以上人が入っているなら、削除する
			if (innerContainer.Where(x => x as Pawn != null && x.def.defName != "LittleFairy_Pawn").Count() >= 5)
            {
				Thing first = innerContainer.Where(x => x as Pawn != null && x.def.defName != "LittleFairy_Pawn").FirstOrDefault();
				if (first != null)
                {
					first.Destroy();
					innerContainer.Remove(first);
				}
			}
			TryAcceptThing(pawn);
			pawn.SetFactionDirect(null);
		}

		public override void Draw()
		{
			//セッティング
			Vector3 vector = base.DrawPos;
			Vector3 s = new Vector3(2.0f, 0.0f, 2.0f);
			float angle = 0f;

			if (this.Nursery_Life > 0)
			{
				vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
				Graphics.DrawMesh(MeshPool.plane10, matrix, Graphic_LitF.BloodNursery_BLeft.TryGetValue(Move_Left), 0);

				vector.y = AltitudeLayer.MoteOverhead.AltitudeFor() + 1.0f;
				Matrix4x4 matrix2 = default(Matrix4x4);
				matrix2.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
				Graphics.DrawMesh(MeshPool.plane10, matrix2, Graphic_LitF.BloodNursery_BRight.TryGetValue(Move_Right), 1);

				if (this.Nursery_Tick < 60000)
				{
					vector.y = AltitudeLayer.MoteOverhead.AltitudeFor() + 1.3f;
					Matrix4x4 matrix3 = default(Matrix4x4);
					matrix3.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
					Graphics.DrawMesh(MeshPool.plane10, matrix3, Graphic_LitF.BloodNursery_BigEgg, 2);
				}
			}
			vector.y = AltitudeLayer.MoteOverhead.AltitudeFor() + 1.5f;
			Matrix4x4 matrix4 = default(Matrix4x4);
			matrix4.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
			Graphics.DrawMesh(MeshPool.plane10, matrix4, Graphic_LitF.BloodNursery_Body, 4);

		}

		public override void Tick()
		{
			base.Tick();
			if (this.IsHashIntervalTick(3))
            {
				if (Move_Left > 0)
                {
					Move_Left -= 1;
				}
				if (Move_Right > 0)
				{
					Move_Right -= 1;
				}
			}
			if (this.IsHashIntervalTick(180))
            {
				if (Move_Left == 0)
                {
					if (Rand.Range(0, 5) == 0)
					{
						Move_Left = 3;
					}
				}
				if (Move_Right == 0)
				{
					if (Rand.Range(0, 5) == 0)
					{
						Move_Right = 3;
					}
				}
				// (保険)死体除去
				innerContainer.RemoveAll(x => x as Pawn == null);
				if (Nursery_Core == null || Nursery_Core.Dead)
                {
					Nursery_Core = null;
					Nursery_Life = 0;
				}
			}
			// 苗床の時間
			if (this.Nursery_Life > 0)
            {
				if (this.Nursery_Core != null)
				{
					//リトルフェアリー以外の種族が入っているなら
					if (!innerContainer.Where(x => x as Pawn != null && x.def.defName != "LittleFairy_Pawn").EnumerableNullOrEmpty())
                    {
						if (this.Nursery_Tick > 0)
						{
							this.Nursery_Tick--;
						}
						else
						{
							Pawn pawn = null;
							// 5%の確率で、ミーフィーが生まれる
							if (Rand.Range(0,100) < LitF_Config.SpawnMeefaeRate)
                            {
								PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named("LitF_Meefae"), null, PawnGenerationContext.NonPlayer, -1, true, true);
								pawn = PawnGenerator.GeneratePawn(request);
								pawn.health.AddHediff(Hediff_LitF.LitF_Consumption);
								GenSpawn.Spawn(pawn, this.InteractionCell, this.Map);
							} else
                            {
								// 子供が生まれる
								PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named("LittleFairy_NurseryBorn"), null, PawnGenerationContext.NonPlayer, -1, true, true, false, false, false, false, 1f, false, true, false, false, false, false, false, false, 0f, 0f, null, 0f, null, null, null, null, null, null, null, null, null, null, null, null, this.Nursery_Core.Ideo != null ? this.Nursery_Core.Ideo : null);
								pawn = PawnGenerator.GeneratePawn(request);
								// スキルのセット
								pawn.story.childhood = BackstoryDatabase.allBackstories.Where(x => x.Key == "LitF_Child_NurseryBorn").FirstOrDefault().Value;
								pawn.story.adulthood = null;
								foreach (var skill in pawn.skills?.skills)
								{
									skill.Level = 0;
									skill.Learn(this.SkillXP.TryGetValue(skill.def, 0) / Rand.Range(5 - Calc_UI.CalcLv(this.XP), 7 - Calc_UI.CalcLv(this.XP)), true);
								}
								List<SkillStyleDef> skillStyleDefs = GetSkillStyle();
								List<SkillDef> skillDefs = GetSkill();
								pawn.GetComp<Comp_LitF>().ResetSkill();
								pawn.GetComp<Comp_LitF>().MakeSkill(skillStyleDefs, this.NeedSkill, skillDefs, Calc_UI.CalcLv(this.XP));
								pawn.health.AddHediff(Hediff_LitF.LitF_Consumption);
								GenSpawn.Spawn(pawn, this.InteractionCell, this.Map);
							}
							Messages.Message("LitF.UI.NurseryBirth".Translate(), pawn, MessageTypeDefOf.PositiveEvent, false);
							// 苗床カウント減少
							Nursery_Life--;
							if (Nursery_Life <= 0)
                            {
								// 苗床を消去する
								if (innerContainer.Contains(Nursery_Core))
								{
									Nursery_Core.Destroy();
									innerContainer.Remove(Nursery_Core);
								}
								Nursery_Core = null;
							}
							FilthMaker.TryMakeFilth(this.Position, this.Map, ThingDefOf.Filth_AmnioticFluid, this.Label, 5, FilthSourceFlags.None);
							this.Nursery_Tick = 300000;							
						}
					}
				}
			}
		}

        private List<SkillStyleDef> GetSkillStyle()
        {
			List<Thing> inner = this.innerContainer.Where(x => x as Pawn != null && x.def.defName != "LittleFairy_Pawn").ToList();
			List<SkillStyleDef> skillstyles = DefDatabase<SkillStyleDef>.AllDefs.Where(x => x.CanGeneRandom && x.TargetPawn.NullOrEmpty()).ToList();
			
			foreach (Pawn pawn in inner)
			{
				List<SkillStyleDef> addskillstyles = DefDatabase<SkillStyleDef>.AllDefs.Where(x => x.CanGeneRandom && x.TargetPawn.Contains(pawn.def.defName)).ToList();
				// 追加出現の可能性があるスキル
				if (!addskillstyles.EnumerableNullOrEmpty())
				{
					foreach (SkillStyleDef addstyle in addskillstyles)
					{
						if (!skillstyles.Contains(addstyle))
						{
							skillstyles.Add(addstyle);
						}
					}
				}
			}
			return skillstyles;
		}

		private List<SkillDef> GetSkill()
		{
			List<Thing> inner = this.innerContainer.Where(x => x as Pawn != null && x.def.defName != "LittleFairy_Pawn").ToList();
			List<SkillDef> skills = DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.TargetPawn.NullOrEmpty()).ToList();
			foreach (Pawn pawn in inner)
			{
				List<SkillDef> addskills = DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.TargetPawn.Contains(pawn.def.defName)).ToList();
				// 追加出現の可能性があるスキル
				if (!addskills.EnumerableNullOrEmpty())
				{
					foreach (SkillDef addskill in addskills)
					{
						if (!skills.Contains(addskill))
						{
							skills.Add(addskill);
						}
					}
				}
			}
			return skills;
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
		{
			foreach (FloatMenuOption option in base.GetFloatMenuOptions(pawn))
			{
				yield return option;
			}
			if (innerContainer.Where(x => x.def.defName == "LittleFairy_Pawn").Count() == 0)
			{
				if (!pawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
				{
					FloatMenuOption failer = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
					yield return failer;
				}
				else
				{
					if (pawn.def.defName != "LittleFairy_Pawn")
					{
						FloatMenuOption failer = new FloatMenuOption("LitF.UI.Job_OnlyLitF".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
						yield return failer;
					}
					else
					{
						JobDef jobDef = Job_LitF.LitF_EnterNursery;
						string jobStr = "LitF.UI.EnterNursery".Translate();
						Action jobAction = delegate ()
						{
							Job job = new Job(jobDef, this);
							pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
						};
						yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(jobStr, jobAction, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, this, "ReservedBy");
					}
				}
			}
			yield break;
		}

		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			bool flag = true;
			if (!text.NullOrEmpty())
			{
				text += "\n";
			}
			if (this.Nursery_Life <= 0)
			{
				text += "LitF.UI.NurseryNeedLitF".Translate();
				return text;
			}
			if (innerContainer.Where(x => x.def.defName != "LittleFairy_Pawn").EnumerableNullOrEmpty())
            {
				text += "LitF.UI.NurseryNeedMale".Translate();
				return text;
			}
			if (flag)
            {
				float Generation = (float)Math.Round((float)(300000 - this.Nursery_Tick) / 300000f * 100f, 1);
				text += "LitF.UI.NurseryProgress".Translate() + ": " + Generation + "%" + "\n";
				text += "LitF.UI.NurseryCoreVal".Translate() + ": " + Nursery_Life;
				return text;
			}
			return text;
		}
	}
}
