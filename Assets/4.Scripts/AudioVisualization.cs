using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualization : MonoBehaviour
{
    public AudioSource audioSource;

    public Transform circule;
 
    // 注意，采样数组的大小必须为2的次方
    // 另外，采样数据的个数，一定要 >= 条形UI的数量
    private float[] sampleData = new float[16];

    /// <summary>
    /// 延迟那个随着节奏而变化的球的大小变化
    /// </summary>
    private int lag = 0;

  
    /// <summary>
    /// 园的初始大小
    /// </summary>
   
    private float[] music = new float[64];

    /// <summary>
    /// 允许⚪变大吗
    /// </summary>
    private bool allowCirculeChange = false;
    
    /// <summary>
    /// 虚拟圆大小。实际的圆会缓动到这个大小上
    /// </summary>
    private Vector2 virtualScale = Vector2.one;

    /// <summary>
    /// 上一个采集的music[0]
    /// </summary>
    private float previousMusic;
    private void Start()
    {
        circule.localScale = Vector3.one*0.01F;
        //把实际圆的初始大小作为虚拟圆的初始大小
        virtualScale = circule.localScale;
    }

    /// <summary>
    /// 进行标准化数据，将数据映射为 0 到 1 之间的数值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private float[] NormalizeData(float[] input)
    {
        float[] output = new float[input.Length];
        float max = 0;
        float min = 0;
        for (int i = 0; i < input.Length; i++)
        {
            max = Mathf.Max(max, input[i]);
            min = Mathf.Min(min, input[i]);
        }

        float len = max - min;

        for (int i = 0; i < input.Length; i++)
        {
            if (len <= 0)
            {
                output[i] = 0;
            }
            else
            {
                output[i] = (input[i] - min) / len;
            }
        }

        return output;
    }

    private void Update()
    {

      
            circule.localScale = Vector2.Lerp(circule.localScale,virtualScale,0.2F) ;
      
      
    }

    void FixedUpdate()
    {
        // 获取原始采样数据
        audioSource.GetSpectrumData(music, 0, FFTWindow.Hamming);
       // Debug.Log(music[0]);
     //   float abs = Mathf.Abs(music[0] - previousMusic);

        if (music[0] >= 0.08f) return;
        virtualScale = music[0] * 15f * Vector2.one;
      //  previousMusic = music[0];


     // 进行标准化处理
     // circuleScale = NormalizeData(sampleData)[5];

     //  Debug.LogFormat("0:{0} 2:{1} 4:{2} 6:{3} 8:{4} 10:{5} 12:{6} 14:{7} 16:{8}", normalizedData[0],
     //    normalizedData[1], normalizedData[2], normalizedData[3], normalizedData[4],normalizedData[5],normalizedData[6],normalizedData[7],normalizedData[8]);

     // circule.localScale = 10 * music[0] * Vector2.one ;

    }
}
