using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GestureCtrl : MonoBehaviour
{
    public Metronome metronome;

    public SpriteRenderer spriteRenderer;

    /// <summary>
    /// 滴答的时间
    /// </summary>
    private float tickTime = -1f;

    private Color normal = new Color(1f, 0.5132f, 0.8657f, 1f);
    private Color miss = new Color(0.8773f, 0.1771f, 0.3712f, 1f);
    private Color pressed = new Color(0.2172f, 0.8784f, 0.1764f, 1f);

    private void Awake()
    {
        //恢复颜色
        spriteRenderer.color = normal;
    }

    // Start is called before the first frame update
    private void Start()
    {
        metronome.AfterTick.AddListener(RecordTick);
    }


    // Update is called once per frame
    void Update()
    {
        if (tickTime < 0f) return;
       c(Input.GetAxisRaw("Vertical"),Input.GetAxisRaw("Horizontal"));
     

    }


  /// <summary>
  /// 对于节拍器每个滴答进行记录
  /// </summary> 
  /// <param name="meter">当前在第几拍 </param>
    public void RecordTick(int meter)
  {
      tickTime = Time.timeSinceLevelLoad;
     //恢复颜色
      spriteRenderer.color = normal;
  }


    /// <summary>
    /// 按照时间判定玩家是否及时按了下去，并赋分
    /// </summary>
    /// <param name="value">传递玩家按下去的值</param>
    void c(float vertical,float horizontal)
    {
        //超时了，不管后面的按键
        if (Time.timeSinceLevelLoad - tickTime >= 0.35f)
        {
            //终止Update
            tickTime = -1f;
            TextUI.textUI.ScoreAndRank(-3000);
           
            spriteRenderer.color = miss;
            return;
        }


        switch (metronome.meter)
        {
            //玩家在规定时间内按下了对应的按键
         case 1 when vertical < 0:
             MeterPressed();
             break;
            case 2 when horizontal < 0:
                MeterPressed();
                break;
            case 3 when horizontal > 0:
                MeterPressed();
                break;
            case 4 when vertical > 0:
                MeterPressed();
                break;
        }
    }

    /// <summary>
    /// 节拍成功按下
    /// </summary>
   private void MeterPressed()
    {
        //终止Update
        tickTime = -1f;
        TextUI.textUI.ScoreAndRank(1500);
        spriteRenderer.color = pressed;
    }
}
