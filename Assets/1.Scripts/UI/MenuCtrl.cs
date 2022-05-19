using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCtrl : MonoBehaviour
{
    /// <summary>
    /// 媒体加载器
    /// </summary>
    MediaLoader mediaLoader = new();
    /// <summary>
    /// 预设的音乐/符卡 卡片
    /// </summary>
    public GameObject songsInf;
    public Transform ManifestParent;

    private void Awake()
    {
        CreateList();
    }



    [ContextMenu("获取列表")]
    ///创建列表
    public void CreateList()
    {

        ClearList();
        StartCoroutine(Load());      
    }

    /// <summary>
    /// 从本地加载资源
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        //先从本地读取文件。
        var list = Core.ManifestList();

        //创建卡片，并向卡片中写入信息
        for (int i = 0; i < list.Count; i++)
        {
            //得到媒体文件
           yield return StartCoroutine(mediaLoader.LoadSound(Core.SubdirectoryTypes.SpellCards, string.Format("{0}/{1}", list[i].Name, list[i].PreviewBGM)));
           yield return StartCoroutine(mediaLoader.LoadImage(Core.SubdirectoryTypes.SpellCards, string.Format("{0}/{1}", list[i].Name, list[i].Icon)));

            GameObject go = Instantiate(songsInf, ManifestParent, false);
            go.transform.SetParent(ManifestParent);
            go.GetComponent<SongsInf>().ApplyInf(list[0].MusicName, list[0].Author, list[0].Origin, list[0].Version, mediaLoader.sprite, list[0].IsAdvanced, mediaLoader.audioClip);
        }
    }

    [ContextMenu("清除列表")]
    /// <summary>
    /// 清除列表
    /// </summary>
    public void ClearList()
    {

        for (int i = ManifestParent.childCount - 1; i >= 0; i--)
        {
          DestroyImmediate(ManifestParent.GetChild(i).gameObject);

        }
    }
}
