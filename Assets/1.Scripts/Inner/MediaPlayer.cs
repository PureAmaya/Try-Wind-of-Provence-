using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MediaPlayer : MonoBehaviour
{
    public static AudioClip BGM;

    //所有的se音效也在这里，另外也预留三个audioclip给玩家自定义

    /// <summary>
    /// 
    /// </summary>
    /// <param name="types"></param>
    /// <param name="FileName">文件名（含拓展名）</param>
    /// <returns></returns>
    public IEnumerator LoadMusicAndSE(Core.SubdirectoryTypes types, string FileName, AudioType audioType = AudioType.MPEG)
    {
        string filepath = string.Format("{0}/{1}/{2}", Core.UnityButNotAssets, types.ToString(), FileName);

        var uwr = UnityWebRequestMultimedia.GetAudioClip(filepath, audioType);

        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success && uwr.result != UnityWebRequest.Result.InProgress)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            BGM = DownloadHandlerAudioClip.GetContent(uwr);
        }
    }

}
