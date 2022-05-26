using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 媒体加载器
/// </summary>
public class MediaLoader
{
    public AudioClip audioClip = null;
    public Sprite sprite = null;

    //所有的se音效也在这里，另外也预留三个audioclip给玩家自定义

    /// <summary>
    /// 
    /// </summary>
    /// <param name="types"></param>
    /// <param name="FileName">文件名（不含拓展名）</param>
    ///  <param name="DirectoryName">专用文件夹名字（支持构建多个文件夹）</param>
    /// <returns></returns>
    public IEnumerator LoadSound(Core.SubdirectoryTypes types, string FileName)
    {
        //缓存路径
        string filepath = string.Format("{0}/{1}/{2}.ogg", Core.UnityButNotAssets,types.ToString(), FileName);
        //音频文件存在
        if(File.Exists(filepath))
        {
            //从本地读取资源
            var uwr = UnityWebRequestMultimedia.GetAudioClip(string.Format("file://{0}", filepath), AudioType.OGGVORBIS);
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success && uwr.result != UnityWebRequest.Result.InProgress)
            {
                GameDebug.Log(uwr.error, GameDebug.Level.Error);
            }
            else
            {
                audioClip = DownloadHandlerAudioClip.GetContent(uwr);
            }

        }
        else
        {
            GameDebug.Log(string.Format("{0}不存在或文件格式不支持", filepath),GameDebug.Level.Error);
            yield return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="types"></param>
    /// <param name="FileName">文件名（不含拓展名）</param>
    public IEnumerator LoadImage(Core.SubdirectoryTypes types, string FileName)
    {
        //缓存路径
        string filepath = string.Format("{0}/{1}/{2}.png", Core.UnityButNotAssets, types.ToString(), FileName);

        //图片文件存在
        if (File.Exists(filepath))
        {
            //从本地读取资源
            var uwr = UnityWebRequestTexture.GetTexture(string.Format("file://{0}", filepath));
            Debug.Log(uwr.url);
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success && uwr.result != UnityWebRequest.Result.InProgress)
            {
                GameDebug.Log(uwr.error, GameDebug.Level.Error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                sprite = Sprite.Create(texture, new Rect(0f,0f,texture.width,texture.height), Vector2.zero);
            }

        }
        else
        {
            GameDebug.Log(string.Format("{0}不存在或文件格式不支持", filepath), GameDebug.Level.Error);
            yield return null;
        }
    }

}
