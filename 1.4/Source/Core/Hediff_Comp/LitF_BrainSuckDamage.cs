using RimWorld;
using System.Linq;
using Verse;

namespace LittleFairy_Race
{
    public class LitF_BrainSuckDamage : DamageWorker_AddInjury
    {
        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);

            if (!pawn.RaceProps.body.AllParts.Where(x => x.def == BodyPartDefOf.Brain).EnumerableNullOrEmpty())
            {
                if (!pawn.RaceProps.IsMechanoid)
                {
                    if (!pawn.Dead)
                    {
                        BodyPartRecord brain = pawn.RaceProps.body.AllParts.Where(x => x.def == BodyPartDefOf.Brain).FirstOrDefault();
                        if (brain != null)
                        {
                            pawn.TakeDamage(new DamageInfo(DamageDefOf.Bite, (totalDamage / 0.5f) + Rand.Range(1.0f, 5.0f), 5.0f, -1, dinfo.IntendedTarget, brain));
                        }
                    }
                }
            }
        }
    }
}
