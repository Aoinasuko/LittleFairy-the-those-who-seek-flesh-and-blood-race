using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Sound_LitF.LitF_Chant.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
            MoteMaker.MakeStaticMote(pawn.TrueCenter(), pawn.Map, Mote_LitF.LitF_Mote_Chanting, 1f);
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
                if (comp.NowFP <= 0)
                {
                    Messages.Message("LitF.NeedMoreFP".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                }
                else
                {
                    comp.UseSkill.SkillCalc.SkillUse(pawn, TargetA);
                    comp.SkillCoolDown.SetOrAdd(comp.UseSkill.defName, comp.UseSkill.SkillCoolDown);
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
