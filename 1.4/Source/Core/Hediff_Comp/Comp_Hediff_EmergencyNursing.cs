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

	public class Comp_Hediff_EmergencyNursing : HediffComp
	{
		public override void CompPostTick(ref float severityAdjustment)
		{
			HediffComp_Disappears hediffComp_Disappears = this.parent.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null && hediffComp_Disappears.ticksToDisappear % 180 == 0)
			{
				Effecter_BEPCore.BEP_UseSkill_D.Spawn(this.Pawn.Position, this.Pawn.Map, Vector3.zero);
				Heal(this.Pawn);
			}
		}

		private void Heal(Pawn pawn)
		{
			IEnumerable<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			foreach (Hediff hediff in hediffs)
			{
				bool flag = hediff is Hediff_Injury;
				if (flag)
				{
					hediff.Heal(2.0f);
					if (!hediff.IsTended())
                    {
						hediff.Tended(10.0f, 0.8f);
					}
				}
			}
		}
	}
}
