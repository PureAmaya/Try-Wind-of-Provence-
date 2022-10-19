using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 负责游戏中文字的动态变化和成果记录
/// </summary>
public class FrameRatioText : MonoBehaviour
{

    private TMP_Text frameCount;
    
   
    private int fps;

    /// <summary>
    /// 多长时间更新一次fps
    /// </summary>
    private const float fpsUpdateTime = 1f;

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
        frameCount = GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        frameCount.text = string.Empty;
    }

    // Update is called once per frame
    public void Update()
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

        frameCount.text = $"fps:{fps.ToString()}";
    }

   

    
   


}