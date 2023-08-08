using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace LittleFairy_Race
{
    public class JobDriver_UseSkill : JobDriver
    {
        public SkillDef Skilldef;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Comp_LitF comp = pawn.GetComp<Comp_LitF>();
            Effecter_BEPCore.BEP_UseSkill_A.Spawn(pawn.Position, pawn.Map, Vector3.zero);
            Toil prepare = Toils_General.Wait(comp.UseSkill.SkillStayTime, TargetIndex.None);
            if (pawn.CurJob.targetA.Thing != null)
            {
                prepare.FailOnDespawnedOrNull(TargetIndex.A);
            }
            prepare.AddEndCondition(delegate
            {
                if (comp.UseSkill.PenetratingWall == PenetratingWall.ShootLine || comp.UseSkill.PenetratingWall == PenetratingWall.Both || GenSight.LineOfSight(pawn.Position, pawn.CurJob.targetA.Cell, pawn.Map))
                {
                    return JobCondition.Ongoing;
                }
                return JobCondition.Incompletable;
            });
            yield return prepare;
            Toil active = new Toil();
            active.initAction = delegate()
            {
                // 現在スキルが使用できるかフラグ
                bool useflag = false;
                if (comp.NowFP <= 0)
                {
                    if (ModLister.BiotechInstalled)
                    {
                        if (Util_BEPCore.CheckBrillianceValue(pawn, 0.1f))
                        {
                            useflag = true;
                        } else
                        {
                            Messages.Message("LitF.NeedMoreFPorBrilliance".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                        }
                    } else
                    {
                        Messages.Message("LitF.NeedMoreFP".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                    }
                } else
                {
                    useflag = true;
                }
                if (useflag)
                {
                    comp.UseSkill.SkillCalc.SkillUse(pawn, TargetA);
                    // 輝きを消費できたかフラグ
                    bool dontconsumption = false;
                    if (ModLister.BiotechInstalled)
                    {
                        if (Util_BEPCore.CheckBrillianceValue(pawn, 0.1f))
                        {
                            Gene_Brilliance gene = pawn.genes.GetFirstGeneOfType<Gene_Brilliance>();
                            gene.useBrilliance(0.1f);
                        } else
                        {
                            dontconsumption = true;
                        }
                    }
                    // 輝きを消費できてない場合、クールダウンが3倍に
                    if (dontconsumption)
                    {
                        comp.SkillCoolDown.SetOrAdd(comp.UseSkill.defName, comp.UseSkill.SkillCoolDown * 3);
                    } else
                    {
                        comp.SkillCoolDown.SetOrAdd(comp.UseSkill.defName, comp.UseSkill.SkillCoolDown);
                    }
                }
            };
            active.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return active;
            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Skilldef, "Skilldef");
        }

    }
}
