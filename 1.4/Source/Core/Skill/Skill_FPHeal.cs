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
    // FPの割合回復
    public class Skill_FPHeal : SkillBase
    {
        int Amount;

        public Skill_FPHeal()
        {
        }

        public Skill_FPHeal(int Amount)
        {
            this.Amount = Amount;
        }

        public override void SkillUse(Pawn pawn, LocalTargetInfo target)
        {
            Comp_LitF comp = pawn.GetComp<Comp_LitF>();
            if (comp.UseSkill.TargetAreaRange != -1f && comp.UseSkill.IsArea)
            {
                IEnumerable<Thing> Enemys = pawn.Map.listerThings.AllThings.Where(x => x.Position.DistanceTo(target.Cell) <= comp.UseSkill.TargetAreaRange && (comp.UseSkill.PenetratingWall == PenetratingWall.Range || comp.UseSkill.PenetratingWall == PenetratingWall.Both || GenSight.LineOfSight(target.Cell, x.Position, x.Map)));
                if (!Enemys.EnumerableNullOrEmpty())
                {
                    for (int i = Enemys.Count() - 1; i >= 0; i--)
                    {
                        Thing Enemy = Enemys.ElementAt(i);
                        if (Enemy != pawn)
                        {
                            if (Enemy.TryGetComp<Comp_LitF>() == null)
                            {
                                continue;
                            }
                            Comp_LitF tar_comp = Enemy.TryGetComp<Comp_LitF>();
                            if (comp.UseSkill.UseMote != null)
                            {
                                MoteMaker.MakeStaticMote(Enemy.TrueCenter(), Enemy.Map, comp.UseSkill.UseMote, 1f);
                            }
                            if (comp.UseSkill.UseSound != null)
                            {
                                comp.UseSkill.UseSound.PlayOneShot(new TargetInfo(Enemy.Position, Enemy.Map, false));
                            }
                            if (comp.UseSkill.UseEffect != null)
                            {
                                comp.UseSkill.UseEffect.Spawn(Enemy.Position, Enemy.Map, Vector3.zero);
                            }
                            int healamo = (int)(((float)tar_comp.MaxFP / 100f) * Amount);
                            tar_comp.FPHeal(healamo);
                        }
                    }
                }
            } else
            {
                if (target.Thing != null)
                {
                    if (target.Thing.TryGetComp<Comp_LitF>() == null)
                    {
                        return;
                    }
                    if (comp.UseSkill.UseMote != null)
                    {
                        MoteMaker.MakeStaticMote(target.Thing.TrueCenter(), target.Thing.Map, comp.UseSkill.UseMote, 1f);
                    }
                    if (comp.UseSkill.UseSound != null)
                    {
                        comp.UseSkill.UseSound.PlayOneShot(new TargetInfo(target.Thing.Position, target.Thing.Map, false));
                    }
                    if (comp.UseSkill.UseEffect != null)
                    {
                        comp.UseSkill.UseEffect.Spawn(target.Thing.Position, target.Thing.Map, Vector3.zero);
                    }
                    Comp_LitF tar_comp = target.Thing.TryGetComp<Comp_LitF>();
                    int healamo = (int)(((float)tar_comp.MaxFP / 100f) * Amount);
                    tar_comp.FPHeal(healamo);
                }
            }
        }
    }
}
