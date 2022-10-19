using TMPro;
using UnityEngine;

/// <summary>
/// 负责游戏中文字的动态变化和成果记录
/// </summary>
public class TextUI : MonoBehaviour
{
    public static TextUI textUI;

    /// <summary>
    /// 记录本次成绩用（顺便保存到saves)
    /// </summary>
    public YamlReadWrite.Achievement achievement;

    [Header("UI与屏幕内容")] public AtlasRead atlasRead;

  
    public TMP_Text score;

    public TMP_Text matchName;

    /// <summary>
    /// 正确按下按键的个数
    /// </summary>
    private int right;

    /// <summary>
    /// 总共有多少按键
    /// </summary>
    private int total;

  

    /// <summary>
    /// fps显示之前累积的RealTimeDelta
    /// </summary>
    private float totalRealTimeDeltaBeforeFpsShow;

    /// <summary>
    /// 累积了几个RealTimeDelta
    /// </summary>
    private int countTimeDelta;

    private void Awake()
    {
        textUI = this;
        total = 0;
        right = 0;
        score.text = $"技术评定：1.000";
    }

    // Start is called before the first frame update
  

    /// <summary>
    /// 计算分数与排名
    /// </summary>
    /// <param name="score">分数（影响技术评定的权重）</param>
    public void ScoreAndRank(int score)
    {
        //正确按下，权重为1
        if (score > 0)
        {
            total++;
            right++;
        }
        //没正确按下，权重为2
        else
        {
            total += 3;
        }

        //记录成绩
        achievement.score = ((float)right / total).ToString("F3");
//展示实时成绩
        this.score.text = $"技术评定：{achievement.score}";

        //根据正确率获取排名
        switch ((float)right / total)
        {
            //府赛没有得到名次
            case < 0.3f:
                matchName.text = "京都府 吹奏楽コンクール";
                atlasRead.spriteName = "clear";
                atlasRead.GetSpriteFromAtlas();
                achievement.RankLevel = 0;
                break;

            //府赛金奖（废金）
            case >= 0.3f and <= 0.4f:
                matchName.text = "京都府 吹奏楽コンクール";
                atlasRead.spriteName = "gold";
                atlasRead.GetSpriteFromAtlas();
                achievement.RankLevel = 1;
                break;

            //府赛金奖（晋级）
            case > 0.4f and <= 0.5f:
                matchName.text = "京都府 吹奏楽コンクール";
                atlasRead.spriteName = "representative";
                atlasRead.GetSpriteFromAtlas();
                achievement.RankLevel = 2;
                break;

            //关西废金
            case > 0.5f and <= 0.7f:
                matchName.text = "関西 吹奏楽コンクール";
                atlasRead.spriteName = "gold";
                atlasRead.GetSpriteFromAtlas();
                achievement.RankLevel = 3;
                break;

            //关西金奖（晋级）
            case > 0.7f and <= 0.9f:
                matchName.text = "関西 吹奏楽コンクール";
                atlasRead.spriteName = "representative";
                atlasRead.GetSpriteFromAtlas();
                achievement.RankLevel = 4;
                break;

            //全国赛 铜奖
            case > 0.9f and <= 0.97f:
                matchName.text = "全日本 吹奏楽コンクール";
                atlasRead.spriteName = "bronze";
                atlasRead.GetSpriteFromAtlas();
                achievement.RankLevel = 5;
                break;

            //全国赛 银奖
            case > 0.97f and < 0.99f:
                matchName.text = "全日本 吹奏楽コンクール";
                atlasRead.spriteName = "sliver";
                atlasRead.GetSpriteFromAtlas();
                achievement.RankLevel = 6;
                break;

            ////全国赛 金奖
            case >= 0.99f:
                matchName.text = "全日本 吹奏楽コンクール";
                atlasRead.spriteName = "gold";
                atlasRead.GetSpriteFromAtlas();
                achievement.RankLevel = 7;
                break;
        }
    }
}