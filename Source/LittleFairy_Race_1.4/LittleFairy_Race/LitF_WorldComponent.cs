﻿using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LittleFairy_Race
{
	public class LitF_WorldComponent : WorldComponent
	{

		public LitF_WorldComponent(World world)
			: base(world)
		{
		}

		public override void WorldComponentTick()
		{
			int ticks = Find.TickManager.TicksAbs;
			if (ticks % 600 == 0)
            {
				// 現在のバージョン
				if (LitF_Config.Updatever < LitF_Config.ver)
                {
					LitF_Config.Updatever = LitF_Config.ver;
					Dialog_Update dialog = new Dialog_Update();
					Find.WindowStack.Add(dialog);
					LoadedModManager.GetMod<LittleFairy_Settings>().WriteSettings();
				}
            }
		}

		public override void ExposeData()
		{
			base.ExposeData();
		}
	}

	public class Dialog_Update : Window
	{
		public override Vector2 InitialSize => new Vector2(700f, 700f);

		public Dialog_Update()
		{
			forcePause = true;
			doCloseX = true;
			absorbInputAroundWindow = true;
			closeOnAccept = false;
			closeOnClickedOutside = false;
		}

		private static Vector2 scrollPosition;

		public override void DoWindowContents(Rect inRect)
		{
			Texture2D texture_update = ContentFinder<Texture2D>.Get("LittleFairy/UI/Update");
			Texture2D texture_nasuko = ContentFinder<Texture2D>.Get("LittleFairy/UI/Nasuko");
			
			Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + 3000f);
			Rect outRect  = new Rect(0f, 30f, inRect.width, inRect.height - 80f);
			// 通常設定
			Listing_Standard listingStandard = new Listing_Standard();
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			listingStandard.maxOneColumn = true;
			listingStandard.ColumnWidth = viewRect.width / 1.1f;
			listingStandard.Begin(viewRect);
			listingStandard.Gap(10f);
			listingStandard.Label("LitF.Update.title".Translate());
			listingStandard.GapLine();
			listingStandard.Gap(100f);
			Widgets.DrawTextureFitted(new Rect(0, listingStandard.CurHeight, viewRect.width, 200f), texture_update, 1.5f);
			listingStandard.Gap(250f);
			listingStandard.Label("LitF.Update.label".Translate());
			listingStandard.End();
			Widgets.DrawTextureFitted(new Rect(viewRect.width / 8 * 7, listingStandard.CurHeight, viewRect.width / 8, 50), texture_nasuko, 1.0f);
			Widgets.EndScrollView();
		}

	}

}
