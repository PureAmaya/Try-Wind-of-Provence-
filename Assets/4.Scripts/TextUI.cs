using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour, IUpdate
{
    public static TextUI textUI;

    /// <summary>
    /// 根据成绩得到的排名
    /// </summary>
    public static int Rank;

    [Header("UI与屏幕内容")] public AtlasRead atlasRead;

    /// <summary>
    /// 记录视频帧数与帧率
    /// </summary>
    public TMP_Text frameCount;

    public TMP_Text score;

    public Image RankIcon;

    public TMP_Text matchName;

    /// <summary>
    /// 正确按下按键的个数
    /// </summary>
    private int right;

    /// <summary>
    /// 总共有多少按键
    /// </summary>
    private int total;

    private int fps;

    /// <summary>
    /// 多长时间更新一次fps
    /// </summary>
    private const float fpsUpdateTime = 0.5f;

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
        Rank = 0;
        score.text = $"技术评定：1.000";
    }

    // Start is called before the first frame update
    private void Start()
    {
        frameCount.text = string.Empty;
        //注册Update
        UpdateManager.updateManager.Updates.Add(this);
    }

    // Update is called once per frame
    public void FastUpdate()
    {
        //时间够了，显示一次fps
        if (totalRealTimeDeltaBeforeFpsShow >= fpsUpdateTime)
        {
            fps = (int)(countTimeDelta / totalRealTimeDeltaBeforeFpsShow);
            countTimeDelta = 0;
            totalRealTimeDeltaBeforeFpsShow = 0f;
        }
        else
        {
            countTimeDelta++;
            totalRealTimeDeltaBeforeFpsShow += Time.unscaledDeltaTime;
        }

        frameCount.text = $"fps:{fps.ToString()}\nframe:{StaticVideoPlayer.videoPlayer.frame}";
    }


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
            total -= score;
        }


        this.score.text = $"技术评定：{((float)right / total).ToString("F3")}";

        //根据正确率获取排名
        switch ((float)right / total)
        {
            //府赛没有得到名次
            case < 0.3f:
                matchName.text = "京都府 吹奏楽コンクール\n<b>预测</b>";
                RankIcon = null;
                Rank = 0;
                break;

            //府赛金奖（废金）
            case >= 0.3f and <= 0.4f:
                matchName.text = "京都府 吹奏楽コンクール\n<b>预测</b>";
                atlasRead.spriteName = "gold";
                atlasRead.GetSpriteFromAtlas();
                Rank = 1;
                break;

            //府赛金奖（晋级）
            case > 0.4f and <= 0.5f:
                matchName.text = "京都府 吹奏楽コンクール\n<b>预测</b>";
                atlasRead.spriteName = "representative";
                atlasRead.GetSpriteFromAtlas();
                Rank = 2;
                break;

            //关西废金
            case > 0.5f and <= 0.7f:
                matchName.text = "関西 吹奏楽コンクール\n<b>预测</b>";
                atlasRead.spriteName = "gold";
                atlasRead.GetSpriteFromAtlas();
                Rank = 3;
                break;

            //关西金奖（晋级）
            case > 0.7f and <= 0.9f:
                matchName.text = "関西 吹奏楽コンクール\n<b>预测</b>";
                atlasRead.spriteName = "representative";
                atlasRead.GetSpriteFromAtlas();
                Rank = 4;
                break;

            //全国赛 铜奖
            case > 0.9f and <= 0.96f:
                matchName.text = "全日本吹奏楽コンクール\n<b>预测</b>";
                atlasRead.spriteName = "bronze";
                atlasRead.GetSpriteFromAtlas();
                Rank = 5;
                break;

            //全国赛 银奖
            case > 0.96f and < 0.99f:
                matchName.text = "全日本吹奏楽コンクール\n<b>预测</b>";
                atlasRead.spriteName = "sliver";
                atlasRead.GetSpriteFromAtlas();
                Rank = 6;
                break;

            ////全国赛 金奖
            case >= 0.99f:
                matchName.text = "全日本吹奏楽コンクール\n<b>预测</b>";
                atlasRead.spriteName = "gold";
                atlasRead.GetSpriteFromAtlas();
                Rank = 7;
                break;
        }
    }
}