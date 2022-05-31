using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuCtrl : MonoBehaviour
{
    /// <summary>
    /// 声明一个媒体加载器
    /// </summary>
   private readonly MediaLoader mediaLoader = new();
    /// <summary>
    /// 预设。歌曲信息的卡片
    /// </summary>
    public GameObject songsInf;
    public Transform manifestParent;

    public static MenuCtrl menuCtrl;

    [Header("左侧信息ʾ")]
    public TMP_Text musicName;
    public Image musicIcon;
    

    private void Awake()
    {
        menuCtrl = this;

        CreateManifest();
    }
   
    /// <summary>
    /// 创建清单
    /// </summary>
    [ContextMenu("创建清单")]
    public void CreateManifest()
    {
        //先清楚清单
        ClearManifest();
        //获取清单列表，并加载所需的资源，并指定EventSystem的FirstSelected参数
        StartCoroutine(Load());      
    }


    /// <summary>
    /// 选择后播放preBGM，并在左侧显示信息
    /// </summary>
    public void OnSelected(SongsInf songsInf)
    {
        //播放preBGM
       PublicUI.publicUI.PlayPreBGM(songsInf.PreBGM);
        //更新左侧信息
        musicIcon.sprite = songsInf.Icon.sprite;
        musicName.text = songsInf.MusicName.text;
    }

    /// <summary>
    /// 获取清单列表，并加载所需的资源，并指定EventSystem的FirstSelected参数
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        //获取清单列表
        var list = YamlAndFormat.ManifestList();

        //获取清单
        for (int i = 0; i < list.Count; i++)
        {
            //加载媒体资源.icon和preBGM
           yield return StartCoroutine(mediaLoader.LoadSound(YamlAndFormat.SubdirectoryTypes.SpellCards, string.Format("{0}/{1}", list[i].Name, list[i].PreviewBGM)));
           yield return StartCoroutine(mediaLoader.LoadImage(YamlAndFormat.SubdirectoryTypes.SpellCards, string.Format("{0}/{1}", list[i].Name, list[i].Icon)));

            GameObject go = Instantiate(songsInf, manifestParent, false);
            go.transform.SetParent(manifestParent);
            go.GetComponent<SongsInf>().ApplyInf(list[0].MusicName, list[0].Author, list[0].Origin, list[0].Version, mediaLoader.Sprite, list[0].IsAdvanced, mediaLoader.AudioClip,list[0].AllowedDifficulty);
       
        
            if(i == 0)
            {
                PublicUI.publicUI.SetEventSystemFirstSelected(go);
            }
        }
    }

    /// <summary>
    /// 清楚清单
    /// </summary>
    [ContextMenu("清除清单")]
    public void ClearManifest()
    {

        for (int i = manifestParent.childCount - 1; i >= 0; i--)
        {
          DestroyImmediate(manifestParent.GetChild(i).gameObject);

        }
    }
}
