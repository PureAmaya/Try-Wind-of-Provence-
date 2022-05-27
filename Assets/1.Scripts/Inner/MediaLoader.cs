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
    public AudioClip AudioClip;
    public Sprite Sprite;

    //所有的se音效也在这里，另外也预留三个audioclip给玩家自定义

    /// <summary>
    /// 加载ogg声音
    /// </summary>
    /// <param name="types"></param>
    /// <param name="fileName">文件名（不含拓展名）</param>
    /// <returns></returns>
    public IEnumerator LoadSound(Core.SubdirectoryTypes types, string fileName)
    {

        //缓存路径
        string filepath = string.Format("{0}/{1}/{2}.ogg", Core.UnityButNotAssets,types.ToString(), fileName);
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
                AudioClip = DownloadHandlerAudioClip.GetContent(uwr);
            }

        }
        else
        {
            GameDebug.Log(string.Format("{0}不存在或文件格式不支持", filepath),GameDebug.Level.Error);
            yield return null;
        }
    }

    /// <summary>
    /// 加载png图片
    /// </summary>
    /// <param name="types"></param>
    /// <param name="fileName">文件名（不含拓展名）</param>
    public IEnumerator LoadImage(Core.SubdirectoryTypes types, string fileName)
    {
        //缓存路径
        string filepath = string.Format("{0}/{1}/{2}.png", Core.UnityButNotAssets, types.ToString(), fileName);

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
                Sprite = Sprite.Create(texture, new Rect(0f,0f,texture.width,texture.height), Vector2.zero);
            }

        }
        else
        {
            GameDebug.Log(string.Format("{0}不存在或文件格式不支持", filepath), GameDebug.Level.Error);
            yield return null;
        }
    }

}
