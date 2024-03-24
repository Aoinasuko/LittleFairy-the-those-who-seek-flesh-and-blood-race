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
			Toil toil = ToilMaker.MakeToil("MakeNewToils");
			toil.WithProgressBarToilDelay(TargetIndex.A);
			toil.FailOnDespawnedOrNull(TargetIndex.A);
			toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			toil.initAction = delegate
			{
				TargetPawn.pather.StopDead();
				PawnUtility.ForceWait(TargetPawn, 15000, null, maintainPosture: true);
			};
			toil.tickAction = delegate
			{
				CompUsable compUsable = Item.TryGetComp<CompUsable>();
			};
			yield return toil;
			if (TargetPawn != pawn)
			{
				PawnUtility.ForceWait(TargetPawn, 180, pawn, true);
			}
			yield return Toils_General.Do(Enslave);
		}

		private void Enslave()
		{
			if (TargetPawn.Faction != null && TargetPawn.Faction.hidden != true)
            {
				if (!TargetPawn.Faction.IsPlayer)
                {
					Faction.OfPlayer.TryAffectGoodwillWith(TargetPawn.Faction, -100, true, true, null, TargetPawn);
				}
			} else
            {
				if (TargetPawn.Faction != null)
                {
					IEnumerable<Pawn> pawns;
					pawns = TargetPawn.Map.mapPawns.AllPawnsSpawned.Where(x => x != TargetPawn && x.Position.DistanceTo(TargetPawn.Position) <= 20.9f && TargetPawn.Faction == x.Faction);
					if (!pawns.EnumerableNullOrEmpty())
					{
						Effecter_BEPCore.BEP_UseSkill_C.Spawn(TargetPawn.Position, TargetPawn.Map, Vector3.zero);
						foreach (Pawn pawn in pawns)
						{
							pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);
						}
						Messages.Message("LitF.Event.Runaway_description".Translate(), new LookTargets(pawns), MessageTypeDefOf.NegativeEvent);
					}
				}
			}
			Sound_LitF.LitF_Chant.PlayOneShot(SoundInfo.InMap(TargetPawn));
			Item.Destroy();
			Thing collor = ThingMaker.MakeThing(ThingDef.Named("BEP_SimpleCollor"), Item.Stuff);
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
