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
	public class CompProperties_TotemAddHediff : CompProperties
	{
		public float Range;

		public HediffDef Hediff;

		public bool Enemy = false;

		public CompProperties_TotemAddHediff()
		{
			compClass = typeof(Comp_TotemAddHediff);
		}
	}

	public class Comp_TotemAddHediff : ThingComp
	{
		public CompProperties_TotemAddHediff Props => (CompProperties_TotemAddHediff)props;
		public override void CompTick()
		{
			if (this.parent.IsHashIntervalTick(300))
			{
				CompRefuelable fuel = this.parent.GetComp<CompRefuelable>();
				if (fuel.HasFuel)
				{
					Map map = this.parent.Map;
					if (map != null)
					{
						IEnumerable<Pawn> pawns;
						if (Props.Enemy == false)
                        {
							pawns = map.mapPawns.AllPawnsSpawned.Where(x => x.Position.DistanceTo(this.parent.Position) <= Props.Range && this.parent.Faction == x.Faction);
						} else
                        {
							pawns = map.mapPawns.AllPawnsSpawned.Where(x => x.Position.DistanceTo(this.parent.Position) <= Props.Range && x.HostileTo(this.parent));
						}
						if (!pawns.EnumerableNullOrEmpty())
						{
							Effecter_BEPCore.BEP_UseSkill_D.Spawn(this.parent.Position, this.parent.Map, Vector3.zero);
							foreach (Pawn pawn in pawns)
							{
								pawn.health.AddHediff(Props.Hediff);
							}
						}
					}
				}
			}
		}
	}

}
