using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 调试。但不负责UI的显示
/// </summary>
public class GameDebug
{
    /// <summary>
    ///来储存所有的控制台信息
    /// </summary>
    public static List<string> ConsoleContents = new List<string>();

    /// <summary>
    /// 日志等级
    /// </summary>
    public enum Level
    {
        None,
        Information,
        Warning,
        Error,
    }

    /// <summary>
    /// 能输出到控制台的最低等级
    /// </summary>
    public static Level ConsoleLevel = Level.Information;
    /// <summary>
    /// 保存到日志文件的最低等级
    /// </summary>
    public static Level SaveLevel = Level.Error;

    /// <summary>
    /// 初始化调试台
    /// </summary>
    public static void Initialization()
    {
        //清除储存的信息
        ConsoleContents = new List<string>();

#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
    }


    public static void Log(string content,Level level)
    {
#if UNITY_EDITOR
        Debug.Log(content);
#endif


        //日志等级足够，能够输出到控制台
        if((int)level >= (int)ConsoleLevel)
        {
            //添加一个识别头。
            content = string.Format("<{0}> {1}：{2}",level.ToString() ,DateTime.Now, content);
           //加入到控制台日志中
            ConsoleContents.Add(content);

            //等级够储存日志文件
            if ((int)level >= (int)SaveLevel)
            {
                //先放在这，以后再写
            }

        }
    }
}
