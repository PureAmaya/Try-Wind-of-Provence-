using System;
using System.IO;
using System.Text;
using UnityEngine;
using YamlDotNet.Serialization;

public static class YamlReadWrite 
{
    public enum  FileName
    {
        Settings,
        JunnaTimeline,
        MasakoTimeline,
        /// <summary>
        /// 游戏成果
        /// </summary>
        Achievement,
    }
 
  
  
  /// <summary>
  /// Assets上一级的目录（结尾没有/）
  /// </summary>
  /// <returns></returns>
  private static string UnityButNotAssets
  {
      get
      {
          string[] raw = Application.dataPath.Split("/");

          string done = string.Empty;
          for (int i = 1; i < raw.Length - 1; i++)
          {
              done = $"{done}/{raw[i]}";
          }
          return done;
      }
  }

/// <summary>
/// 在规定的文件夹中写yaml文件
/// </summary>
/// <param name="content">内容</param>
/// <param name="fileName">文件名</param>
/// <param name="notes">注释（按照yaml规范书写）</param>
/// <typeparam name="T"></typeparam>
  public static void Write<T>(T content, FileName fileName,string notes = null)
  {
//检查是否存在所需的游戏文件夹，不存在则创建
      CheckAndCreateDirectory();
      Serializer serializer = new Serializer();

      //把注释写入的内容
      string authenticContent = $"{notes}\n{serializer.Serialize(content)}" ;

      StreamWriter streamWriter =
          new StreamWriter($"{UnityButNotAssets}/saves/{fileName.ToString()}.yaml", false, Encoding.UTF8);
      streamWriter.Write(authenticContent);
      streamWriter.Dispose();
      streamWriter.Close();
  }

/// <summary>
/// 读取yaml
/// </summary>
/// <param name="fileName">文件名</param>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
public static T Read<T>(FileName fileName)
{
    //检查是否存在所需的游戏文件夹，不存在则创建
    CheckAndCreateDirectory();
    Deserializer deserializer = new();
    StreamReader streamReader =
        new StreamReader($"{UnityButNotAssets}/saves/{fileName.ToString()}.yaml", Encoding.UTF8);


    var content = deserializer.Deserialize<T>(streamReader.ReadToEnd());
    streamReader.Dispose();
    streamReader.Close();
    return content;

}

 
    /// <summary>
    /// 检查是否存在所需的游戏文件夹，不存在则创建
    /// </summary>
    public static void CheckAndCreateDirectory()
    {
        if (!Directory.Exists($"{UnityButNotAssets}/saves"))
        {
            Directory.CreateDirectory($"{UnityButNotAssets}/saves");
        }
    }


    #region yaml用的各种结构体（类）
    /// <summary>
    /// 储存两位路人王时间的结构体
    /// </summary>
    [Serializable]
    public struct Timeline
    {
        public string[] Time;
    }

    /// <summary>
    /// 游戏排名（本地多次成绩排名）
    /// </summary>
    [Serializable]
    public struct Achievement
    {
        public int RankLevel;
        public string score;

    }

    /// <summary>
    /// 设置的内容
    /// </summary>
    [Serializable]
    public struct SettingsContent
    {
        public  float MusicVolume;
        public  float SoundEffectVolume; 
        public int lag;
    }
    
    
    

    #endregion
}
