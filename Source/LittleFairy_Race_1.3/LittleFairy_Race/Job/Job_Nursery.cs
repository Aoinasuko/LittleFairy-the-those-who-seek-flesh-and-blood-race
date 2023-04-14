using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace LittleFairy_Race
{
	public class JobDriver_CarryNursery : JobDriver
	{
		private const TargetIndex TakeeInd = TargetIndex.A;

		private const TargetIndex NurseryInd = TargetIndex.B;

		protected Pawn Takee => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		protected Thing_BloodNursery Nursery => job.GetTarget(TargetIndex.B).Thing as Thing_BloodNursery;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = base.pawn;
			LocalTargetInfo target = Takee;
			Job job = base.job;
			if (pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
			{
				pawn = base.pawn;
				target = Nursery;
				job = base.job;
				return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
			}
			return false;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnDestroyedOrNull(TargetIndex.B);
			this.FailOnAggroMentalState(TargetIndex.A);
			this.FailOn(() => !Nursery.Accepts(Takee));
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.B)
				.FailOn(() => Takee.def.defName == "LittleFairy_Pawn" && Nursery.Nursery_Life > 0)
				.FailOn(() => !pawn.CanReach(Takee, PathEndMode.OnCell, Danger.Deadly))
				.FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			yield return Toils_Haul.StartCarryThing(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell)
				.FailOn(() => Takee.def.defName == "LittleFairy_Pawn" && Nursery.Nursery_Life > 0);
			Toil prepare = Toils_General.Wait(200);
			prepare.FailOnCannotTouch(TargetIndex.B, PathEndMode.InteractionCell);
			prepare.WithProgressBarToilDelay(TargetIndex.B);
			yield return prepare;
			yield return new Toil
			{
				initAction = delegate
				{
					Nursery.AddNursery(Takee);
					if (Research_LitF.LitF_ResourceRecycling.IsFinished)
					{
						List<Thing> things = Takee.ButcherProducts(pawn, 0.8f).ToList();
						foreach (Thing thing in things)
                        {
							GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
						}
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}

		public override object[] TaleParameters()
		{
			return new object[2]
			{
			pawn,
			Takee
			};
		}
	}

	public class JobDriver_EnterNursery : JobDriver
	{

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo targetA = this.job.targetA;
			Job job = this.job;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			Toil prepare = Toils_General.Wait(200, TargetIndex.None);
			prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
			yield return prepare;
			Toil enter = new Toil();
			enter.initAction = delegate ()
			{
				Pawn actor = enter.actor;
				Thing_BloodNursery nursery = (Thing_BloodNursery)actor.CurJob.targetA.Thing;
				Action action = delegate ()
				{
					actor.DeSpawn(DestroyMode.Vanish);
					nursery.AddNursery(actor);
					if (Research_LitF.LitF_ResourceRecycling.IsFinished)
					{
						List<Thing> things = actor.ButcherProducts(pawn, 0.8f).ToList();
						foreach (Thing thing in things)
						{
							GenPlace.TryPlaceThing(thing, TargetA.Thing.InteractionCell, TargetA.Thing.Map, ThingPlaceMode.Near);
						}
					}
				};
				action();
			};
			enter.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return enter;
			yield break;
		}

	}
}
