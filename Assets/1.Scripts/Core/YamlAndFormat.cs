using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


//版本控制还没弄好啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊

/// <summary>
/// yaml读写，以及yaml相关的格式化（manifest之类的）
/// </summary>
public class YamlAndFormat
{
    #region YAML

    /// <summary>
    /// 用于展示基本信息的清单
    /// </summary>
    [System.Serializable]
    public class Manifest
    {
        /// <summary>
        /// 关卡名称。用于文件夹命名以及定位
        /// </summary>
        public string Name = "Default Stages";

        /// <summary>
        /// 关卡名称。用于对玩家展示
        /// </summary>
        public string StageName = "默认关卡";

        /// <summary>
        /// 关卡图标。与与addressable里的address中一致（默认无拓展名，有路径进行区分）
        /// </summary>
        [InspectorReadOnly] public string Icon = String.Empty;

        /// <summary>
        /// 关卡作者名称。
        /// </summary>
        public string Author = "不愿透露姓名的大佬";

        /// <summary>
        /// 简单介绍。右侧关卡选择列表中，对本关卡进行简单的介绍
        /// </summary>
        public string ShortInstr = "介绍白给了";

        /// <summary>
        /// 详细的介绍。在左侧的面板中，对本关卡进行详细的介绍
        /// </summary>
        [TextArea] public string Instruction = "404 Not Found";

        /// <summary>
        /// 该关卡的版本。（用于对自己做的关卡进行版本控制）
        /// </summary>
        public string Version = "1.0.0";

        /// <summary>
        /// 所有dll二进制文件的名字（与addressable里的address一致，默认无拓展名）
        /// </summary>
        public string[] DllBytesFileNames = new string[1];
    }

    #endregion

    /// <summary>
    /// Manifest加载状态 -1加载失败 0还没加载完 1成功
    /// </summary>
    public static int ManifestLoadStatue = 0;

    /// <summary>
    /// 获取所有的Manifest，用于得到游戏列表。这个是强制性的。必定会从ab中重新获取
    /// </summary>
    /// <returns></returns>
    public static void GetManifestList()
    {
        //状态恢复
        ManifestLoadStatue = 0;
        Cache.cache.CashForManifestsList.Clear();

        //异步加载所有label为manifest的清单
        Addressables.LoadAssetsAsync<TextAsset>(new List<object> { "Manifest", "Manifest" }, null,
            Addressables.MergeMode.Union).Completed += OnCompleteLoadAllManifest;
    }


    #region 读取存放文件

    /// <summary>
    /// yaml读取（从外部yaml文件中读取）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="types">子文件夹类型</param>
    /// <param name="fileName">文件名（不含拓展名）</param>
    /// <param name="directoryName">专用文件夹名字</param>
    /// <returns></returns>
    public static T YamlRead<T>(DefaultDirectory.SubdirectoryTypes types, string directoryName, string fileName)
    {
        //提前准备文件的路径
        string path = string.Format("{0}/{1}/{2}/{3}.yaml", DefaultDirectory.UnityButNotAssets, types.ToString(),
            directoryName, fileName);


        if (File.Exists(path))
        {
            //yaml文件的内容
            string content =
                File.ReadAllText(
                    string.Format("{0}/{1}/{2}/{3}.yaml", DefaultDirectory.UnityButNotAssets, types.ToString(),
                        directoryName, fileName), System.Text.Encoding.UTF8);


            /*
            //如果yaml的版本低于读取规定的版本，则输出一个警告
            if (int.Parse(content.Split("#")[1]) < VersionControl[(int)types])
            {
                GameDebug.Log(string.Format("当前文件的版本已过时，游玩时可能发生错误。低版本的类别为“{0}”、路径为{1}", types.ToString(), path),
                    GameDebug.Level.Warning);
            }*/


            return YamlRead<T>(content);
        }
        else
        {
            GameDebug.Log(string.Format("{0} 不存在。", path), GameDebug.Level.Error);
            return default;
        }
    }

    /// <summary>
    /// yaml读取（直接从string读取）
    /// </summary>
    /// <param name="content"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T YamlRead<T>(string content)
    {
        Deserializer read = new();
        return read.Deserialize<T>(content);
    }

    /// <summary>
    /// yaml向外部写（协程 异步）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="types">子文件夹类型</param>
    /// <param name="fileName">文件名（不含拓展名）</param>
    /// <param name="directoryName">专用文件夹名字</param>
    /// <param name="content">yaml内容</param>
    public static IEnumerator IEYamlWrite<T>(DefaultDirectory.SubdirectoryTypes types, string directoryName,
        string fileName, [NotNull] T content)
    {
        //提前准备好文件夹的路径（不含最终的文件)
        string path = string.Format("{0}/{1}/{2}", DefaultDirectory.UnityButNotAssets, types.ToString(), directoryName);
        Debug.Log(path);
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
        //东西不多，同步赶紧写完得了
        yield return sw.WriteAsync(string.Format("# 请不要直接修改本文件\n# 如需修改，请使用游戏自带的编辑器\n\n{0}", yaml));
        sw.Close();
        f.Close();
        yield return sw.DisposeAsync();
        yield return f.DisposeAsync();
    }

    /// <summary>
    /// yaml向外部写（同步）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="types">子文件夹类型</param>
    /// <param name="fileName">文件名（不含拓展名）</param>
    /// <param name="directoryName">专用文件夹名字。以 / 开头</param>
    /// <param name="content">yaml内容</param>
    public static void YamlWrite<T>(DefaultDirectory.SubdirectoryTypes types, string directoryName, string fileName,
        [NotNull] T content)
    {
        //提前准备好文件夹的路径（不含最终的文件)
        string path = string.Format("{0}/{1}/{2}", DefaultDirectory.UnityButNotAssets, types.ToString(), directoryName);
        Debug.Log(path);
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
        //东西不多，同步赶紧写完得了
        sw.Write("# 请不要直接修改本文件\n# 如需修改，请使用游戏自带的编辑器\n\n{0}", yaml);
        sw.Close();
        f.Close();
        sw.Dispose();
        f.Dispose();
    }

    #endregion


    #region 私有函数

    /// <summary>
    /// 回调函数，检查manifest的加载情况
    /// </summary>
    /// <param name="asyncOperationHandle"></param>
    static void OnCompleteLoadAllManifest(AsyncOperationHandle<IList<TextAsset>> asyncOperationHandle)
    {
        switch (asyncOperationHandle.Status)
        {
            case AsyncOperationStatus.Failed:
                GameDebug.Log(string.Format("{0}部分或所有的Manifest文件读取错误", asyncOperationHandle.OperationException),
                    GameDebug.Level.Warning);
                ManifestLoadStatue = -1;
                break;
            case AsyncOperationStatus.None:
                GameDebug.Log("不存在任何Manifest。可能没有游戏文件或者加载的游戏文件中Manifest错误", GameDebug.Level.Warning);
                ManifestLoadStatue = -1;
                break;
            case AsyncOperationStatus.Succeeded:
                GameDebug.Log(string.Format("Manifest加载成功。共有{0}个可玩游戏", asyncOperationHandle.Result.Count.ToString()),
                    GameDebug.Level.Information);

                //string格式化为class/struct
                for (int i = 0; i < asyncOperationHandle.Result.Count; i++)
                {
                    Cache.cache.CashForManifestsList.Add(YamlRead<Manifest>(asyncOperationHandle.Result[i].text));
                }

                //卸载加载的yaml文件（反正已经保存成可用的类或者结构体了
                Addressables.Release(asyncOperationHandle);
                ManifestLoadStatue = 1;
                break;
        }
    }

    #endregion
}