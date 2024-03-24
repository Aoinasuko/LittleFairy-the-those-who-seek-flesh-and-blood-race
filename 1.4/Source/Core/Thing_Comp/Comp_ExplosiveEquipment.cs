using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LittleFairy_Race
{
	public class CompProperties_ExplosiveEquipment : CompProperties
	{
		public float Range = 5.9f;
		public int Damage = 100;

		public CompProperties_ExplosiveEquipment()
		{
			compClass = typeof(Comp_ExplosiveEquipment);
		}
	}

	public class Comp_ExplosiveEquipment : ThingComp
	{
		public CompProperties_ExplosiveEquipment Props => (CompProperties_ExplosiveEquipment)props;

		public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
		{
			Pawn pawn = ((Apparel)this.parent).Wearer;
			Gizmo Gizmo = null;
			if (pawn != null && pawn.Map != null)
            {
				if (pawn.Faction?.IsPlayer ?? false)
                {
					Gizmo = new Command_Action
					{
						defaultLabel = "LitF.UI.labelExplosiveEquipment".Translate(),
						icon = Graphic_LitF.ExplosiveEquipment_Icon,
						defaultDesc = "LitF.UI.ExplosiveEquipment".Translate(),
						disabled = false,
						disabledReason = "",
						action = delegate
						{
							IntVec3 position = pawn.Position;
							Map map = pawn.Map;
							Thing instigator = null;
							GenExplosion.DoExplosion(position, map, Props.Range, DamageDefOf.Bomb, instigator, Props.Damage, 5f, null, null, null, null, null, 1f, 1, null, false, null, 0, 0, 0, false);

							if (pawn != null & !pawn.Destroyed)
							{
								pawn.Kill(null);
								if (pawn.Dead)
								{
									if (!pawn.Destroyed)
									{
										pawn.Destroy();
									}
									pawn.Corpse.Destroy();
								}
							}
						},
						onHover = delegate
						{
							Calc_UI.DrawRadiusRing(pawn.Position, Props.Range, Color.white, false, null);
						}
					};
					yield return Gizmo;
				}			
			}
            
        }
	}

	public class CompProperties_Oppression : CompProperties
	{
		public CompProperties_Oppression()
		{
			compClass = typeof(Comp_Oppression);
		}
	}

	public class Comp_Oppression : ThingComp
	{
		public CompProperties_Oppression Props => (CompProperties_Oppression)props;

		public override void CompTick()
		{
			if (this.parent.IsHashIntervalTick(10))
            {
				if (ModLister.IdeologyInstalled)
                {
					Pawn pawn = ((Apparel)this.parent).Wearer;
					if (pawn != null)
                    {
						if (pawn.IsSlave)
                        {
							pawn.needs.TryGetNeed<Need_Suppression>().CurLevel = pawn.needs.TryGetNeed<Need_Suppression>().MaxLevel;
						}
                    }
                }
            }
		}
	}

}
