using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LittleFairy_Race
{
    // スキル派生のDef
    public class SkillStyleDef : Def
    {
        // FPの最大値
        public int FPMax = 0;
        // この派生が発生するために必要な生物のDefName(どれか1つだけあればOK)
        public List<String> TargetPawn = new List<string>();
        // ランダム生成されるか？
        public bool CanGeneRandom = true;
        // 生成確率(高いほど生成されやすい)
        public int CommonRate = 100;
        // 取得できるスキルのDef(3つ)
        public List<SkillDef> SkillDefs = new List<SkillDef>();
        // 派生の色
        public Color SkillColor = new Color(1f, 1f, 1f);
        // スキルスタイルの画像パス
        public String SkillPicture = "";
    }

    // スキルのDef
    public class SkillDef : Def
    {
        // パッシブ効果の追加説明
        [MustTranslate]
        [DefaultValue("")]
        public string PassiveDescription = "";
        // 取得で得られるFPの最大値
        public int FPMax = 0;
        // 取得で得られるFPの回復量
        public int FPHealRate = 0;
        // 取得で得られるスキルのクールダウン量
        public int CoolDownRate = 0;
        // 汎用特殊能力か？
        public bool IsCommonSkill = false;
        // 汎用特殊能力の時、重複できるか？
        public bool IsDuplication = true;
        // 汎用特殊能力の時、このスキルが発生するために必要な生物のDefName(どれか1つだけあればOK)
        public List<String> TargetPawn = new List<string>();
        // 生成確率(高いほど生成されやすい)
        public int CommonRate = 100;
        // スキルの種別
        // Passive : 常に発動
        // Active：ボタンを押すと発動
        // Toggle：ボタンを押すと発動、ただし切り替え式
        // Both：Passive+Active
        public SkillCategory SkillCategory = SkillCategory.Passive;
        // スキル使用時のターゲット
        // Self     ：自身
        // Pawn     ：ポーン一人
        // Thing    ：ポーンも建築物も
        // AreaPawn ：範囲のポーン
        public SkillTarget Target = SkillTarget.Self;
        // 射程
        public float TargetRange = -1f;
        // 範囲攻撃か？
        public bool IsArea = false;
        // 範囲の大きさ
        public float TargetAreaRange = -1f;
        // 範囲が壁を通過できるか
        // None     : 貫通しない
        // ShootLine: 射線のみ貫通出来る
        // Range    : 範囲が貫通できる
        // Both     : 射線・範囲貫通
        public PenetratingWall PenetratingWall = PenetratingWall.None;
        // 使用するスキルの処理
        public SkillBase SkillCalc = null;
        // スキルを使用するのに必要な待機Tick
        public int SkillStayTime = 0;
        // スキルを使用するのに必要なクールダウンTick
        public int SkillCoolDown = 0;
        // スキルの画像パス
        public String SkillPicture = "";
        // 入手で追加されるHediff
        public HediffDef AddHediff = null;
        // 要求経験値
        public int NeedXP = 100;
        // スキル使用時に貰える経験値
        public int GetXP = 100;
        // 有害なスキルか？（他の派閥に使うと友好度の低下を招くか？）
        public bool Isbad = false;
        // どれぐらい友好度が低下するか？
        public int Downval = 10;
        // 効果発動時に対象に与えるエフェクト
        public ThingDef UseMote = null;
        // 効果発動時に対象に与える音声
        public SoundDef UseSound = null;

        public SkillDef Clone()
        {
            // Object型で返ってくるのでキャストが必要
            return (SkillDef)MemberwiseClone();
        }
    }

    // スキルのベース
    public abstract class SkillBase
    {
        // Tick毎の処理(Passive or Both)
        public virtual void Tick(Pawn pawn)
        {

        }

        // FP回復時間毎の処理 1時間毎(Passive or Both)
        public virtual void FPHealTick(Pawn pawn)
        {

        }

        // スキル使用時の処理(Active or Both)
        public virtual void SkillUse(Pawn pawn, LocalTargetInfo target)
        {

        }

        // スキルが現在使用できるかをチェックする(Active)
        public virtual bool SkillUseCheck(Pawn pawn, out String reason)
        {
            reason = "";
            return true;
        }
    }

    public enum SkillCategory {
        Passive,
        Active,
        Toggle,
        Both,
    }

    public enum SkillTarget
    {
        Self,
        Pawn,
        Build,
        Locate,
    }

    public enum PenetratingWall
    {
        None,
        ShootLine,
        Range,
        Both
    }

}
