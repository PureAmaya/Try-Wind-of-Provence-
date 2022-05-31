using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;


/// <summary>
/// yaml读写，以及yaml相关的格式化（manifest之类的）
/// </summary>
public class YamlAndFormat
{
    /// <summary>
    /// 预设的子文件夹
    /// </summary>
    public enum SubdirectoryTypes
    {
        SpellCards,
        Skins,
        Settings,
        PlayerProfile,
        Log,
    }

    /// <summary>
    /// 版本控制，顺序与SubdirectoryTypes一致
    /// </summary>
    readonly static int[] VersionControl = { 1, 1, 1, 1, 1 };


    /// <summary>
    /// Assets上一级的目录
    /// </summary>
    /// <returns></returns>
    internal static string UnityButNotAssets
    {
        get
        {
            string[] raw = Application.dataPath.Split("/");

            string done = string.Empty;
            for (int i = 1; i < raw.Length - 1; i++)
            {
                //得到从unity开始的路径
                done = string.Format("{0}/{1}", done, raw[i]);
            }

            DirectoryInfo directoryInfo = new(done);
            return directoryInfo.FullName;
        }
    }


    #region YAML用的结构体
    /// <summary>
    /// 用于展示基本信息的清单。不包含弹幕设计(YAML）
    /// </summary>
    public struct Manifest
    {
        /// <summary>
        /// 应当与所在的文件夹同名。
        /// </summary>
        public string Name;
        /// <summary>
        /// 音乐名称，在游戏中展示。
        /// </summary>
        public string MusicName;
        /// <summary>
        /// 铺面图标（不含拓展名）
        /// </summary>
        public string Icon; 
        /// <summary>
        /// 铺面作者名称。
        /// </summary>
        public string Author;
        /// <summary>
        /// BGM出处/所属专辑。
        /// </summary>
        public string Origin;
        /// <summary>
        /// 预览音乐（不含拓展名）
        /// </summary>
        public string PreviewBGM;
        /// <summary>
        /// 可用的难度。按照Easy Normal Hard Lunatic的顺序。
        /// </summary>
        public bool[] AllowedDifficulty;
        /// <summary>
        /// 是否为高级符卡。
        /// </summary>
        public bool IsAdvanced;
        /// <summary>
        /// 该文件的版本。（用于对自己做的符卡进行版本控制）
        /// </summary>
        public string Version;

        /// <summary>
        ///  用于展示基本信息的清单。不包含弹幕设计(YAML）
        /// </summary>
        /// <param name="name">铺面名称。用于文件夹命名以及定位</param>
        /// <param name="musicName">音乐名称。默认在对应的铺面目录下，ogg格式，不包含拓展名</param>
        /// <param name="icon">音乐缩略图。默认在对应的铺面目录下，png格式，不含拓展名</param>
        /// <param name="author">铺面作者。</param>
        /// <param name="origin">音乐来源。</param>
        /// <param name="previewBGM">预览音乐名称。默认在对应的铺面目录下，ogg格式，不包含拓展名</param>
        /// <param name="isAdvanced">高级铺面？  dll封装请设置为true</param>
        /// <param name="version">铺面版本。铺面作者给定的版本</param>
        /// <param name="difficulties">可用难度。至少设定一个难度,按照Easy Normal Hard Lunatic的顺序.</param>
        public Manifest(bool[] difficulties, string name = "Default SpellCard", string musicName = "默认符卡",
            string icon = "Icon", string author = "匿名", string origin = "未知出处", string previewBGM = "Pre",
            bool isAdvanced = false, string version = "1.0.0")
        {
            if (difficulties.Length != 4)
            {
                GameDebug.Log(String.Format("{0}难度设定有误。", name), GameDebug.Level.Error);
            }

            Name = name;
            MusicName = musicName;
            Icon = icon;
            Author = author;
            Origin = origin;
            PreviewBGM = previewBGM;
            IsAdvanced = isAdvanced;
            Version = version;
            AllowedDifficulty = difficulties;

        }
    }

    #endregion

    /// <summary>
    /// 从SpellCards文件夹种读取所有的清单文件，用来形成铺面列表
    /// </summary>
    /// <returns></returns>
    public static List<Manifest> ManifestList()
    {
        var all = ReadAllSubdirectory(SubdirectoryTypes.SpellCards);

        //储存有效的valid文件
        List<Manifest> valid = new List<Manifest>();

        //有清单文件就存进去
        foreach (var item in all)
        {

            if (File.Exists(string.Format("{0}/Manifest.yaml", item.FullName)))
            {
                valid.Add(YamlRead<Manifest>(SubdirectoryTypes.SpellCards, item.Name, "Manifest"));
            }
        }

        return valid;
    }



    #region 读取存放文件


    /// <summary>
    /// yaml读取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="types">子文件夹类型</param>
    /// <param name="fileName">文件名（不含拓展名）</param>
    /// <param name="directoryName">专用文件夹名字</param>
    /// <returns></returns>
    public static T YamlRead<T>(SubdirectoryTypes types, string directoryName, string fileName)
    {       
        //提前准备文件的路径
        string path = string.Format("{0}/{1}/{2}/{3}.yaml", UnityButNotAssets, types.ToString(), directoryName,fileName);

        if (File.Exists(path))
        {
            //yaml文件的内容
            string content = File.ReadAllText(string.Format("{0}/{1}/{2}/{3}.yaml", UnityButNotAssets, types.ToString(), directoryName, fileName), System.Text.Encoding.UTF8);


            //如果yaml的版本低于读取规定的版本，则输出一个警告
            if(int.Parse(content.Split("#")[1]) < VersionControl[(int)types])
            {
                GameDebug.Log(string.Format("当前文件的版本已过时，游玩时可能发生错误。低版本的类别为“{0}”、路径为{1}",types.ToString(),path), GameDebug.Level.Warning);
            }

            Deserializer read = new();
            return read.Deserialize<T>(content);

        }
        else
        {
            GameDebug.Log(string.Format("{0} 不存在。", path), GameDebug.Level.Error);
            return default;
        }

    }
    /// <summary>
    /// yaml写入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="types">子文件夹类型</param>
    /// <param name="fileName">文件名（不含拓展名）</param>
    /// <param name="directoryName">专用文件夹名字</param>
    /// <param name="content">yaml内容</param>
    public static void YamlWrite<T>(SubdirectoryTypes types, string directoryName, string fileName, T content)
    {
        //提前准备好文件夹的路径（不含最终的文件)
        string path = string.Format("{0}/{1}/{2}", UnityButNotAssets, types.ToString(), directoryName);
        //准备好序列化的yaml内容
        var write = new Serializer();
        var yaml = write.Serialize(content);


        //路径不存在不存在就创建相应的文件夹
        if (!File.Exists(string.Format("{0}/{1}.yaml", path, fileName)))
        {
            Directory.CreateDirectory(path);
        }

        //直接创建一个新的文件得了，顺便用这个文件流写进去
        var f = new FileStream(string.Format("{0}/{1}.yaml", path, fileName), FileMode.Create);
        StreamWriter sw = new(f, System.Text.Encoding.UTF8);
        sw.Write("#{0}\n# 请不要直接修改本文件\n# 如需修改，请使用游戏自带的编辑器\n\n{1}",VersionControl[(int)types].ToString(), yaml);
        sw.Close();
        f.Close();
    }

    #endregion


    #region  私有函数
    /// <summary>
    /// 读取全部子文件夹，可能包含非法子文件夹，需要对应的代码进行判断
    /// </summary>
    /// <param name="subdirectoryTypes"></param>
    static DirectoryInfo[] ReadAllSubdirectory(SubdirectoryTypes subdirectoryTypes)
    {
        string path = string.Format("{0}/{1}", UnityButNotAssets, subdirectoryTypes.ToString());
        if (Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //得到SpellCards文件夹下的所有子文件夹（就一级）
        DirectoryInfo folder = new DirectoryInfo(path);
        return folder.GetDirectories();
    }

    #endregion

}
