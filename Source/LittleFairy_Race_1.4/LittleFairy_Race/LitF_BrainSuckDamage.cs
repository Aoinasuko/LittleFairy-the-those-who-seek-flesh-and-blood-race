using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleFairy_Race
{
    public class LitF_BrainSuckDamage : DamageWorker_AddInjury
    {
        public override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
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
                            pawn.TakeDamage(new DamageInfo(DamageDefOf.Bite, Rand.Range(3.0f, 8.0f), 5.0f, -1, dinfo.IntendedTarget, brain));
                            Pawn p = (Pawn)dinfo.Instigator;
                            if (pawn.Dead)
                            {
                                if (!pawn.Corpse.Destroyed)
                                {
                                    pawn.Corpse.Destroy();
                                    Sound_LitF.LitF_UseFP.PlayOneShot(new TargetInfo(p.Position, p.Map, false));
                                    MoteMaker.MakeStaticMote(p.TrueCenter(), p.Map, Motes_LitF.LitF_Mote_Buff, 1f);
                                    Hediff hediff = p.health.AddHediff(Hediff_LitF.LitF_Vitality);
                                    hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = 7500;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
