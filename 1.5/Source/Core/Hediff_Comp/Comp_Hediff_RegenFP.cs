using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LittleFairy_Race
{
	public class HediffCompProperties_RegenFP : HediffCompProperties
	{
		public int RegenTime = 60;
		public int Amount = 1;

		public HediffCompProperties_RegenFP()
		{
			compClass = typeof(Comp_Hediff_RegenFP);
		}
	}

	public class Comp_Hediff_RegenFP : HediffComp
	{
		public HediffCompProperties_RegenFP Props => (HediffCompProperties_RegenFP)props;
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (this.Pawn.IsHashIntervalTick(Props.RegenTime))
            {
				if (this.Pawn.TryGetComp<Comp_LitF>() != null)
                {
					this.Pawn.TryGetComp<Comp_LitF>().FPHeal(Props.Amount);
				}
            }
		}
	}
}
