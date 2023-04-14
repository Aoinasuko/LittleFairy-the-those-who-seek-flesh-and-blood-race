using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
							MoteMaker.MakeStaticMote(this.parent.TrueCenter(), this.parent.Map, Mote_LitF.LitF_Mote_Chanting, 1f);
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
