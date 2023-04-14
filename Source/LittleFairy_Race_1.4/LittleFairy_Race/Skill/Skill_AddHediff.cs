using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace LittleFairy_Race
{
    public class Skill_AddHediff : SkillBase
    {
        HediffDef GetHediff;

        public Skill_AddHediff()
        {
        }

        public Skill_AddHediff(HediffDef GetHediff)
        {
            this.GetHediff = GetHediff;
        }

        public override void SkillUse(Pawn pawn, LocalTargetInfo target)
        {
            Comp_LitF comp = pawn.GetComp<Comp_LitF>();
            if (comp.UseSkill.TargetAreaRange != -1f && comp.UseSkill.IsArea)
            {
                IEnumerable<Thing> Enemys = pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x.Position.DistanceTo(target.Cell) <= comp.UseSkill.TargetAreaRange && (comp.UseSkill.PenetratingWall == PenetratingWall.Range || comp.UseSkill.PenetratingWall == PenetratingWall.Both || GenSight.LineOfSight(target.Cell, x.Position, x.Map)));
                if (!Enemys.EnumerableNullOrEmpty())
                {
                    for (int i = Enemys.Count() - 1; i >= 0; i--)
                    {
                        Thing Enemy = Enemys.ElementAt(i);
                        if (Enemy as Pawn != null)
                        {
                            if (Enemy != pawn)
                            {
                                Pawn EmemyPawn = Enemy as Pawn;
                                if (comp.UseSkill.UseMote != null)
                                {
                                    MoteMaker.MakeStaticMote(Enemy.TrueCenter(), Enemy.Map, comp.UseSkill.UseMote, 1f);
                                }
                                if (comp.UseSkill.UseSound != null)
                                {
                                    comp.UseSkill.UseSound.PlayOneShot(new TargetInfo(Enemy.Position, Enemy.Map, false));
                                }
                                EmemyPawn.health.AddHediff(GetHediff);
                            }
                        }
                    }
                }
            }
            else
            {
                if (target.Pawn != null)
                {
                    if (comp.UseSkill.UseMote != null)
                    {
                        MoteMaker.MakeStaticMote(target.Thing.TrueCenter(), target.Thing.Map, comp.UseSkill.UseMote, 1f);
                    }
                    if (comp.UseSkill.UseSound != null)
                    {
                        comp.UseSkill.UseSound.PlayOneShot(new TargetInfo(target.Thing.Position, target.Thing.Map, false));
                    }
                    target.Pawn.health.AddHediff(GetHediff);
                }
            }
        }
    }
}
