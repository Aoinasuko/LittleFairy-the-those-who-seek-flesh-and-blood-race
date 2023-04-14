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
	public class CompProperties_TotemPlantGrowth : CompProperties
	{
		public bool OnlyWood = true;

		public CompProperties_TotemPlantGrowth()
		{
			compClass = typeof(Comp_TotemPlantGrowth);
		}
	}

	public class Comp_TotemPlantGrowth : ThingComp
	{
		public CompProperties_TotemPlantGrowth Props => (CompProperties_TotemPlantGrowth)props;
		public override void CompTick()
		{
			if (Find.TickManager.TicksGame % 2500 == 0)
			{
				CompRefuelable fuel = this.parent.GetComp<CompRefuelable>();
				if (fuel.HasFuel)
				{
					Map map = this.parent.Map;
					if (map != null)
					{
						IEnumerable<Thing> Crops = this.parent.Map.listerThings.AllThings.Where(x
							=> x.Position.DistanceTo(this.parent.Position) <= 10.9f
							&& x.Position.GetPlant(this.parent.Map) != null
							&& (Props.OnlyWood == false || x.Position.GetPlant(this.parent.Map).def.plant.harvestTag == "Wood") && x.Position.GetPlant(this.parent.Map).GrowthRate > 0.00f);
						if (Crops != null)
						{
							foreach (Thing crop in Crops)
							{
								if (crop.Position.GetPlant(this.parent.Map).LifeStage != PlantLifeStage.Sowing)
								{
									if (crop.Position.GetPlant(this.parent.Map).Growth < 1.0f)
									{
										MoteMaker.MakeStaticMote(crop.TrueCenter(), crop.Map, Mote_LitF.LitF_Mote_Chanting, 1f);
										crop.Position.GetPlant(this.parent.Map).Growth += 0.02f * crop.Position.GetPlant(this.parent.Map).GrowthRate;
									}
								}
							}
						}
					}
				}
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Growing>() != null)
			{
				Command_Action command_Action = new Command_Action();
				command_Action.action = MakeMatchingGrowZone;
				command_Action.icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Growing");
				command_Action.defaultLabel = "CommandSunLampMakeGrowingZoneLabel".Translate();
				yield return command_Action;
			}
		}

		public void MakeMatchingGrowZone()
		{
			IEnumerable<IntVec3> GrowableCells = GenRadial.RadialCellsAround(this.parent.Position, this.parent.def.specialDisplayRadius, useCenter: true);

			Designator designator = DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Growing>();
			designator.DesignateMultiCell(GrowableCells.Where((IntVec3 tempCell) => designator.CanDesignateCell(tempCell).Accepted));
		}
	}
}
