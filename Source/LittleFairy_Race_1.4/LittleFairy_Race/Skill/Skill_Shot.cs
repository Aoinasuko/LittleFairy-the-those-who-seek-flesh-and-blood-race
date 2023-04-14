using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LittleFairy_Race
{
    // ターゲットに射撃
    public class Skill_ShotSingle : SkillBase
    {
        ThingDef Thing;

        public Skill_ShotSingle()
        {
        }

        public Skill_ShotSingle(ThingDef Thing)
        {
            this.Thing = Thing;
        }

        public override void SkillUse(Pawn pawn, LocalTargetInfo target)
        {
            Comp_LitF comp = pawn.GetComp<Comp_LitF>();
            if (target.Cell != null)
            {
                if (comp.UseSkill.UseMote != null)
                {
                    MoteMaker.MakeStaticMote(pawn.TrueCenter(), pawn.Map, comp.UseSkill.UseMote, 1f);
                }
                if (comp.UseSkill.UseSound != null)
                {
                    comp.UseSkill.UseSound.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                }
                Projectile shot = (Projectile)GenSpawn.Spawn(Thing, pawn.Position, pawn.Map);
                Vector3 drawPos = pawn.DrawPos;
                if (target.Thing != null)
                {
                    IntVec3 c = target.Thing.Position;                    
                    shot.Launch(pawn, drawPos, c, pawn.Position, ProjectileHitFlags.All, false, null);
                } else
                {
                    IntVec3 c = target.Cell;
                    shot.Launch(pawn, drawPos, c, pawn.Position, ProjectileHitFlags.All, false, null);
                }
            }
        }
    }
}
