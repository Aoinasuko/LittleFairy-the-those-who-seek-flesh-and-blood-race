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
    public class Skill_SummonThing : SkillBase
    {
        ThingDef Thing;
        DamageFilter Filter = DamageFilter.Pawn;

        public Skill_SummonThing()
        {
        }

        public Skill_SummonThing(ThingDef Thing, DamageFilter Filter)
        {
            this.Thing = Thing;
            this.Filter = Filter;
        }

        public override void SkillUse(Pawn pawn, LocalTargetInfo target)
        {
            Comp_LitF comp = pawn.GetComp<Comp_LitF>();
            if (comp.UseSkill.Target == SkillTarget.Locate)
            {
                if (comp.UseSkill.UseMote != null)
                {
                    MoteMaker.MakeStaticMote(target.Cell, pawn.Map, comp.UseSkill.UseMote, 1f);
                }
                if (comp.UseSkill.UseSound != null)
                {
                    comp.UseSkill.UseSound.PlayOneShot(new TargetInfo(target.Cell, pawn.Map, false));
                }
                if (comp.UseSkill.UseEffect != null)
                {
                    comp.UseSkill.UseEffect.Spawn(target.Cell, pawn.Map, Vector3.zero);
                }
                Thing thing = GenSpawn.Spawn(Thing, target.Cell, pawn.Map);
                if (thing.TryGetComp<Comp_SummonThing>() != null)
                {
                    Comp_SummonThing comp_thing = thing.TryGetComp<Comp_SummonThing>();
                    comp_thing.SetLife();
                    comp_thing.SetOwner(pawn);
                    if (target.Thing != null)
                    {
                        comp_thing.SetTarget(target.Thing);
                    }
                }
            }
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
                            if (comp.UseSkill.UseEffect != null)
                            {
                                comp.UseSkill.UseEffect.Spawn(Enemy.Position, Enemy.Map, Vector3.zero);
                            }
                            Thing thing = GenSpawn.Spawn(Thing, Enemy.Position, Enemy.Map);
                            if (thing.TryGetComp<Comp_SummonThing>() != null)
                            {
                                Comp_SummonThing comp_thing = thing.TryGetComp<Comp_SummonThing>();
                                comp_thing.SetLife();
                                comp_thing.SetOwner(pawn);
                                if (target.Thing != null)
                                {
                                    comp_thing.SetTarget(target.Thing);
                                }
                            }
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
                    if (comp.UseSkill.UseEffect != null)
                    {
                        comp.UseSkill.UseEffect.Spawn(target.Thing.Position, target.Thing.Map, Vector3.zero);
                    }
                    Thing thing = GenSpawn.Spawn(Thing, target.Thing.Position, target.Thing.Map);
                    if (thing.TryGetComp<Comp_SummonThing>() != null)
                    {
                        Comp_SummonThing comp_thing = thing.TryGetComp<Comp_SummonThing>();
                        comp_thing.SetLife();
                        comp_thing.SetOwner(pawn);
                        if (target.Thing != null)
                        {
                            comp_thing.SetTarget(target.Thing);
                        }
                    }
                }
            }
        }
    }
}
