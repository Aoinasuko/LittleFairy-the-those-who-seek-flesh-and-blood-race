using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace LittleFairy_Race
{
	public class SkillThing_Trap : ThingWithComps
	{
		public override void Tick()
		{
			base.Tick();
			Comp_SummonThing comp_thing = this.TryGetComp<Comp_SummonThing>();
			if (comp_thing != null)
            {
				if (comp_thing.life % 5 == 0)
                {
					IEnumerable<Pawn> pawns = this.Map.mapPawns.AllPawnsSpawned.Where(x => x.Position.DistanceTo(this.Position) <= 1.9f & x.HostileTo(comp_thing.owner));
					if (!pawns.EnumerableNullOrEmpty())
					{
						Pawn pawn = pawns.RandomElement();
						Sound_LitF.LitF_Chant.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
						MoteMaker.MakeStaticMote(pawn.TrueCenter(), pawn.Map, Mote_LitF.LitF_Mote_Effect, 1f);
						pawn.health.AddHediff(Hediff_LitF.LitF_Skill_Traped);
						pawn.TakeDamage(new DamageInfo(DamageDefOf.Cut, 10.0f, 2.0f, default, comp_thing.owner));						
						this.Destroy();
					}
				}
			}
		}
	}

}
