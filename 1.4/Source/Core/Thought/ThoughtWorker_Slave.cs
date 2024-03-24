using RimWorld;
using Verse;

namespace LittleFairy_Race
{
	public class ThoughtWorker_Slave : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!ModLister.IdeologyInstalled)
			{
				return ThoughtState.Inactive;
			}
			if (p.def.defName == "LittleFairy_Pawn")
			{
				if (p.IsSlave)
				{
					return ThoughtState.ActiveDefault;
				}
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			int multi = 1;
			Need_Suppression need_Suppression = p.needs.TryGetNeed<Need_Suppression>();
			multi = (int)(need_Suppression.CurLevel * 100) / 10;
			return multi;
		}
	}
}
