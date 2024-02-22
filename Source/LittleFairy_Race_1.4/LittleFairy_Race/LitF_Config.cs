using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LittleFairy_Race
{
	public class LittleFairy_Settings : Mod
	{
		public LittleFairy_Settings(ModContentPack content) : base(content)
		{
			GetSettings<LitF_Config>();
		}

		public override string SettingsCategory()
		{
			return "LitF.Config.Title".Translate();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			LitF_Config.DoWindowContents(inRect);
		}
	}

	public class LitF_Config : ModSettings
	{
		// バージョン
		public static int ver = 321;

		// アップデートバージョン
		public static int Updatever = 0;

		// - 起動時でも反映 -
		// 女性を捧げられるようにする
		public static bool CanOfferFemaleRace = false;
		// 1日ごとに得られる経験量
		public static int GetDailyXPAmount = 50;
		// 野生のリトルフェアリーがランダムで継承スキルを所持するか
		public static bool RandomSuccession = false;
		// 継承したリトルフェアリーの出現率
		public static int RandomSuccession_Rate = 10;
		// 敵対するリトルフェアリーのダメージ補正
		public static float EnemyFairy_Damage = 1.0f;
		// ミーフィーが産まれる確率
		public static int SpawnMeefaeRate = 5;
		// 大人の状態で産まれるようにする
		public static bool AdultCondition = true;
		

		// - 再起動後に反映 -
		// 動物にも継承スキルを反映
		public static bool Add_Animal_Succession = false;
		// 継承スキルの確率
		public static int Succession_Rate = 50;
		// 継承スキルの強さ
		public static float Succession_Power = 1.0f;

		public override void ExposeData()
		{
			Scribe_Values.Look(ref Updatever, "Updatever", 0);
			Scribe_Values.Look(ref CanOfferFemaleRace, "CanOfferFemaleRace", false);
			Scribe_Values.Look(ref GetDailyXPAmount, "GetDailyXPAmount", 50);
			Scribe_Values.Look(ref RandomSuccession, "RandomSuccession", false);
			Scribe_Values.Look(ref EnemyFairy_Damage, "EnemyFairy_Damage", 1.0f);
			Scribe_Values.Look(ref RandomSuccession_Rate, "Succession_Amount", 10);
			Scribe_Values.Look(ref SpawnMeefaeRate, "SpawnMeefaeRate", 5);
			Scribe_Values.Look(ref AdultCondition, "AdultCondition", true);
			Scribe_Values.Look(ref Add_Animal_Succession, "Add_Animal_Succession", false);
			Scribe_Values.Look(ref Succession_Rate, "Succession_Rate", 50);
			Scribe_Values.Look(ref Succession_Power, "Succession_Power", 1.0f);
		}

		private static Vector2 scrollPosition;

		public static void DoWindowContents(Rect inRect)
		{
			Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + 700f);
			Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 80f);

			// 通常設定
			Listing_Standard listingStandard = new Listing_Standard();
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			listingStandard.maxOneColumn = true;
			listingStandard.ColumnWidth = viewRect.width / 1.1f;
			listingStandard.Begin(viewRect);
			listingStandard.Gap(50f);
			listingStandard.Label("LitF.Config.Title.0".Translate());
			listingStandard.GapLine();
			listingStandard.Gap(10f);
			listingStandard.CheckboxLabeled("LitF.Config.LabelCanOfferFemaleRace".Translate(), ref CanOfferFemaleRace, "LitF.Config.CanOfferFemaleRace".Translate());
			listingStandard.Gap(10f);
			listingStandard.Label("LitF.Config.LabelGetDailyXPAmount".Translate() + ": " + GetDailyXPAmount, -1.0f, "LitF.Config.GetDailyXPAmount".Translate());
			listingStandard.Gap(5f);
			GetDailyXPAmount = (int)listingStandard.Slider(GetDailyXPAmount, 0, 200);
			listingStandard.Gap(10f);
			listingStandard.Label("LitF.Config.LabelSpawnMeefaeRate".Translate() + ": " + SpawnMeefaeRate + "%", -1.0f, "LitF.Config.SpawnMeefaeRate".Translate());
			listingStandard.Gap(5f);
			SpawnMeefaeRate = (int)listingStandard.Slider(SpawnMeefaeRate, 0, 100);
			listingStandard.Gap(10f);
			listingStandard.CheckboxLabeled("LitF.Config.LabelAdultCondition".Translate(), ref AdultCondition, "LitF.Config.AdultCondition".Translate());
			listingStandard.Gap(10f);
			listingStandard.CheckboxLabeled("LitF.Config.LabelRandomSuccession".Translate(), ref RandomSuccession, "LitF.Config.RandomSuccession".Translate());
			if (RandomSuccession)
            {
				listingStandard.Gap(10f);
				listingStandard.Label("LitF.Config.LabelRandomSuccession_Rate".Translate() + ": " + RandomSuccession_Rate + "%", -1.0f, "LitF.Config.RandomSuccession_Rate".Translate());
				listingStandard.Gap(5f);
				RandomSuccession_Rate = (int)listingStandard.Slider(RandomSuccession_Rate, 0, 100);
			}
			listingStandard.Gap(10f);
			listingStandard.Label("LitF.Config.LabelEnemyFairy_Damage".Translate() + ": x" + EnemyFairy_Damage, -1.0f, "LitF.Config.EnemyFairy_Damage".Translate());
			listingStandard.Gap(5f);
			EnemyFairy_Damage = (float)Math.Round(listingStandard.Slider(EnemyFairy_Damage, 0.1f, 5.0f), 1);
			listingStandard.GapLine();
			listingStandard.Gap(50f);
			listingStandard.Label("LitF.Config.Title.1".Translate());
			listingStandard.GapLine();
			listingStandard.CheckboxLabeled("LitF.Config.LabelAdd_Animal_Succession".Translate(), ref Add_Animal_Succession, "LitF.Config.Add_Animal_Succession".Translate());
			listingStandard.Gap(10f);
			listingStandard.Label("LitF.Config.LabelSuccession_Rate".Translate() + ": " + Succession_Rate + "%", -1.0f, "LitF.Config.Succession_Rate".Translate());
			listingStandard.Gap(5f);
			Succession_Rate = (int)listingStandard.Slider(Succession_Rate, 0, 100);
			listingStandard.Gap(10f);
			listingStandard.Label("LitF.Config.LabelSuccession_Power".Translate() + ": x" + Succession_Power, -1.0f, "LitF.Config.Succession_Power".Translate());
			listingStandard.Gap(5f);
			Succession_Power = (float)Math.Round(listingStandard.Slider(Succession_Power, 0.1f, 3.0f), 1);
			listingStandard.GapLine();
			if (listingStandard.ButtonText("LitF.Config.Update".Translate()))
            {
				Updatever = ver;
				Dialog_Update dialog = new Dialog_Update();
				Find.WindowStack.Add(dialog);
			}
			listingStandard.End();
			Widgets.EndScrollView();
		}

	}
}
