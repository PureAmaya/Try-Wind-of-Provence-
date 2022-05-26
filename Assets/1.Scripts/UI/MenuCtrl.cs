using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public static MenuCtrl menuCtrl;

    [Header("左侧信息显示")]
    public TMP_Text MusicName;
    public Image MusicIcon;
    

    private void Awake()
    {
        menuCtrl = this;

        CreateList();
    }
   
    /// <summary>
    /// 创建列表
    /// </summary>
    [ContextMenu("获取列表")]
    public void CreateList()
    {
        //先清除以后列表
        ClearList();
        //获取并加载列表，顺便设定EventSystem的FirstSelected参数
        StartCoroutine(Load());      
    }


    /// <summary>
    /// 铺面被选定。展示信息与播放preBGM
    /// </summary>
    public void OnSelected(SongsInf songsInf)
    {
        //b播放选定的铺面的预览BGM
       PublicUI.publicUI.PlayPreBGM(songsInf.PreBGM);
        //展示缩略图
        MusicIcon.sprite = songsInf.Icon.sprite;
        //展示名称
        MusicName.text = songsInf.MusicName.text;
    }

    /// <summary>
    /// 从本地加载资源（用于列表）
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        //先从本地读取文件。
        var list = Core.ManifestList();

        //创建卡片，并向卡片中写入信息
        for (int i = 0; i < list.Count; i++)
        {
            //得到媒体文件（图标和预览bgm）
           yield return StartCoroutine(mediaLoader.LoadSound(Core.SubdirectoryTypes.SpellCards, string.Format("{0}/{1}", list[i].Name, list[i].PreviewBGM)));
           yield return StartCoroutine(mediaLoader.LoadImage(Core.SubdirectoryTypes.SpellCards, string.Format("{0}/{1}", list[i].Name, list[i].Icon)));

            GameObject go = Instantiate(songsInf, ManifestParent, false);
            go.transform.SetParent(ManifestParent);
            go.GetComponent<SongsInf>().ApplyInf(list[0].MusicName, list[0].Author, list[0].Origin, list[0].Version, mediaLoader.sprite, list[0].IsAdvanced, mediaLoader.audioClip,list[0].AllowedDifficulty);
       
        
            if(i == 0)
            {
                PublicUI.publicUI.SetEventSystemFirstSelected(go);
            }
        }
    }

    /// <summary>
    /// 清除列表
    /// </summary>
    [ContextMenu("清除列表")]
    public void ClearList()
    {

        for (int i = ManifestParent.childCount - 1; i >= 0; i--)
        {
          DestroyImmediate(ManifestParent.GetChild(i).gameObject);

        }
    }
}
