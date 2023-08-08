using BEPRace_Core;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace LittleFairy_Race
{
	public class CompTargetable_LitF : CompTargetable
	{
		protected override bool PlayerChoosesTarget => true;

		protected override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetPawns = true,
				canTargetBuildings = false,
				canTargetItems = false,
				mapObjectTargetsMustBeAutoAttackable = false,
				validator = (TargetInfo x) => IsFairy(x.Thing)
			};
		}

		public bool IsFairy(Thing t)
		{
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				if (pawn.def.defName == "LittleFairy_Pawn")
				{
					return true;
				}
			}
			return false;
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}
	}

	public class CompTargetable_LitF_OnlyPlayerorPrisoner : CompTargetable
	{
		protected override bool PlayerChoosesTarget => true;

		protected override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetSelf = false,
				canTargetPawns = true,
				canTargetBuildings = false,
				canTargetItems = false,				
				mapObjectTargetsMustBeAutoAttackable = false,
				validator = (TargetInfo x) => IsFairy(x.Thing)
			};
		}

		public bool IsFairy(Thing t)
		{
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				if (pawn.def.defName == "LittleFairy_Pawn" || pawn.def.defName == "Mofy_Pawn" || pawn.def.defName == "Resurreviewy_Pawn" || pawn.def.defName == "Glassy_Pawn" || pawn.def.defName == "Sweety_Pawn")
				{
					if ((((ModLister.IdeologyInstalled && pawn.IsColonist) || pawn.IsPrisoner) && !pawn.IsSlave) || (!pawn.Faction?.IsPlayer ?? true))
                    {
						return true;
					}
				}
			}
			return false;
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}
	}

	public class CompTargetEffect_BrillianceAmplifier : CompTargetEffect
	{
		public override void DoEffectOn(Pawn user, Thing target)
		{
			if (target.def.defName == "LittleFairy_Pawn")
			{
				Comp_LitF comp = ((Pawn)target).GetComp<Comp_LitF>();
				if (comp != null)
				{
					if (comp.CommonSkillDef.Count >= 12)
					{
						Messages.Message("LitF.UI.CantAddSkill".Translate(), target, MessageTypeDefOf.RejectInput, false);
						return;
					}
					Sound_LitF.LitF_Chant.PlayOneShot(new TargetInfo(target.Position, target.Map, false));
					Effecter_BEPCore.BEP_UseSkill_C.Spawn(this.parent.Position, this.parent.Map, Vector3.zero);
					List<SkillDef> SkillRand = DefDatabase<SkillDef>.AllDefs.Where(x => x.IsCommonSkill && x.TargetPawn.EnumerableNullOrEmpty() && x.CommonRate > Rand.Range(0, 100) && (x.IsDuplication || !comp.CommonSkillDef.Contains(x))).ToList();
					SkillDef skill = SkillRand.RandomElement();
					comp.CommonSkillDef.Add(skill);
					comp.SetFPMax();
					Find.LetterStack.ReceiveLetter("LitF.Incident.LetterLabelAddSkill".Translate(), "LitF.Incident.LetterAddSkill".Translate(target, skill.label, Calc_UI.ShowSkillState(skill, true)), LetterDefOf.PositiveEvent, target);
				}
			}
		}
	}

	public class CompTargetEffect_EnSlave : CompTargetEffect
	{
		public override void DoEffectOn(Pawn user, Thing target)
		{
			if (target.def.defName == "LittleFairy_Pawn" || target.def.defName == "Mofy_Pawn" || target.def.defName == "Resurreviewy_Pawn" || target.def.defName == "Glassy_Pawn" || target.def.defName == "Sweety_Pawn")
			{
				if (user.IsColonistPlayerControlled && user.CanReserveAndReach(target, PathEndMode.Touch, Danger.Deadly))
				{
					Job job = JobMaker.MakeJob(Job_LitF.LitF_UseCollar, target, parent);
					job.count = 1;
					user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
				}
			}
		}
	}

}
