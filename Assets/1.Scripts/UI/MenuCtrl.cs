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
    public TMP_Text StageName;
    public Image StageIcon;
    public TMP_Text StageInstruction;

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
        //更新左侧信息
        StageIcon.sprite = songsInf.Icon.sprite;
        StageName.text = songsInf.StagesName.text;
        StageInstruction.text = songsInf.Instruction;
    }

    /// <summary>
    /// 获取清单列表，并加载所需的资源，并指定EventSystem的FirstSelected参数
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        //开始读取addressable，并得到准确的清单数据
        YamlAndFormat.GetManifestList();
        
        //获取清单列表
        while (true)
        {
            if (YamlAndFormat.ManifestLoadStatue == 1)
            {
                break;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
            
        }
        var list = YamlAndFormat.AllManifestsReady;

        //获取清单
        for (int i = 0; i < list.Count; i++)
        {
            //加载媒体资源.icon
            yield return StartCoroutine(mediaLoader.LoadImage(DefaultDirectory.SubdirectoryTypes.Stages, string.Format("{0}/{1}", list[i].Name, list[i].Icon)));

            //在右侧创造卡片，并更新信息
           GameObject go = Instantiate(songsInf, manifestParent, false);
            go.transform.SetParent(manifestParent);
            go.GetComponent<SongsInf>().ApplyInf(list[i],mediaLoader.Sprite);
       
        
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
