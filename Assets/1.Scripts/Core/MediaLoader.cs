using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 媒体加载器
/// </summary>
public class MediaLoader
{
    //所有的se音效也在这里，另外也预留三个audioclip给玩家自定义
    AudioClip AudioClip;
    Sprite Sprite;

    /// <summary>
    /// 图片加载状态 -1加载失败 0还没加载完 1成功
    /// </summary>
    int ImageLoadStatue  = 0;



    /// <summary>
    /// 清除加载的图像，并恢复原始的状态
    /// </summary>
    public void ClearLoadedImage()
    {
        Sprite = null;
        ImageLoadStatue = 0;
    }

    /// <summary>
    /// 获取图像加载状态   -1加载失败 0还没加载完 1成功
    /// </summary>
    public int GetImageLoadStatus()
    {
        return ImageLoadStatue;
    }
    
    
/// <summary>
/// 获取加载的图像 
/// </summary>
/// <returns></returns>
    public Sprite GetImage()
    {
        switch (ImageLoadStatue)
        {
            case 1:
                    return Sprite;
                break;
                default:
                    return null;
                break;
        }
    }
    
    
    
    
    
    /// <summary>
    /// 加载外部ogg音频
    /// </summary>
    /// <param name="types"></param>
    /// <param name="fileName">文件名（不含拓展名）</param>
    /// <returns></returns>
    public IEnumerator IELoadSound()
    {
        yield return null;
    }

    /// <summary>
    /// 加载外部png图片
    /// </summary>
    /// <param name="types">子文件夹类型</param>
    /// <param name="fileName">文件名（不含拓展名，仅支持png）</param>
    public IEnumerator IELoadImage(DefaultDirectory.SubdirectoryTypes types, string fileName)
    {
        //还原为尚未加载完成的状态
        ImageLoadStatue = 0;
        
        //缓存路径
        string filepath = string.Format("{0}/{1}/{2}.png", DefaultDirectory.UnityButNotAssets, types.ToString(), fileName);

        //图片文件存在
        if (File.Exists(filepath))
        {
            //从本地读取资源
            var uwr = UnityWebRequestTexture.GetTexture(string.Format("file://{0}", filepath));

            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success && uwr.result != UnityWebRequest.Result.InProgress)
            {
                GameDebug.Log(uwr.error, GameDebug.Level.Warning);
                ImageLoadStatue = -1;
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite = Sprite.Create(texture, new Rect(0f,0f,texture.width,texture.height), Vector2.zero);
                ImageLoadStatue = 1;
            }

        }
        else
        {
            GameDebug.Log(string.Format("{0}不存在或文件格式不支持", filepath), GameDebug.Level.Error);
            ImageLoadStatue = -1;
            yield return null;
        }
    }
    
   /// <summary>
   /// 加载addressable内的图片。
   /// </summary>
   /// <returns></returns>
    public IEnumerator LoadImage(string AddressableKey)
   {
       //还原为尚未加载完成的状态
       ImageLoadStatue = 0;
       
       Addressables.LoadAssetAsync<Texture2D>(AddressableKey).Completed += LoadImageFromAddressable;
        
        
        yield return null;
        
    }
   
   
   #region 内部方法

   private void LoadImageFromAddressable(AsyncOperationHandle<Texture2D> asyncOperationHandle)
   {
       switch (asyncOperationHandle.Status)
       {
           case AsyncOperationStatus.Failed:
               GameDebug.Log(string.Format("图片加载错误：{0}",asyncOperationHandle.OperationException),GameDebug.Level.Warning);
               ImageLoadStatue = -1;
               break;
           case AsyncOperationStatus.None:
               GameDebug.Log(string.Format("图片不存在：{0}",asyncOperationHandle.OperationException),GameDebug.Level.Warning);
               ImageLoadStatue = -1;
               break;
           case  AsyncOperationStatus.Succeeded:
               Sprite = Sprite.Create(asyncOperationHandle.Result, new Rect(0f,0f,asyncOperationHandle.Result.width,asyncOperationHandle.Result.height), Vector2.zero);
               ImageLoadStatue = 1;
               break;
               
       }
   }
   #endregion

}
