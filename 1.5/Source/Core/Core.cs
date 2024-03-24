using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LittleFairy_Race
{
    [DefOf]
    public static class Job_LitF
    {
        public static JobDef LitF_UseSkill;
        public static JobDef LitF_EnterNursery;
        public static JobDef LitF_CarryToNursery;
        public static JobDef LitF_UseCollar;
    }

    [DefOf]
    public static class Sound_LitF
    {
        public static SoundDef LitF_Chant;
        public static SoundDef LitF_Parry;
        public static SoundDef LitF_Break;
    }

    /*
    [DefOf]
    public static class Mote_LitF
    {
        public static ThingDef LitF_Mote_Chanting;
        public static ThingDef LitF_Mote_Effect;
        public static ThingDef LitF_Mote_Reflect;
        public static ThingDef LitF_Mote_FPGuard;
    }
    */

    [DefOf]
    public static class Hediff_LitF
    {
        public static HediffDef LitF_Consumption;
        public static HediffDef LitF_Skill_BerserkerCharge;
        public static HediffDef LitF_Skill_Traped;
        public static HediffDef LitF_Counterattack;
        public static HediffDef LitF_WOTF;
    }

    [DefOf]
    public static class FleshType_LitF
    {
        public static FleshTypeDef LitF_Fairy;
    }

    [DefOf]
    public static class Research_LitF
    {
        public static ResearchProjectDef LitF_ResourceRecycling;
    }

    [DefOf]
    public static class Ideology_LitF
    {
        [MayRequireIdeology]
        public static MemeDef LitF_Fairy;
    }

}
