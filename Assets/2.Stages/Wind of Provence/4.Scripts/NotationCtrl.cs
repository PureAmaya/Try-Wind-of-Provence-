using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class NotationCtrl : MonoBehaviour
{
#if UNITY_EDITOR

    #region EDBUG用。用完了记得删了

    public Transform go;
    public TMP_Text frameCount;

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

    public VideoPlayer videoPlayer;
    public Camera cameraUI;

    //友好的显示音符出现的时间
    public Timeline Junna;
    public Timeline Masako;

    /// <summary>
    /// 铺面滞后数。越大铺面开始的时间越晚。负数则提前出现
    /// </summary>
    public int lag = 0;

    /// <summary>
    /// 要用哪一个时间点了 Junna Masaka
    /// </summary>
    public int[] timePointToUse = { 0, 0 };

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
  readonly Vector3 notePoint = new Vector3(-6f, -100f, 0f);

    /// <summary>
    /// (Junna)生成的音符池
    /// </summary>
    private List<Transform> notesPoolForJunna = new List<Transform>();

    /// <summary>
    /// (Masako)生成的音符池
    /// </summary>
    private List<Transform> notesPoolForMasako = new List<Transform>();

    /// <summary>
    /// Junna用的间隔记录（即每一组的结尾是第几个音符）
    /// </summary>
    private List<int> JunnaInterval = new List<int>();

    /// <summary>
    /// Masako用的间隔记录（即每一组的结尾是第几个音符）
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
    /// Masako间隔分组内，每8个为一个亚组，该用那个亚组了
    /// </summary>
    private int subGroupToUse = 0;

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
        videoPlayer.Stop();
        //然后游戏初始化
        Initialization();
        //准备就绪，开始播放
        videoPlayer.Play();
    }


    void Update()
    {
//提前显示下一个（如果在时间区间较短）
        ShowAheadForJunna();
        ShowAheadForMasako();

        //节奏判断，并更新下一个要判定的音符
        Rhythm();

#if UNITY_EDITOR
        //debug用，记录时间与帧数
        frameCount.text = string.Format("{0}\n{1}", videoPlayer.frame.ToString(), ((int)videoPlayer.time).ToString());
#endif
    }

    /// <summary>
    /// 将有好的时间线转化为电脑可以用的（视频帧数）
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
            JunnaReadable[i] += lag * 10;

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
            MasakoReadable[i] += lag * 10; //6用来修复我个人手动录入音符时的延迟误差

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
        if (videoPlayer.frame >= JunnaReadable[JunnaInterval[^2] + 1] - 60)
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


        /*我在这讲两句
         * JunnaInterval[whichIntervalToUseForJunna - 1] + 1 ： 定位到本组的第一个，即上一组的结尾然后往后推算了一个
         * -60：比本组第一个提前60帧（视频帧率）出来
         */
        if (videoPlayer.frame >= JunnaReadable[JunnaInterval[whichIntervalToUseForJunna - 1] + 1] - 60)
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
            if (whichIntervalToUseForJunna != JunnaInterval.Count - 1) whichIntervalToUseForJunna++;
            return;
        }


        //用于重置本组的音符，以备下一组（whichIntervalToUseForJunna这一组的时候加了一，要减去）
        //本组超时1s后重置
        if (videoPlayer.frame >= JunnaReadable[JunnaInterval[whichIntervalToUseForJunna - 1]] + 60)
        {
            foreach (var VARIABLE in notesPoolForJunna)
            {
                VARIABLE.position = notePoint;
            }
        }
    }


    private void ShowAheadForMasako()
    {
        //Masako 除了第一个是四分音符（有线），第四小节八分音符（有点） ，最后一个八分音符（有点）以外，其余的都是八分音符（有线）
        //所以，notesPoolForMasako中，第一个是四分音符（有线），第二个是八分音符（有点），剩下八个都是八分音符（有线） 
        //第一组单独的
        if (whichIntervalToUseForMasako == 0)
        {
            HowMasakoNoteShow(MasakoInterval[0] + 1,true);
            return;
        }
     
       
        
        /*我在这讲两句
         * MasakoInterval[whichIntervalToUseForMasako - 1] + 1 ： 定位到本组的第一个，即上一组的结尾然后往后推算了一个
         * -60：比本组第一个提前60帧（视频帧率）出来
         */
        if (videoPlayer.frame >= MasakoReadable[MasakoInterval[whichIntervalToUseForMasako - 1] + 1] - 60)
        {
            HowMasakoNoteShow(MasakoInterval[whichIntervalToUseForMasako] -
                              MasakoInterval[whichIntervalToUseForMasako - 1]);
            return;
            
        }
    

        //用于重置本组的音符，以备下一组（whichIntervalToUseForMasako这一组的时候加了一，要减去）
        //本组超时1s后重置
        if (videoPlayer.frame >= MasakoReadable[MasakoInterval[whichIntervalToUseForMasako - 1]] + 60)
        {
            foreach (var VARIABLE in notesPoolForMasako)
            {
                VARIABLE.position = notePoint;
            }

            //亚组重置
            subGroupToUse = 0;
        }
    }

    /// <summary>
    /// 判定节奏
    /// </summary>
    private void Rhythm()
    {
        //帧数匹配，节奏对上，那个判定的圆形移动到判定线，且中心与判定线重合
        if ((int)videoPlayer.frame == MasakoReadable[timePointToUse[1]])
        {
#if UNITY_EDITOR
            go.Rotate(Vector3.forward, 30f); //到时候的判定 debug务必要保证游戏帧数大于60
#endif

            //准备读取下一个时间点
            if (timePointToUse[1] != MasakoReadable.Length - 1) timePointToUse[1]++;
        }


        //帧数匹配，节奏对上，那个判定的圆形移动到判定线，且中心与判定线重合
        if ((int)videoPlayer.frame == JunnaReadable[timePointToUse[0]])
        {
#if UNITY_EDITOR
            //   go.Rotate(Vector3.forward, 30f); //到时候的判定 debug务必要保证游戏帧数大于60
#endif

            //准备读取下一个时间点
            if (timePointToUse[0] != JunnaReadable.Length - 1) timePointToUse[0]++;
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
        notesPoolForJunna.Add(Instantiate(quarterNoteWithLine, notePoint, Quaternion.identity));
        notesPoolForJunna.Add(Instantiate(eighthNoteWithPoint, notePoint, Quaternion.identity));
        for (int i = 0; i < 8; i++)
        {
            notesPoolForJunna.Add(Instantiate(eighthNoteWithLine, notePoint, Quaternion.identity));
            notesPoolForMasako.Add(Instantiate(eighthNote, notePoint, Quaternion.identity));
        }
        //Masaka就直接八个八分音符了。（五个循环8个）

        
    }

    /// <summary>
    /// UI坐标转世界坐标（2d)
    /// </summary>
    /// <param name="uiObject"></param>
    /// <returns></returns>
    private Vector2 GetUIToWordPos(RectTransform uiObject)
    {
        Vector2 ptScreen = RectTransformUtility.WorldToScreenPoint(cameraUI, uiObject.position);
        return cameraUI.ScreenToWorldPoint(ptScreen);
    }

    /// <summary>
    /// Masako的音符怎么显示
    /// </summary>
    /// <param name="number">算出来一共要几个八分音符</param>
    /// <param name="isFirst">是第一组吗</param>
    private void HowMasakoNoteShow(int number,bool isFirst = false)
    {
        //这里的话就全都是八分音符
    
        switch (number)
        {
            //数目少于等于8（一共就8个坑），直接放上去
            case <= 8:
                for (int i = 0; i < number; i++)
                {
                    notesPoolForMasako[i].position = GetUIToWordPos(MasakoNoteLocation[i]);
                }

                //准备进行下一个了
                //最后一个，不再增加数值，防止数组溢出
                if (whichIntervalToUseForMasako != MasakoInterval.Count - 1) whichIntervalToUseForMasako++;
              
                break;

            //数目大于8（一共就8个坑），8个8个的往上放
            default:

               
                //计算出这组内有几个亚组(subGroup)
                int subGroup = (int)(number / 8);
                //剩下不够8个，单独一个亚组
                number = number - subGroup * 8;
                
                Debug.Log(string.Format("subGroup:{0}  toUse:{1} number:{2}",subGroup,subGroupToUse,number));
                
                //根据要用第几个亚组，显示对应数量的八分音符
                //第一个亚组
                if (subGroupToUse == 0)
                {
                    //按照预定的，每一个分组内的第一个亚组要提前60fps显示。并持续保留到最后一个亚组
                    for (int i = 0; i < 8; i++)
                    {
                        notesPoolForMasako[i].position = GetUIToWordPos(MasakoNoteLocation[i]);
                    }

                    subGroupToUse++;
                }
                //最后一个亚组
                else if (subGroupToUse == subGroup)
                {
                    //直接让多余的消失
                    for (int i = 0; i < 8 - number; i++)
                    {
                        notesPoolForMasako[i + number].position = notePoint;
                    }
                    
                    //准备进行下一个了
                    //最后一个，不再增加数值，防止数组溢出
                    if (whichIntervalToUseForMasako != MasakoInterval.Count - 1) whichIntervalToUseForMasako++;
                }
                //中间几个，得满足每个亚组提前5fps出来的要求
                else
                {
                    HowMasakoIntermiddleNoteShow(number,isFirst);
                }

                break;
        }
    }

    /// <summary>
    /// Masako每个分组内，位于中间部分的亚组的音符怎么显示
    /// </summary>
    /// <param name="number"></param>
    /// <param name="isFirst">是第一组吗</param>
    private void HowMasakoIntermiddleNoteShow(int number,bool isFirst = false)
    {
        switch (isFirst)
        {
            case false:
                if (videoPlayer.frame >=
                    MasakoReadable[MasakoInterval[whichIntervalToUseForMasako - 1] + 1 + subGroupToUse * 8] - 5)
                {
                    Enter(1);
                    subGroupToUse++;
                }
                break;
            
            case  true:
                if (videoPlayer.frame >=
                    MasakoReadable[subGroupToUse * 8] - 5)
                {
                    Enter(1);
                    subGroupToUse++;
                }
                break;
        }
        
      
    }

/// <summary>
/// 滚动的判定条，让他回车，回到开头
/// </summary>
/// <param name="character">0 Junna  1 Masako</param>
    private void Enter(int character)
    {
        Debug.Log("Enter");
    }
}