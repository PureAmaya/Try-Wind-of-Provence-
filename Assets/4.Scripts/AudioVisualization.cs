using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualization : MonoBehaviour
{
    public AudioSource audioSource;

    public Transform circule;
 

    /// <summary>
    /// 园的初始大小
    /// </summary>
    private float[] music = new float[64];

    /// <summary>
    /// 允许⚪变大吗
    /// </summary>
    private bool allowCirculeChange = false;

    /// <summary>
    /// 圆圈的最小大小
    /// </summary>
    private Vector2 minScale = new Vector2(2.5f, 2.5f);

    /// <summary>
    /// 圆圈的最大大小
    /// </summary>
    private Vector2 maxScale = new Vector2(3.5f, 3.5f);

   
    /// <summary>
    /// 上一个采集的music[0]
    /// </summary>
    private float previousMusic;
/// <summary>
/// 短笛采样延迟阈值（面板上设置）
/// </summary>
   [Range(0,30)]
    public int lagForPicclo = 0;
/// <summary>
/// 运行时的短笛采样延迟
/// </summary>
private int lagForPiccoloRuntime;
/// <summary>
/// 短笛圆圈能够膨胀
/// </summary>
private bool expandForPiccolo = false;
    private void Start()
    {
       
      
    }

   

    private void Update()
    {
        //通用的放大
        if (circule.localScale.x <= 2.52f)
        {
            circule.localScale = Vector2.Lerp(circule.localScale, maxScale,Time.deltaTime * 20f) ;
            //取消各种乐器的缩小
            expandForPiccolo = false;
            return;
        }
//短笛膨胀
       if(expandForPiccolo)
        { 
          
            circule.localScale = Vector2.Lerp(circule.localScale,minScale,Time.deltaTime * 10f) ;
        }
      
    
      
      
    }

    /// <summary>
    /// 目前的设定是60fps。
    /// </summary>
    void FixedUpdate()
    {
        // 获取原始采样数据
        audioSource.GetSpectrumData(music, 0, FFTWindow.Hamming);
        
        Piccolo();
        
       
      

     //   float abs = Mathf.Abs(music[0] - previousMusic);

      
        
      //  previousMusic = music[0];


     // 进行标准化处理
     // circuleScale = NormalizeData(sampleData)[5];

     //  Debug.LogFormat("0:{0} 2:{1} 4:{2} 6:{3} 8:{4} 10:{5} 12:{6} 14:{7} 16:{8}", normalizedData[0],
     //    normalizedData[1], normalizedData[2], normalizedData[3], normalizedData[4],normalizedData[5],normalizedData[6],normalizedData[7],normalizedData[8]);

     // circule.localScale = 10 * music[0] * Vector2.one ;

    }


    /// <summary>
    /// 短笛部分
    /// </summary>
    private void Piccolo()
    {
        Debug.Log(music[6]);
        
        //还没有到规定的延迟次数
        if (lagForPiccoloRuntime != lagForPicclo)
        {
            //取负数，说明舍弃
            music[0] = -1f;
            lagForPiccoloRuntime++;
            return;
        }
        //延迟lagForPicclo次之后，赋值，让圆圈开始动
        else
        {
            lagForPiccoloRuntime = 0;
            if (music[0] >= 0.006f)  expandForPiccolo = true;

        }
    }
}
