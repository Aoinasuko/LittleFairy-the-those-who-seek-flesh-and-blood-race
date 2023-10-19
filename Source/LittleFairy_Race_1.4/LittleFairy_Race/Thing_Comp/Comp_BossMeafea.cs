using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LittleFairy_Race
{
	public class CompProperties_BossMeafea : CompProperties
	{
		public CompProperties_BossMeafea()
		{
			compClass = typeof(Comp_BossMeafea);
		}
	}

	public class Comp_BossMeafea : ThingComp
	{
        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            if (absorbed == false)
            {
                if (dinfo.Def == DamageDefOf.EMP || dinfo.Def == DamageDefOf.Stun)
                {
                    absorbed = true;
                    return;
                }
                if (this.parent.TryGetComp<Comp_BossHP>().GetHP() > 0)
                {
                    if (dinfo.Amount > 30.0f)
                    {
                        dinfo.SetAmount(30.0f);
                    }
                    Pawn pawn = (Pawn)this.parent;
                    pawn.health.AddHediff(Hediff_LitF.LitF_Counterattack);
                    int DamageAmount = Math.Max((int)(dinfo.Amount * pawn.GetStatValue(StatDefOf.IncomingDamageFactor)), 1);
                    Effecter_BEPCore.BEP_Parry_B.Spawn(pawn.Position, pawn.Map, Vector3.zero);
                    FPDamage(DamageAmount);
                    absorbed = true;
                }
            }
        }

        public void FPDamage(float value)
        {
            Pawn pawn = (Pawn)this.parent;
            this.parent.TryGetComp<Comp_BossHP>().HPDamage((int)value);
            if (this.parent.TryGetComp<Comp_BossHP>().GetHP() > 0)
            {
                return;
            }
            Sound_LitF.LitF_Break.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
            if (pawn.Spawned)
            {
                if (pawn.Map != null)
                {
                    Effecter_BEPCore.BEP_UseSkill_D.Spawn(pawn.Position, pawn.Map, Vector3.zero);
                    if (pawn.Faction?.IsPlayer ?? false || pawn.Faction == null)
                    {
                        pawn.Destroy();
                        return;
                    }                    
                    // アイテムをドロップする
                    Thing thing = ThingMaker.MakeThing(ThingDef.Named("LitF_WK_Blood"));
                    GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                }
            }
            pawn.Destroy();
            return;
        }

    }

}
