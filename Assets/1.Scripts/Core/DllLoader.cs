/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DllLoader
{
    /// <summary>
    /// 临时存储读取的二进制文件
    /// </summary>
    private static byte[] bytes;

    private static bool AllDllLoadDone = false;

    private static ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    
    /// <summary>
    /// 加载之前的dll（这个方法就只加载一个）
    /// </summary>
    /// <param name="authorName">作者名字</param>
    /// <param name="nameOfFile">文件名字。从manifest中获取，不含拓展名。保存的dll也是这个名字</param>
    ///  <param name="stageName">对应manifest.name</param>
    public static IEnumerator DllLoad(string authorName, string stageName, string nameOfFile)
    {
        //以防万一，初始化bytes字段和加载状态
        bytes = null;
        AllDllLoadDone = false;
        
        //addressable中的key
        string path = string.Format("Stages/{0}/{1}/DllBytes/{2}.bytes", authorName, stageName, nameOfFile);

        //得到二进制
       Addressables.LoadAssetAsync<TextAsset>(path).Completed += DllLoadDone;

       
       //等加载完
       WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
       while (true)
       {
           if (AllDllLoadDone)
           {
               break;
           }
           else
           {
               yield return waitForEndOfFrame;
           }
       }
       
        
        //dll存放的外部路径
        
        //二进制转化为dll文件，并存放在临时缓存目录里
      //  yield return IEBytesToDllOrPdb(authorName, stageName, nameOfFile);
        
        //使用ILRuntime，读取dll
        ILRuntimeLoadDll();
    }
    
    
    #region 私有函数

    /// <summary>
    /// 异步加载二进制文件完事了（from Addressable）
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static void DllLoadDone(AsyncOperationHandle<TextAsset> asyncOperationHandle)
    {
        if (asyncOperationHandle.Task.IsCompletedSuccessfully)
        {
            AllDllLoadDone = true;
            bytes = asyncOperationHandle.Result.bytes;
        }
    }

    /// <summary>
    /// 将二进制内容转换成的dll(pdb)，并保存在（外部）Cache/Stages/{作者名称}/{关卡名称}/DllBytes/中
    /// </summary>
    /// <param name="authorName">作者名字</param>
    /// <param name="nameOfFile">文件名字。从manifest中获取，不含拓展名。保存的dll也是这个名字</param>
    ///  <param name="stageName">对应manifest.name</param>
    /// <param name="bytes"></param>
    [Obsolete]//ILRuntime可能不需要把二进制转换为dll
    static IEnumerator  IEBytesToDllOrPdb(string authorName, string stageName, string nameOfFile)
    {
        //保存路径（外部）Cache/Stages/{作者名称}/{关卡名称}/DllBytes。
        string path = string.Format("{0}/Cache/Stages/{1}/{2}/DllBytes", DefaultDirectory.UnityButNotAssets, authorName,
            stageName);

        if (Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //写正经的dll文件。确定好dll的路径
        path = string.Format("{0}/{1}.dll", path, nameOfFile);

        yield return File.WriteAllBytesAsync(path, bytes);
    }

/// <summary>
/// 使用ILRuntime来加载dll（二进制直接加载，无参数）
/// </summary>
/// <param name="pat">dll的路径（含拓展名）</param>
/// <returns></returns>
    static void ILRuntimeLoadDll()
    {
        Debug.Log("Dll加载完了");
        
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        
        //新建内存流，读取之前存放的二进制dll（不含pdb）
        MemoryStream dll = new MemoryStream(bytes);
        
        //加载二进制dll
        appdomain.LoadAssembly(dll);
        
        
        
 }
    #endregion
}
*/