using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour, IUpdate
{
    public static TextUI textUI;
    
    /// <summary>
    /// 记录视频帧数与帧率
    /// </summary>
    [Header("UI与屏幕内容")]
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

    /// <summary>
    /// 游戏分数
    /// </summary>
    private int gameScore;
    
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
        gameScore = 0;
        total = 0;
        right = 0;
    }

    // Start is called before the first frame update
    private void Start()
    {
        frameCount.text =string.Empty;
        //注册Update
        UpdateManager.updateManager.Updates.Add(this);
    }

    // Update is called once per frame
    public void FastUpdate()
    {
        
        //时间够了，显示一次fps
        if (totalRealTimeDeltaBeforeFpsShow >= fpsUpdateTime)
        {
            fps = (int)( countTimeDelta / totalRealTimeDeltaBeforeFpsShow);
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
    /// <param name="score"></param>
    public void ScoreAndRank(int score)
    {
        total++;
        if (score > 0) right++;
        gameScore += score;

        this.score.text = $"分数：{gameScore.ToString()}";
        
        switch ((float)right/total)
        {
            case < 0.1f:
                matchName.text = "落选";
                RankIcon = null;
                break;

        }
        
    }
}
