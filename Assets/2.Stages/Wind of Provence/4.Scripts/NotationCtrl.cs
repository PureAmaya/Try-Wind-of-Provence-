using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class NotationCtrl : MonoBehaviour
{
    
#if UNITY_EDITOR
    #region EDBUG用。用完了记得删了
    public Transform go;
    public TMP_Text FrameCount;
    #endregion
   #endif
    
    /// <summary>
    /// 储存两位路人王时间的结构体
    /// </summary>
    [System.Serializable]
    public struct Timeline
    {
        public string[] Time;
    }

    public VideoPlayer VideoPlayer;
    public Camera CameraUI;

    //友好的显示音符出现的时间
    public Timeline Junna;
    public Timeline Masako;

    /// <summary>
    /// 铺面滞后数。越大铺面开始的时间越晚。负数则提前出现
    /// </summary>
    public int Lag = 0;
  
    /// <summary>
    /// 要用哪一个时间点了 Junna Masaka
    /// </summary>
    public int[] TimePointToUse = {0,0};

    /// <summary>
    /// Junna的音符显示的位置
    /// </summary>
    public RectTransform[] JunnaNoteLocation;
    public RectTransform[] MasakoNoteLocation;
    

    
//音符预设
    public Transform quarterNoteWithLine;
    public Transform eighthNoteWithLine;
    public Transform eighthNoteWithPoint;
    public Transform eighthNote;

    
    /// <summary>
    /// 音符的初始位置（视野之外）
    /// </summary>
    Vector3 NotePoint = new Vector3(-6f, -100f,0f);
    
    /// <summary>
    /// (Junna)生成的音符池
    /// </summary>
    private List<Transform> notesPoolForJunna = new List<Transform>();
    /// <summary>
    /// (Masako)生成的音符池
    /// </summary>
    private List<Transform> notesPoolForMasako = new List<Transform>();
    /// <summary>
    /// Junna用的间隔记录（即第几个音符是每一组的结尾）
    /// </summary>
    private List<int> JunnaInterval = new List<int>();
    /// <summary>
    /// Masako用的间隔记录（即第几个音符是每一组的结尾）
    /// </summary>
    private List<int> MasakoInterval = new List<int>();
    /// <summary>
    /// 该用Junna的哪一组间隔符号了
    /// </summary>
    private int whichIntervalToUseForJunna = 0;
    /// <summary>
    /// 该用Masako的哪一组间隔符号了
    /// </summary>
    private int whichIntervalToUseForMasako = 0;
/// <summary>
/// masako间隔，是否允许某一组超时1s后自动消失。如果不允许，则只能被下一组顶掉，自己不会消失
/// </summary>
    private bool[] MasakoCouldHideAutomatically;
    
    //将友好型Timeline，转化为的以0.1s为单位的协程可用的时间
    private int[] JunnaReadable;
    private int[] MasakoReadable;
    
#if UNITY_EDITOR
    public TextAsset yamlJunna;
    public TextAsset yamlMasako;

    [ContextMenu("读取yaml")]
    public void ReadYamlAndApply()
    {
      Junna = YamlAndFormat.YamlRead<Timeline>(yamlJunna);
      Masako = YamlAndFormat.YamlRead<Timeline>(yamlMasako);
    }
#endif

    private void Awake()
    {
        //先停止播放
        VideoPlayer.Stop();
        //然后游戏初始化
        Initialization();
        //准备就绪，开始播放
        VideoPlayer.Play();
    }


    
    void Update()
    {
        
//提前显示下一个（如果在时间区间较短）
        ShowAheadForJunna();
        ShowAheadForMasako();
        
        //节奏判断，并更新下一个要判定的音符
        Rhythm();

#if  UNITY_EDITOR
        //debug用，记录时间与帧数
        FrameCount.text = string.Format("{0}\n{1}",VideoPlayer.frame.ToString(), ((int)VideoPlayer.time).ToString()) ;
#endif

    }

    /// <summary>
    /// 将有好的时间线转化为电脑可以用的（帧数）
    /// </summary>
    private void ConvertFriendlyToReadable()
    {
        JunnaReadable = new int[Junna.Time.Length];
        MasakoReadable = new int[Masako.Time.Length];
        
        
        //将友好型时间转化为可读的以 1帧 单位的可读时间
        for (int i = 0; i < Junna.Time.Length; i++)
        {
            //按照冒号分开。长度为4(有的是5）. 0 =1舍弃 =2有改动 .1是分钟（1min=60s=3600)  2是秒（1s=60） 3则可以视为帧数
            string[] fix = Junna.Time[i].Split(':');

            JunnaReadable[i] = int.Parse(fix[3]);
            JunnaReadable[i] += int.Parse(fix[2]) * 60;
            JunnaReadable[i] += int.Parse(fix[1]) * 3600;

            //对于整体的滞后性进行修复
            JunnaReadable[i] += Lag * 10;

            //按照预制的分隔符（fix 4 = D）记录音符间隔分组符号
            if (fix.Length == 5)
            {
                JunnaInterval.Add(i);
            }
         
            
        }

        //将友好型时间转化为可读的以 1帧 单位的可读时间
        for (int i = 0; i < Masako.Time.Length; i++)
        {
            //按照冒号分开。长度为4(有的是5）. 0 =1舍弃 =2有改动 .1是分钟（1min=60s=3600)  2是秒（1s=60） 3则可以视为帧数
            string[] fix = Masako.Time[i].Split(':');

            MasakoReadable[i] = int.Parse(fix[3]);
            MasakoReadable[i] += int.Parse(fix[2]) * 60;
            MasakoReadable[i] += int.Parse(fix[1]) * 3600;

            //对于整体的滞后性进行修复
            MasakoReadable[i] += Lag * 10;//6用来修复我个人手动录入音符时的延迟误差
            
            //按照预制的分隔符（fix 4 = D）记录音符间隔分组符号
            if (fix.Length == 5)
            {
               MasakoInterval.Add(i);
            }
        }
    }

    /// <summary>
    /// 提前显示下一个（如果在时间区间较短）
    /// </summary>
    private void ShowAheadForJunna()
    {
        //Junna 除了第一个是四分音符（有线），第四小节八分音符（有点） ，最后一个八分音符（有点）以外，其余的都是八分音符（有线）
        //所以，notesPoolForJunna中，第一个是四分音符（有线），第二个是八分音符（有点），剩下八个都是八分音符（有线） 
       
      //按照预定好的分组，把池子里的音符拿出来
       //第一组单独的
       if (whichIntervalToUseForJunna == 0)
       {
           //开头的四分音符（有线）
           notesPoolForJunna[0].position = GetUIToWordPos(JunnaNoteLocation[0]);
           //中间的正常八分音符（有线） 
           notesPoolForJunna[2].position = GetUIToWordPos(JunnaNoteLocation[1]);
           notesPoolForJunna[3].position = GetUIToWordPos(JunnaNoteLocation[2]);
           //第四小节的八分音符（有点）
           notesPoolForJunna[1].position = GetUIToWordPos(JunnaNoteLocation[3]);

           whichIntervalToUseForJunna++;
           
           return;
       }
       
       //最后一组单独的
       if (VideoPlayer.frame >= JunnaReadable[JunnaInterval[^2] + 1] - 60)
       {
           
           //算出来要几个八分音符
           int number = JunnaInterval[whichIntervalToUseForJunna] - JunnaInterval[whichIntervalToUseForJunna - 1] - 1;
         
           //按照数目要求，把八分音符放上去
           for (int i = 0; i < number; i++)
           {
               notesPoolForJunna[2 + i].position = GetUIToWordPos(JunnaNoteLocation[i]);
           }
           
           //第四小节的八分音符（有点）
           notesPoolForJunna[1].position = GetUIToWordPos(JunnaNoteLocation[number]);
           
           return;
       }
       
       //用于重置上组的音符，以备下一组
       //本组超时1s后重置
       if (VideoPlayer.frame >= JunnaReadable[JunnaInterval[whichIntervalToUseForJunna - 1]] + 60)
       {
           foreach (var VARIABLE in notesPoolForJunna)
           {
               VARIABLE.position = NotePoint;
           }
           return;
           
       }
      
      
       /*我在这讲两句
        * JunnaInterval[whichIntervalToUseForJunna - 1] + 1 ： 定位到本组的第一个，即上一组的结尾然后往后推算了一个
        * -60：比本组第一个提前60帧（视频帧率）出来
        */
       if(VideoPlayer.frame >= JunnaReadable[JunnaInterval[whichIntervalToUseForJunna - 1] + 1] - 60)
       {
          
           //这里的话就全都是八分音符（有线） 
           //算出来要几个八分音符
           int number = JunnaInterval[whichIntervalToUseForJunna] - JunnaInterval[whichIntervalToUseForJunna - 1];
//按照数目要求，把八分音符放上去
           for (int i = 0; i < number; i++)
           {
               notesPoolForJunna[2 + i].position = GetUIToWordPos(JunnaNoteLocation[i]);
           }
           
           
           //准备进行下一个了
           //最后一个，不再增加数值，防止数组溢出
           if (whichIntervalToUseForJunna != JunnaInterval.Count - 1)whichIntervalToUseForJunna++;
           
       }

       
      
       
    }


    private void ShowAheadForMasako()
    {
        
       

      

        //第一组单独的（嗯摆烂了）
        if (whichIntervalToUseForMasako == 0)
        {
            //这里的话就全都是八分音符
            //算出来要几个八分音符
            int number = MasakoInterval[0];
            
            
            //按照数目要求，把八分音符放上去
            for (int i = 0; i < number; i++)
            {
                notesPoolForMasako[i].position = GetUIToWordPos(MasakoNoteLocation[i]);
            }
           
           
            //准备进行下一个了
            whichIntervalToUseForMasako++;
            return;
        }

        //用于重置本组的音符，以备下一组
        //本组超时1s后重置（要求允许自己消失）
        if (VideoPlayer.frame >= MasakoReadable[MasakoInterval[whichIntervalToUseForMasako - 1]] + 60 && MasakoCouldHideAutomatically[whichIntervalToUseForMasako])
        {
            foreach (var VARIABLE in notesPoolForMasako)
            {
                VARIABLE.position = NotePoint;
            }

            return;
        }
        
        /*我在这讲两句
       * MasakoInterval[whichIntervalToUseForMasako - 1] + 1 ： 定位到本组的第一个，即上一组的结尾然后往后推算了一个
       * -60：比本组第一个提前60帧（视频帧率）出来
       */
        if(VideoPlayer.frame >= MasakoReadable[MasakoInterval[whichIntervalToUseForMasako - 1] + 1] - 60)
        {
          
            //这里的话就全都是八分音符
            //算出来要几个八分音符
            int number = MasakoInterval[whichIntervalToUseForMasako] - MasakoInterval[whichIntervalToUseForMasako - 1];

            //上一组允许自己消失吗，如果不允许，那么就在这里顶掉
            if (!MasakoCouldHideAutomatically[whichIntervalToUseForMasako - 1])
            {
                foreach (var VARIABLE in notesPoolForMasako)
                {
                    VARIABLE.position = NotePoint;
                }
            }
            
            //按照数目要求，把八分音符放上去
            for (int i = 0; i < number; i++)
            {
                notesPoolForMasako[i].position = GetUIToWordPos(MasakoNoteLocation[i]);
            }
           
           
            //准备进行下一个了
            //最后一个，不再增加数值，防止数组溢出
            if (whichIntervalToUseForMasako != MasakoInterval.Count - 1)whichIntervalToUseForMasako++;
           
        }
        
        
       
    }
    /// <summary>
    /// 判定节奏
    /// </summary>
    private void Rhythm()
    {
        //帧数匹配，节奏对上，那个判定的圆形移动到判定线，且中心与判定线重合
        if ((int)VideoPlayer.frame == MasakoReadable[TimePointToUse[1]])
        {
#if UNITY_EDITOR
            go.Rotate(Vector3.forward, 30f); //到时候的判定 debug务必要保证游戏帧数大于60
#endif

            //准备读取下一个时间点
            if (TimePointToUse[1] != MasakoReadable.Length - 1) TimePointToUse[1]++;
        }


        //帧数匹配，节奏对上，那个判定的圆形移动到判定线，且中心与判定线重合
        if ((int)VideoPlayer.frame == JunnaReadable[TimePointToUse[0]])
        {
#if UNITY_EDITOR
            //   go.Rotate(Vector3.forward, 30f); //到时候的判定 debug务必要保证游戏帧数大于60
#endif
            
            //准备读取下一个时间点
            if(TimePointToUse[0] != JunnaReadable.Length - 1)  TimePointToUse[0]++;
        }

       
    }

    /// <summary>
    /// 游戏初始化
    /// </summary>
    private void Initialization()
    {
        //把友好的时间转化为电脑可读的（视频帧数 视频是60fps的录屏emmmmmmm）
        //并且进行每一组的区分
        ConvertFriendlyToReadable();
        
      
        //初始化Junna大镲的音符
        //Junna 除了第一个是四分音符（有线），第四小节八分音符（有点） ，最后一个八分音符（有点）以外，其余的都是八分音符（有线）
        //所以，notesPoolForJunna中，第一个是四分音符（有线），第二个是八分音符（有点），剩下八个都是八分音符（有线） 
        notesPoolForJunna.Add(Instantiate(quarterNoteWithLine,NotePoint,Quaternion.identity));
        notesPoolForJunna.Add(Instantiate(eighthNoteWithPoint,NotePoint,Quaternion.identity));
        for (int i = 0; i < 8; i++)
        {
            notesPoolForJunna.Add(Instantiate(eighthNoteWithLine,NotePoint,Quaternion.identity));
            notesPoolForMasako.Add(Instantiate(eighthNote,NotePoint,Quaternion.identity));
        }
        //Masaka就直接八个八分音符了。（五个循环8个）


        //根据每个分组之间的时间间隔，确定是否允许自行消失（仅Masako需要，因为有时候鼓点比较密）
        MasakoCouldHideAutomatically = new bool[MasakoInterval.Count];
        for (int i = 0; i < MasakoInterval.Count - 1; i++)
        {
            //后一组的第一个和本组的最后一个之间的时间差长于2s则允许自动消失（每组音符提前1s出现，延后1s消失）
            MasakoCouldHideAutomatically[i] =
                MasakoReadable[MasakoInterval[i] + 1] - MasakoReadable[MasakoInterval[i]] <= 120;
        }

//最后一组一定允许自动消失
        MasakoCouldHideAutomatically[MasakoInterval.Count - 1] = true;
    }
    
    /// <summary>
    /// UI坐标转世界坐标（2d)
    /// </summary>
    /// <param name="uiObject"></param>
    /// <returns></returns>
    private Vector2 GetUIToWordPos(RectTransform uiObject)
    {
        Vector2 ptScreen = RectTransformUtility.WorldToScreenPoint(CameraUI, uiObject.position);
        return CameraUI.ScreenToWorldPoint(ptScreen);
        
    }

}
