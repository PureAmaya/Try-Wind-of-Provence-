using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;


/// <summary>
/// 公共的方法、类啥的
/// </summary>
public class Core
{
    /// <summary>
    /// 预设的子文件夹
    /// </summary>
    public enum SubdirectoryTypes
    {
        SpellCards,
        Skins,
        Settings,
    }

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
                done = string.Format("{0}/{1}", done, raw[i]);
            }
            return done;
        }
    }


    #region YAML用的类的定义部分
    /// <summary>
    /// 用于展示基本信息的清单。不包含弹幕设计(YAML）
    /// </summary>
    public class Manifest
    {
        /// <summary>
        /// 应当与所在的文件夹同名。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 音乐名称，在游戏中展示。
        /// </summary>
        public string MusicName { get; set; }
        /// <summary>
        /// 铺面作者名称。
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// BGM出处/所属专辑。
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 预览音乐（含拓展名）
        /// </summary>
        public string PreviewBGM { get; set; }
        /// <summary>
        /// 该文件的版本。
        /// </summary>
        public string Version { get { return "1.0.0"; } set { } }
    }

    #endregion

    /// <summary>
    /// 读取全部子文件夹，可能包含非法子文件夹，需要对应的代码进行判断
    /// </summary>
    /// <param name="subdirectoryTypes"></param>
    public static DirectoryInfo[] ReadAllSubdirectory(SubdirectoryTypes subdirectoryTypes)
    {
        string path = string.Format("{0}/{1}", UnityButNotAssets, subdirectoryTypes.ToString());
        if (Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //得到SepllCards文件夹下的所有子文件夹（就一级）
        DirectoryInfo folder = new DirectoryInfo(path);
        return folder.GetDirectories();
    }

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
    /// <param name="FileName">文件名（不含拓展名）</param>
    /// <param name="DirectoryName">专用文件夹名字（支持构建多个文件夹）</param>
    /// <returns></returns>
    public static T YamlRead<T>(SubdirectoryTypes types, string DirectoryName, string FileName)
    {       
        //提前准备文件的路径
        string Path = string.Format("{0}/{1}/{2}/{3}.yaml", UnityButNotAssets, types.ToString(), DirectoryName,FileName);

        if (File.Exists(Path))
        {
            Deserializer read = new();
            return read.Deserialize<T>(File.ReadAllText(string.Format("{0}/{1}/{2}/{3}.yaml", UnityButNotAssets, types.ToString(), DirectoryName, FileName, System.Text.Encoding.UTF8)));

        }
        else
        {
            Debug.LogError(string.Format("{0} 不存在。", Path));
            return default;
        }

    }
    /// <summary>
    /// yaml写入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="types">子文件夹类型</param>
    /// <param name="FileName">文件名（不含拓展名）</param>
    /// <param name="DirectoryName">专用文件夹名字（支持构建多个文件夹）</param>
    /// <param name="Content">yaml内容</param>
    public static void YamlWrite<T>(SubdirectoryTypes types, string DirectoryName, string FileName, T Content)
    {
        //提前准备好文件夹的路径（不含最终的文件)
        string Path = string.Format("{0}/{1}/{2}", UnityButNotAssets, types.ToString(), DirectoryName);
        //准备好序列化的yaml内容
        Debug.Log(Content);
        var write = new Serializer();
        var yaml = write.Serialize(Content);


        //路径不存在不存在就创建相应的文件夹
        if (!File.Exists(string.Format("{0}/{1}.yaml", Path, FileName)))
        {
            Directory.CreateDirectory(Path);
        }

        //直接创建一个新的文件得了，顺便用这个文件流写进去
        var f = new FileStream(string.Format("{0}/{1}.yaml", Path, FileName), FileMode.Create);
        StreamWriter sw = new(f, System.Text.Encoding.UTF8);
        sw.Write(string.Format("{0}\n#请不要直接修改本文件。",yaml.ToString()));
        sw.Close();
        f.Close();
    }

    #endregion
}
