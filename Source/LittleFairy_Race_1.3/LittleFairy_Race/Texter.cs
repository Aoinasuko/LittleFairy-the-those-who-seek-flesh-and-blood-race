using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LittleFairy_Race
{
	[StaticConstructorOnStartup]
	public static class Graphic_LitF
	{
		public static readonly Texture2D texture_back_1 = ContentFinder<Texture2D>.Get("LittleFairy/UI/ITab/Back_Category_1");
		public static readonly Texture2D texture_back_2 = ContentFinder<Texture2D>.Get("LittleFairy/UI/ITab/Back_Category_2");
		public static readonly Texture2D texture_back_3 = ContentFinder<Texture2D>.Get("LittleFairy/UI/ITab/Back_Category_3");
		public static readonly Texture2D texture_back_4 = ContentFinder<Texture2D>.Get("LittleFairy/UI/ITab/Back_Category_4");
		public static readonly Texture2D texture_tentacle = ContentFinder<Texture2D>.Get("LittleFairy/UI/ITab/Back_Tentacle");
		public static readonly Texture2D texture_lock     = ContentFinder<Texture2D>.Get("LittleFairy/UI/Icon/Skill/Lock");
		public static readonly Texture2D texture_stats    = ContentFinder<Texture2D>.Get("LittleFairy/UI/Icon/Status");

		public static readonly Dictionary<String, Texture2D> texture_skillstyle = new Dictionary<String, Texture2D>();
		public static readonly Dictionary<String, Texture2D> texture_skill = new Dictionary<String, Texture2D>();

		public static readonly Dictionary<int, Material> BloodNursery_BLeft = new Dictionary<int, Material>() {
			{ 0, MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BackLeft/0", reportFailure: true)},
			{ 1, MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BackLeft/1", reportFailure: true)},
			{ 2, MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BackLeft/2", reportFailure: true)},
			{ 3, MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BackLeft/3", reportFailure: true)},
		};

		public static readonly Dictionary<int, Material> BloodNursery_BRight = new Dictionary<int, Material>() {
			{ 0, MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BackRight/0", reportFailure: true)},
			{ 1, MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BackRight/1", reportFailure: true)},
			{ 2, MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BackRight/2", reportFailure: true)},
			{ 3, MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BackRight/3", reportFailure: true)},
		};

		public static readonly Material BloodNursery_BigEgg = MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/BigEgg");

		public static readonly Material BloodNursery_Body = MaterialPool.MatFrom("LittleFairy/Build/Meat/BloodNursery/Flont");

		public static readonly Texture2D ExplosiveEquipment_Icon = ContentFinder<Texture2D>.Get("LittleFairy/UI/Icon/Explosive");

		static Graphic_LitF()
		{
			// スタイルの画像登録
			foreach (SkillStyleDef def in DefDatabase<SkillStyleDef>.AllDefs)
			{
				texture_skillstyle.Add(def.defName, ContentFinder<Texture2D>.Get(def.SkillPicture));
			}
			// スキルの画像登録
			foreach (SkillDef def in DefDatabase<SkillDef>.AllDefs)
			{
				texture_skill.Add(def.defName, ContentFinder<Texture2D>.Get(def.SkillPicture));
			}
		}

	}

}
