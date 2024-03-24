using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LittleFairy_Race
{
	public class SkillThing_HealBall : ThingWithComps
	{
		public override void Tick()
		{
			base.Tick();
			Comp_SummonThing comp_thing = this.TryGetComp<Comp_SummonThing>();
			if (comp_thing != null)
            {
				if (comp_thing.life % 60 == 0)
                {
					Effecter_BEPCore.BEP_UseSkill_D.Spawn(this.Position, this.Map, Vector3.zero);
					IEnumerable<Pawn> pawns = this.Map.mapPawns.AllPawnsSpawned.Where(x => x.Position.DistanceTo(this.Position) <= 5.9f & !x.HostileTo(comp_thing.owner));
					if (!pawns.EnumerableNullOrEmpty())
					{
						foreach (Pawn pawn in pawns)
                        {
							Heal(pawn);
						}
					}
				}
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
					hediff.Heal(1.0f);
					break;
				}
			}
			pawn.health.hediffSet.hediffs.RemoveAll((Hediff x) => x.def.everCurableByItem && x is Hediff_Injury && x.IsPermanent() && pawn.health.hediffSet.GetPartHealth(x.Part) <= 0f && x.Part.def.GetMaxHealth(pawn) <= 10.0f);
		}
	}

}
