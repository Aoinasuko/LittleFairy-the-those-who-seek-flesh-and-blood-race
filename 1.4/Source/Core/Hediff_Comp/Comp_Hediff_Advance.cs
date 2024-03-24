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

	public class Comp_Hediff_Advance : HediffComp
	{
		public int ticksToCounter;

		public override void CompPostTick(ref float severityAdjustment)
		{
			HediffComp_Disappears hediffComp_Disappears = this.parent.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null && hediffComp_Disappears.ticksToDisappear % 30 == 0)
			{
				Effecter_BEPCore.BEP_UseSkill_D.Spawn(this.Pawn.Position, this.Pawn.Map, Vector3.zero);
			}
			if (ticksToCounter <= 0)
			{
				IEnumerable<Thing> Projs = Pawn.Map.listerThings.AllThings.Where(x => x as Projectile != null & x.Position.DistanceTo(Pawn.Position) <= 2.9f & x != Pawn);
				if (!Projs.EnumerableNullOrEmpty())
				{
					for (int i = Projs.Count() - 1; i >= 0; i--)
					{
						Projectile proj = (Projectile)Projs.ElementAt(i);
						if (proj.Launcher?.Spawned ?? false)
						{
							if (proj.Launcher.HostileTo(Pawn) & Pawn.CanSee(proj))
							{
								FleckMaker.Static(proj.TrueCenter(), proj.Map, FleckDefOf.ExplosionFlash, 12f);
								for (int i2 = 0; i2 < 6; i2++)
								{
									FleckMaker.ThrowDustPuff(proj.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), proj.Map, Rand.Range(0.8f, 1.2f));
								}
								IntVec3 c = proj.Launcher.Position;
								Vector3 drawPos = proj.DrawPos;
								proj.Destroy();
								Sound_LitF.LitF_Parry.PlayOneShot(proj);
								ticksToCounter = 6;
								break;
							}
						}
					}
				}
			}
			else
			{
				ticksToCounter--;
			}
		}

		public override void CompExposeData()
		{
			Scribe_Values.Look(ref ticksToCounter, "ticksToCounter", 0);
		}
	}
}
