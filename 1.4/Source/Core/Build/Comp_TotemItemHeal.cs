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
	public class CompProperties_TotemItemHeal : CompProperties
	{
		public CompProperties_TotemItemHeal()
		{
			compClass = typeof(Comp_TotemItemHeal);
		}
	}

	public class Comp_TotemItemHeal : ThingComp
	{
		public CompProperties_TotemItemHeal Props => (CompProperties_TotemItemHeal)props;
		public override void CompTick()
		{
			if (Find.TickManager.TicksGame % 1200 == 0)
			{
				CompRefuelable fuel = this.parent.GetComp<CompRefuelable>();
				if (fuel.HasFuel)
				{
					Map map = this.parent.Map;
					if (map != null)
					{
                        Effecter_BEPCore.BEP_UseSkill_D.Spawn(this.parent.Position, this.parent.Map, Vector3.zero);
                        IEnumerable<Thing> Items = this.parent.Map.listerThings.AllThings.Where(x
							=> x.Position.DistanceTo(this.parent.Position) <= 10.9f);
						IEnumerable<Thing> Items_Rotten = Items.Where(x => x.TryGetComp<CompRottable>() != null);
						if (!Items_Rotten.EnumerableNullOrEmpty())
						{
							foreach (Thing tag in Items_Rotten)
							{
								CompRottable rot = tag.TryGetComp<CompRottable>();
                                if (rot.Stage < RotStage.Rotting)
								{
									rot.RotProgress = 0f;
								}
                            }
						}
                        IEnumerable<Thing> Items_MeatWall = Items.Where(x => x.def.defName == "LitF_MeatWall");
                        if (!Items_MeatWall.EnumerableNullOrEmpty())
                        {
                            foreach (Thing tag in Items_MeatWall)
                            {
								tag.HitPoints = Math.Min(3000, tag.HitPoints + 600);
                            }
                        }
                        IEnumerable<Thing> Items_HasHP = Items.Where(x => x.def.useHitPoints && x.def.defName != "LitF_MeatWall");
                        if (!Items_HasHP.EnumerableNullOrEmpty())
                        {
                            foreach (Thing tag in Items_HasHP)
                            {
                                tag.HitPoints = Math.Min(tag.MaxHitPoints, tag.HitPoints + 1);
                            }
                        }
                    }
				}
			}
		}
	}
}
