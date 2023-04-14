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
    public class Skill_SkyFaller : SkillBase
    {
        ThingDef FallThing;
        DamageFilter Filter = DamageFilter.Pawn;

        public Skill_SkyFaller()
        {
        }

        public Skill_SkyFaller(ThingDef FallThing, DamageFilter Filter)
        {
            this.FallThing = FallThing;
            this.Filter = Filter;
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
                            if (Filter == DamageFilter.Pawn && Enemy as Pawn == null)
                            {
                                continue;
                            }
                            if (Filter == DamageFilter.Thing && Enemy as Pawn != null)
                            {
                                continue;
                            }
                            if (comp.UseSkill.UseMote != null)
                            {
                                MoteMaker.MakeStaticMote(Enemy.TrueCenter(), Enemy.Map, comp.UseSkill.UseMote, 1f);
                            }
                            if (comp.UseSkill.UseSound != null)
                            {
                                comp.UseSkill.UseSound.PlayOneShot(new TargetInfo(Enemy.Position, Enemy.Map, false));
                            }
                            SkyfallerMaker.SpawnSkyfaller(FallThing, Enemy.Position, Enemy.Map);
                        }
                    }
                }
            }
            else
            {
                if (target.Thing != null)
                {
                    if (Filter == DamageFilter.Pawn && target.Thing as Pawn == null)
                    {
                        return;
                    }
                    if (Filter == DamageFilter.Thing && target.Thing as Pawn != null)
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
                    SkyfallerMaker.SpawnSkyfaller(FallThing, target.Thing.Position, target.Thing.Map);
                }
            }
        }
    }
}
