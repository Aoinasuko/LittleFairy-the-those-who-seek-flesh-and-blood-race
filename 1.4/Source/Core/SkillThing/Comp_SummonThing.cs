using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LittleFairy_Race
{

	public class CompProperties_SummonThing : CompProperties
	{
		public int LifeTick;

		public CompProperties_SummonThing()
		{
			compClass = typeof(Comp_SummonThing);
		}
	}

	public class Comp_SummonThing : ThingComp
	{
		public CompProperties_SummonThing Props => (CompProperties_SummonThing)props;

		public int life = 600;
		public Pawn owner = null;
		public Thing target = null;

		public override void CompTick()
		{
			life--;
			if (life < 0)
			{
				this.parent.Destroy();
			}
		}

		public void SetLife()
		{
			life = Props.LifeTick;
		}

		public void SetOwner(Pawn pawn)
		{
			owner = pawn;
		}

		public void SetTarget(Thing thing)
		{
			target = thing;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref life, "life", 0);
			Scribe_References.Look(ref owner, "owner", false);
			Scribe_References.Look(ref target, "target", false);
		}
	}

}
