using AlienRace;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace LittleFairy_Race
{

    [StaticConstructorOnStartup]
    static class LitF_Harmony
    {
        static LitF_Harmony()
        {
            var harmony = new Harmony("BEP.LitF");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Defパッチ
            if (ModLister.IdeologyInstalled)
            {
                foreach (FactionDef x in DefDatabase<FactionDef>.AllDefsListForReading.Where(x => x.allowedMemes.NullOrEmpty()))
                {
                    if (x.disallowedMemes == null)
                    {
                        x.disallowedMemes = new List<MemeDef>();
                    }
                    x.disallowedMemes.Add(Ideology_LitF.LitF_Fairy);
                }
            }

            // 継承
            SkillDef baseskill = DefDatabase<SkillDef>.AllDefsListForReading.Where(x => x.defName == "LitF_Succession_Base").First().Clone();
            //HediffDef basehediff = DefDatabase<HediffDef>.AllDefsListForReading.Where(x => x.defName == "LitF_Succession_Base").First().LitF_Clone();
            List<ThingDef_AlienRace> things = DefDatabase<ThingDef_AlienRace>.AllDefsListForReading.Where(x => x.defName != "LittleFairy_Pawn").ToList();
            foreach (ThingDef_AlienRace ali in things)
            {
                // Hediff作成
                HediffDef hediff = new HediffDef(); //basehediff.LitF_Clone();
                hediff.defName = "LitF_Succession_" + ali.defName;
                hediff.label = "LitF.Add.Succession".Translate() + "(" + ali.label + ")";
                hediff.description = "LitF.Add.Succession_description".Translate();
                hediff.defaultLabelColor = new Color(0.8f, 0.4f, 1.0f);
                hediff.isBad = false;
                hediff.stages = new List<HediffStage>();
                hediff.stages.Add(new HediffStage());
                hediff.stages.First().statOffsets = new List<StatModifier>();
                List<StatModifier> modi = hediff.stages.First().statOffsets;
                ThingDef human = DefDatabase<ThingDef_AlienRace>.AllDefsListForReading.Where(x => x.defName == "Human").First();
                // 人間の能力値と比較
                foreach (StatModifier statBase in ali.statBases)
                {
                    if (statBase.stat == StatDefOf.MarketValue || statBase.stat == StatDefOf.Mass || statBase.stat == StatDefOf.MeatAmount || statBase.stat == StatDefOf.LeatherAmount)
                    {
                        continue;
                    }
                    // 人間が持っていないなら、デフォルト値と比較する
                    if (!human.statBases.Where(x => x.stat == statBase.stat).EnumerableNullOrEmpty())
                    {
                        StatModifier stat = ali.statBases.Find(x => x.stat == statBase.stat);
                        StatModifier mod = new StatModifier();
                        mod.stat = statBase.stat;
                        mod.value = stat.value - statBase.value;
                        if (mod.value != 0.00f)
                        {
                            modi.Add(mod);
                        }
                    } else
                    {
                        float stat = statBase.stat.defaultBaseValue;
                        StatModifier mod = new StatModifier();
                        mod.stat = statBase.stat;
                        mod.value = (statBase.value - stat) * LitF_Config.Succession_Power;
                        if (mod.value != 0.00f)
                        {
                            modi.Add(mod);
                        }
                    }
                }
                // 作成したHediffを追加
                hediff.modContentPack = baseskill.modContentPack;
                hediff.generated = true;
                hediff.modContentPack?.AddDef(hediff, "ImpliedDefs");
                hediff.PostLoad();
                DefDatabase<HediffDef>.Add(hediff);
                // スキル作成
                SkillDef makeskill = baseskill.Clone();
                makeskill.label = "LitF.Add.Succession".Translate() + "(" + ali.label + ")";
                makeskill.defName = "LitF_Succession_" + ali.defName;
                makeskill.AddHediff = hediff;
                makeskill.IsCommonSkill = true;
                makeskill.TargetPawn = new List<string>();
                makeskill.TargetPawn.Add(ali.defName);
                makeskill.CommonRate = LitF_Config.Succession_Rate;
                // 作成したスキルを追加
                makeskill.generated = true;
                makeskill.PostLoad();
                DefDatabase<SkillDef>.Add(makeskill);                
            }
            if (LitF_Config.Add_Animal_Succession)
            {
                List<ThingDef> animals = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => (x.race?.Animal ?? false) == true && x as ThingDef_AlienRace == null).ToList();
                foreach (ThingDef ali in animals)
                {
                    // Hediff作成
                    HediffDef hediff = new HediffDef(); //basehediff.LitF_Clone();
                    hediff.defName = "LitF_Succession_" + ali.defName;
                    hediff.label = "LitF.Add.Succession".Translate() + "(" + ali.label + ")";
                    hediff.description = "LitF.Add.Succession_description".Translate();
                    hediff.defaultLabelColor = new Color(0.8f, 0.4f, 1.0f);
                    hediff.isBad = false;
                    hediff.stages = new List<HediffStage>();
                    hediff.stages.Add(new HediffStage());
                    hediff.stages.First().statOffsets = new List<StatModifier>();
                    List<StatModifier> modi = hediff.stages.First().statOffsets;
                    ThingDef human = DefDatabase<ThingDef_AlienRace>.AllDefsListForReading.Where(x => x.defName == "Human").First();
                    // 人間の能力値と比較
                    foreach (StatModifier statBase in ali.statBases)
                    {
                        if (statBase.stat == StatDefOf.MarketValue || statBase.stat == StatDefOf.Mass || statBase.stat == StatDefOf.MeatAmount || statBase.stat == StatDefOf.LeatherAmount)
                        {
                            continue;
                        }
                        // 人間が持っていないなら、デフォルト値と比較する
                        if (!human.statBases.Where(x => x.stat == statBase.stat).EnumerableNullOrEmpty())
                        {
                            StatModifier stat = ali.statBases.Find(x => x.stat == statBase.stat);
                            StatModifier mod = new StatModifier();
                            mod.stat = statBase.stat;
                            mod.value = stat.value - statBase.value;
                            if (mod.value != 0.00f)
                            {
                                modi.Add(mod);
                            }
                        }
                        else
                        {
                            float stat = statBase.stat.defaultBaseValue;
                            StatModifier mod = new StatModifier();
                            mod.stat = statBase.stat;
                            mod.value = (statBase.value - stat) * LitF_Config.Succession_Power;
                            if (mod.value != 0.00f)
                            {
                                modi.Add(mod);
                            }
                        }
                    }
                    // 作成したHediffを追加
                    hediff.modContentPack = baseskill.modContentPack;
                    hediff.generated = true;
                    hediff.modContentPack?.AddDef(hediff, "ImpliedDefs");
                    hediff.PostLoad();
                    DefDatabase<HediffDef>.Add(hediff);
                    // スキル作成
                    SkillDef makeskill = baseskill.Clone();
                    makeskill.label = "LitF.Add.Succession".Translate() + "(" + ali.label + ")";
                    makeskill.defName = "LitF_Succession_" + ali.defName;
                    makeskill.AddHediff = hediff;
                    makeskill.TargetPawn = new List<string>();
                    makeskill.TargetPawn.Add(ali.defName);
                    makeskill.CommonRate = LitF_Config.Succession_Rate;
                    // 作成したスキルを追加
                    makeskill.generated = true;
                    DefDatabase<SkillDef>.Add(makeskill);
                }
            }
        }
    }

    // 肉を人間扱いにしなくなる
    [HarmonyPatch(typeof(FoodUtility), "IsHumanlikeCorpseOrHumanlikeMeat")]
    static class LitF_FixFoodUtility
    {
        [HarmonyPrefix]
        public static bool Fix_IsHumanlike(ref bool __result, ref Thing source, ref ThingDef foodDef)
        {
            Corpse corpse = source as Corpse;
            if (corpse != null && (corpse.InnerPawn.def.race.FleshType == FleshType_LitF.LitF_Fairy))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FoodUtility), "GetMeatSourceCategoryFromCorpse")]
    [HarmonyPatch(new Type[]
    {
        typeof(Thing)
    })]
    static class LitF_FixFoodUtility_GetMeatSourceCategoryCorpse
    {
        [HarmonyPrefix]
        public static bool Fix_IsHumanlike(ref MeatSourceCategory __result, Thing thing)
        {
            Corpse corpse = thing as Corpse;
            if (corpse != null && corpse.InnerPawn.def.race.FleshType == FleshType_LitF.LitF_Fairy)
            {
                __result = MeatSourceCategory.Undefined;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FoodUtility), "GetMeatSourceCategory")]
    [HarmonyPatch(new Type[]
    {
        typeof(ThingDef)
    })]
    static class LitF_FixFoodUtility_GetMeatSourceCategory
    {
        [HarmonyPrefix]
        public static bool Fix_IsHumanlike(ref MeatSourceCategory __result, ThingDef source)
        {
            if (source.ingestible == null)
            {
                return true;
            }
            if ((source.ingestible.foodType & FoodTypeFlags.Meat) == FoodTypeFlags.Meat)
            {
                if (source.ingestible.sourceDef != null)
                {
                    if (source.ingestible.sourceDef.race.FleshType == FleshType_LitF.LitF_Fairy)
                    {
                        __result = MeatSourceCategory.Undefined;
                        return false;
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Corpse), "ButcherProducts")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(float) })]
    static class LitF_Corpse_Patch
    {

        [HarmonyBefore(new string[] { "rimworld.erdelf.alien_race.main" })]
        [HarmonyPrefix]
        static bool Prefix(Pawn butcher, float efficiency, ref IEnumerable<Thing> __result, Corpse __instance)
        {

            if (__instance.InnerPawn.def.race.FleshType == FleshType_LitF.LitF_Fairy)
            {
                Pawn corpse = __instance.InnerPawn;
                IEnumerable<Thing> enumerable = corpse.ButcherProducts(butcher, efficiency);
                if (corpse.RaceProps.BloodDef != null)
                {
                    FilthMaker.TryMakeFilth(butcher.Position, butcher.Map, corpse.RaceProps.BloodDef, corpse.LabelIndefinite());
                }
                __result = enumerable;
                return false;
            }
            return true;
        }

    }

    // 死体に耐性をつける
    [HarmonyPatch(typeof(CorpseObsessionMentalStateUtility), "IsCorpseValid")]
    [HarmonyPatch(new Type[]
    {
        typeof(Corpse),
        typeof(Pawn),
        typeof(bool),
    })]
    internal static class CorpseObsession_Patch
    {

        [HarmonyPrefix]
        static bool Prefix(ref bool __result, Corpse corpse, Pawn pawn, bool ignoreReachability)
        {
            if (corpse == null)
            {
                return true;
            }
            if (corpse.InnerPawn.def.defName == "LittleFairy_Pawn")
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Corpse), "GiveObservedHistoryEvent")]
    [HarmonyPatch(new Type[]
    {
            typeof(Pawn),
    })]
    internal static class Corpse_Patch
    {

        [HarmonyPrefix]
        static bool Prefix(ref Corpse __instance, ref HistoryEventDef __result, Pawn observer)
        {
            if (__instance.InnerPawn.def.defName == "LittleFairy_Pawn")
            {
                if (!observer.story.traits.HasTrait(TraitDefOf.Kind))
                {
                    __result = null;
                    return false;
                }
            }
            return true;
        }
    }



    // リトルフェアリーの場合、バーサーカー状態なら近接クールダウンを倍進める
    [HarmonyPatch(typeof(Stance_Cooldown), "StanceDraw")]
    static class Patch_Test
    {
        [HarmonyPostfix]
        public static void Postfix(ref Stance_Cooldown __instance)
        {
            if (__instance.ticksLeft > 0)
            {
                if (__instance.stanceTracker.pawn.def.defName == "LittleFairy_Pawn")
                {
                    if (__instance.stanceTracker.pawn.health.hediffSet.HasHediff(Hediff_LitF.LitF_Skill_BerserkerCharge))
                    {
                        __instance.ticksLeft -= 1;
                    }
                }
            }
            return;
        }
    }

    // リトルフェアリーを苗床にぶちこむ
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    [HarmonyPatch(new Type[]
    {
                typeof(Vector3),
                typeof(Pawn),
                typeof(List<FloatMenuOption>),
    })]
    static class Fix_AddHumanlikeOrders
    {

        [HarmonyPrefix]
        static bool Prefix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                foreach (LocalTargetInfo localTargetInfo3 in GenUI.TargetsAt(clickPos, new TargetingParameters
                {
                    canTargetPawns = true,
                    canTargetMechs = false,
                    canTargetBuildings = false,
                    onlyTargetIncapacitatedPawns = false
                }, true))
                {
                    LocalTargetInfo localTargetInfo4 = localTargetInfo3;
                    Pawn victim = (Pawn)localTargetInfo4.Thing;
                    if (victim.def.defName == "LittleFairy_Pawn" || victim.gender == Gender.Male || LitF_Config.CanOfferFemaleRace)
                    {
                        if ((victim.Downed || victim.IsSlave || victim.IsPrisoner) && pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true) && Thing_BloodNursery.FindNurseryFor(victim, pawn, true) != null)
                        {
                            string text4 = "LitF.UI.CarryToNursery".Translate(localTargetInfo4.Thing.LabelCap, localTargetInfo4.Thing);
                            JobDef jDef = Job_LitF.LitF_CarryToNursery;
                            Action action3 = delegate ()
                            {
                                Thing_BloodNursery building_Nursery = Thing_BloodNursery.FindNurseryFor(victim, pawn, false);
                                if (building_Nursery == null)
                                {
                                    building_Nursery = Thing_BloodNursery.FindNurseryFor(victim, pawn, true);
                                }
                                if (building_Nursery == null)
                                {
                                    Messages.Message("LitF.UI.CannotCarryToNursery".Translate(localTargetInfo4.Thing) + ": " + "LitF.UI.NoNursery".Translate(), victim, MessageTypeDefOf.RejectInput, false);
                                    return;
                                }
                                Job job = new Job(jDef, victim, building_Nursery);
                                job.count = 1;
                                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                            };
                            string label = text4;
                            Action action2 = action3;
                            Pawn revalidateClickTarget = victim;
                            opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action2, MenuOptionPriority.Default, null, revalidateClickTarget, 0f, null, null), pawn, victim, "ReservedBy"));
                        }
                    }
                }
            }
            return true;
        }
    }

    // 死は仲がいいやつ以外誰も悲しまない
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
        typeof(DamageInfo),
        typeof(PawnDiedOrDownedThoughtsKind),
        typeof(List<IndividualThoughtToAdd>),
        typeof(List<ThoughtToAddToAll>),
    })]
    public static class LitF_PreDead
    {
        [HarmonyPrefix]
        public static bool LitF_PreApplyDead(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            if (victim.def.race.FleshType == FleshType_LitF.LitF_Fairy)
            {
                return false;
            }
            return true;
        }

    }

    /// <summary>
    /// 脱走しない
    /// </summary>
    [HarmonyPatch(typeof(PrisonBreakUtility), "InitiatePrisonBreakMtbDays")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
        typeof(StringBuilder),
        typeof(bool)
    })]
    static class FixPrisonBreak
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn, StringBuilder sb, bool ignoreAsleep)
        {
            if (pawn.def.defName == "LittleFairy_Pawn")
            {
                __result = -1f;
                return false;
            }
            if (ModsConfig.IdeologyActive)
            {
                if (pawn.ideo?.Ideo?.memes.Contains(Ideology_LitF.LitF_Fairy) ?? false)
                {
                    __result = -1f;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SlaveRebellionUtility), "InitiateSlaveRebellionMtbDays")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    static class FixSlaveRebellion
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "LittleFairy_Pawn")
            {
                __result = -1f;
                return false;
            }
            if (ModsConfig.IdeologyActive)
            {
                if (pawn.ideo?.Ideo?.memes.Contains(Ideology_LitF.LitF_Fairy) ?? false)
                {
                    __result = -1f;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SlaveRebellionUtility), "InitiateSlaveRebellionMtbDaysHelper")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    static class FixSlaveRebellion_Fix2
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "LittleFairy_Pawn")
            {
                __result = -1f;
                return false;
            }
            if (ModsConfig.IdeologyActive)
            {
                if (pawn.ideo?.Ideo?.memes.Contains(Ideology_LitF.LitF_Fairy) ?? false)
                {
                    __result = -1f;
                    return false;
                }
            }
            return true;
        }
    }

    // リトルフェアリーの派閥追加時、服装規定を無効化する
    [HarmonyPatch(typeof(Precept_Role), "GenerateNewApparelRequirements")]
    [HarmonyPatch(new Type[]
    {
        typeof(FactionDef),
    })]
    internal static class ApparelRequirement_Patch
    {

        [HarmonyPrefix]
        static bool Prefix(ref Precept_Role __instance, ref List<PreceptApparelRequirement> __result, FactionDef generatingFor)
        {
            if (generatingFor != null)
            {
                if (generatingFor.defName == "LitF_BloodyWildLittleFairy")
                {
                    List<PreceptApparelRequirement> ApparelRequirement = new List<PreceptApparelRequirement>();
                    // 役割のdefName取得
                    String role = __instance.def.defName;
                    // 役割によって装備を指定
                    if (role == "IdeoRole_Leader")
                    {
                        PreceptApparelRequirement item = new PreceptApparelRequirement();
                        item.requirement = new ApparelRequirement();
                        item.requirement.bodyPartGroupsMatchAny = new List<BodyPartGroupDef>();
                        item.requirement.requiredDefs = new List<ThingDef>();
                        item.requirement.bodyPartGroupsMatchAny.Add(BodyPartGroupDefOf.Torso);
                        item.requirement.requiredDefs.Add(ThingDef.Named("LitF_BloodyArmor"));
                        ApparelRequirement.Add(item);
                    }
                    if (!ApparelRequirement.NullOrEmpty())
                    {
                        __result = ApparelRequirement;
                    }
                    else
                    {
                        __result = null;
                    }
                    return false;
                }
                if (generatingFor.defName == "LitF_GentleWildLittleFairy")
                {
                    List<PreceptApparelRequirement> ApparelRequirement = new List<PreceptApparelRequirement>();
                    // 役割のdefName取得
                    String role = __instance.def.defName;
                    // 役割によって装備を指定
                    if (!ApparelRequirement.NullOrEmpty())
                    {
                        __result = ApparelRequirement;
                    }
                    else
                    {
                        __result = null;
                    }
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 奴隷時に作業速度ボーナス
    /// <summary>
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(StatPart_Slave))]
    internal static class TransformValue_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StatPart_Slave.TransformValue),
            new Type[] { typeof(StatRequest), typeof(float) },
            new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
        static bool Prefix(StatRequest req, ref float val)
        {
            if (req.HasThing)
            {
                if (req.Thing as Pawn != null)
                {
                    Pawn pawn = (Pawn)req.Thing;
                    if (pawn.def.defName == "LittleFairy_Pawn")
                    {
                        Need_Suppression need_Suppression = pawn.needs.TryGetNeed<Need_Suppression>();
                        if (need_Suppression != null)
                        {
                            int multi = (int)Math.Min(5, need_Suppression.CurLevel * 100 / 10);
                            val *= (float)1.0f + multi * 0.1f;
                            return false;
                        }
                    }
                    if (pawn.ideo?.Ideo?.memes.Contains(Ideology_LitF.LitF_Fairy) ?? false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 奴隷時のボーナス説明
    /// <summary>
    [HarmonyPatch(typeof(StatPart_Slave), "ExplanationPart")]
    [HarmonyPatch(new Type[]
    {
        typeof(StatRequest),
    })]
    internal static class ExplanationPart_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ref StatPart_Slave __instance, ref String __result, StatRequest req)
        {
            if (req.HasThing)
            {
                if (req.Thing as Pawn != null)
                {

                    Pawn pawn = (Pawn)req.Thing;
                    if (pawn.def.defName == "LittleFairy_Pawn")
                    {
                        Need_Suppression need_Suppression = pawn.needs.TryGetNeed<Need_Suppression>();
                        if (need_Suppression != null)
                        {
                            int multi = (int)Math.Min(5, need_Suppression.CurLevel * 100 / 10);
                            float parc = (float)1.0f + multi * 0.1f;
                            __result = "LitF.UI.Excitement".Translate() + (": x" + parc.ToStringPercent());
                            return false;
                        }
                    }
                    if (pawn.ideo?.Ideo?.memes.Contains(Ideology_LitF.LitF_Fairy) ?? false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    // 奴隷による仕事の制限を解除
    [HarmonyPatch(typeof(Pawn), "GetDisabledWorkTypes")]
    [HarmonyPatch(new Type[]
    {
        typeof(bool),
    })]
    internal class LitF_FixGetDisabledWorkTypes
    {
        [HarmonyPrefix]
        private static bool Prefix(ref List<WorkTypeDef> __result, ref Pawn __instance, bool permanentOnly)
        {
            if (ModsConfig.IdeologyActive)
            {
                if (__instance.IsSlave)
                {
                    if (__instance.ideo?.Ideo?.memes?.Contains(Ideology_LitF.LitF_Fairy) ?? false)
                    {
                        __result = new List<WorkTypeDef>();
                        __result.Add(WorkTypeDefOf.Warden);
                        return false;
                    }
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 年齢補正無効化
    /// <summary>
    [HarmonyPatch(typeof(StatPart_Age), "AgeMultiplier")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    internal static class AgeMultiplier_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ref StatPart_Age __instance, ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "LittleFairy_Pawn")
            {
                __result = 1.0f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StatPart_AgeOffset), "AgeOffset")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    internal static class AgeMultiplier_Patch_Offset
    {
        [HarmonyPrefix]
        static bool Prefix(ref StatPart_AgeOffset __instance, ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "LittleFairy_Pawn")
            {
                __result = 0f;
                return false;
            }
            return true;
        }
    }

}
