using System;
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

        //初始化
        StageName.text = "游戏加载中......";
        EnterGameButton.SetActive(false);
    }

    private void Start()
    {
        //没有缓存，就从本地的addressable中重新获取
        if (Cache.cache.CashForManifestsList.Count == 0)
        {
            GameDebug.Log("开始从Addressable中读取游戏列表...",GameDebug.Level.Information);
            StartCoroutine(Load());  
           
        }
        //有缓存的话，就读缓存
        else
        {
            GameDebug.Log("开始从缓存中读取游戏列表...",GameDebug.Level.Information);
            ShowManifestList();
           }
        
       
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
    /// 获取清单列表和他的图标文件
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        //开始读取addressable，并得到准确的清单数据
        YamlAndFormat.GetManifestList();//这行执行之后，Cache.cache.CashForManifestsList里面就有东西了
        
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
        
        //获取清单的必要资源（图片）
        var list = Cache.cache.CashForManifestsList;
        //提示信息更新
        StageName.text = "加载中...... ";
        for (int i = 0; i < list.Count; i++)
        {
            //还原图标加载状态
            mediaLoader.ClearLoadedImage();
            //加载图标资源
            yield return StartCoroutine(mediaLoader.LoadImage(string.Format("Assets/Stages/{0}/{1}/Assets/{2}", list[i].Author,
                list[i].Name, list[i].Icon)));

            while (true)
            {
                //加载成功
                if (mediaLoader.GetImageLoadStatus() == 1)
                {
                    //总之，先给缓存上
                    Cache.cache.CacheManifestImage.Add(mediaLoader.GetImage());
                    break;
                }
                else if (mediaLoader.GetImageLoadStatus() == -1)
                {
                    //占位子，防止顺序错了
                    Cache.cache.CacheManifestImage.Add(null);
                    break;
                }
                else
                {
                    //还没加载完出结果，等到这一帧的结束
                    yield return new WaitForEndOfFrame();
                }

            }
            Cache.cache.CacheManifestImage.Add(mediaLoader.GetImage());
        }
        
        //得到清单列表之后，进行展示（右侧）
        ShowManifestList();

      
    }

    /// <summary>
    /// 已经得到清单列表之后，进行展示（右侧）
    /// </summary>
    /// <returns></returns>
    private void ShowManifestList()
    {
        var list = Cache.cache.CashForManifestsList;
        //提示信息更新
        StageName.text = "加载中...... ";
       

        //获取清单
        for (int i = 0; i < list.Count; i++)
        {
 
                //在右侧创造卡片，并更新信息
                GameObject go = Instantiate(songsInf, manifestParent, false);
                go.transform.SetParent(manifestParent);
                go.GetComponent<SongsInf>().ApplyInf(list[i],Cache.cache.CacheManifestImage[i]);
            
        }
        //提示信息
        StageName.text = "右面选一个→ ";
    }
    
    
    #region 外部UI交互，事件

    public void EnterGame()
    {
        SceneLoader.AddressableAsyncLoadScene(string.Format("Stages/{0}/{1}/Assets/{1}.unity",OnselectedInf.UsedManifestInf.Author,OnselectedInf.UsedManifestInf.Name));
    }
    
    #endregion
    
    
}
