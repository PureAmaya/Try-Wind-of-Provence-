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
    public GameObject EnterGameButton;

    private SongsInf OnselectedInf;

    
    
    private void Awake()
    {
        menuCtrl = this;

        CreateManifest();
        
        //初始化
        StageName.text = "游戏加载中......";
        EnterGameButton.SetActive(false);
    }
   
    /// <summary>
    /// 创建清单列表
    /// </summary>
    [ContextMenu("创建清单")]
    public void CreateManifest()
    {
        
        //获取清单列表，并加载所需的资源，并指定EventSystem的FirstSelected参数
        StartCoroutine(Load());      
    }

   

    /// <summary>
    /// 选择后在左侧显示信息
    /// </summary>
    public void OnSelected(SongsInf songsInf)
    {
        OnselectedInf = songsInf;
      
        //更新左侧信息
        StageIcon.sprite = songsInf.GetImage();
        StageName.text = songsInf.UsedManifestInf.StageName;
        StageInstruction.text = songsInf.UsedManifestInf.Instruction;
       
        //激活进入游戏的按钮
        EnterGameButton.SetActive(true);
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
            yield return StartCoroutine(mediaLoader.LoadImage(string.Format("Stages/{0}/{1}/Assets/{2}", list[i].Author,
                list[i].Name, list[i].Icon)));
           //不断循环，等到成功读取到媒体资源为止
            while (true)
            {
                if(mediaLoader.ImageLoadStatue == 1)
                {
                    StageName.text = "右面选一个→ ";
                    break;
                }
                else if (mediaLoader.ImageLoadStatue == 0)
                {
                    StageName.text = "加载中...... ";
                    yield return new WaitForEndOfFrame();
                }
                //出粗的情况 -1
                else
                {
                    StageName.text = "载入错误，详细原因请看控制台";
                    break;
                }
               
            }
            
            //媒体资源可用才创建
            if(mediaLoader.ImageLoadStatue == 1)
            {
                //在右侧创造卡片，并更新信息
                GameObject go = Instantiate(songsInf, manifestParent, false);
                go.transform.SetParent(manifestParent);
                go.GetComponent<SongsInf>().ApplyInf(list[i],mediaLoader.GetImage());
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
    
    
    #region 外部UI交互，事件

    public void EnterGame()
    {
        SceneLoader.AddressableAsyncLoadScene(string.Format("Stages/{0}/{1}/Assets/{1}.unity",OnselectedInf.UsedManifestInf.Author,OnselectedInf.UsedManifestInf.Name));
    }
    
    /// <summary>
    /// 按钮用。强制重新加载manifest
    /// </summary>
    public void ForceToReloadAllManifest()
    {
        //先清楚清单
        ClearManifest();
        CreateManifest();
        
    }
    #endregion
}
