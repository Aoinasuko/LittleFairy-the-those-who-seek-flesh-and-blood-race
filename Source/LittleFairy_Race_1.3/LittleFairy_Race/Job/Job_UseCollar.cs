using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace LittleFairy_Race
{
	public class JobDriver_LitF_UseCollar : JobDriver
	{
		private const TargetIndex PawnInd = TargetIndex.A;

		private const TargetIndex ItemInd = TargetIndex.B;

		private const int DurationTicks = 120;

		private Pawn TargetPawn => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		private Thing Item => job.GetTarget(TargetIndex.B).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(TargetPawn, job, 1, -1, null, errorOnFailed))
			{
				return pawn.Reserve(Item, job, 1, -1, null, errorOnFailed);
			}
			return false;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
			if (TargetPawn != pawn)
            {
				PawnUtility.ForceWait(TargetPawn, 180, pawn, true);
			}
			Toil toil = Toils_General.Wait(120);
			toil.WithProgressBarToilDelay(TargetIndex.A);
			toil.FailOnDespawnedOrNull(TargetIndex.A);
			toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			toil.tickAction = delegate
			{
				CompUsable compUsable = Item.TryGetComp<CompUsable>();
			};
			yield return toil;
			yield return Toils_General.Do(Enslave);
		}

		private void Enslave()
		{
			Sound_LitF.LitF_Chant.PlayOneShot(SoundInfo.InMap(TargetPawn));
			Item.Destroy();
			Thing collor = ThingMaker.MakeThing(ThingDef.Named("LitF_SimpleCollor"), Item.Stuff);
			TargetPawn.apparel.Wear((Apparel)collor, true, true);
			if (ModLister.IdeologyInstalled)
            {
				try
                {
					GenGuest.EnslavePrisoner(pawn, TargetPawn);
				} catch
                {

                }
			} else
            {
				TargetPawn.SetFaction(Faction.OfPlayer, pawn);
			}
			
		}
	}
}
