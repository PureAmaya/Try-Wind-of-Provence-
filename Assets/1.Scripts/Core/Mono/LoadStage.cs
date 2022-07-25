using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LoadStage:MonoBehaviour
{
   public static LoadStage loadStage;

   private List<AsyncOperationHandle<SceneInstance>> RecordAllSceneLoaded =
      new List<AsyncOperationHandle<SceneInstance>>();
   
   
   private void Awake()
   {
      loadStage = this;
      DontDestroyOnLoad(this);
   }

   /// <summary>
   /// 开始游戏（按钮按下之后触发的）
   /// </summary>
   public void StartGame(YamlAndFormat.Manifest manifest)
   {
      //先同步进入loadingStage
      SceneManager.LoadScene("LoadStage", LoadSceneMode.Single);

      Debug.Log("只有这个，bug还没修");
      
   //  StartCoroutine(IELoadSceneFromAddressable(manifest));
      
      Debug.Log("这个执行了，bug就解决了" + manifest.Author);
      
      //异步加载ab包中的场景（第一个，即入口场景）
      var stageName = manifest.Name;
      var authorName = manifest.Author;
      var sceneName = manifest.sceneNames[0];
      Addressables.LoadSceneAsync(String.Format("Stages/{0}/{1}/{2}", authorName, stageName, sceneName)).Completed += StageLoadDone;

   }
   /*
  /// <summary>
  /// 从addressable中异步加载1个场景，并应用
  /// </summary>
  /// <param name="manifest"></param>
  /// <returns></returns>
   private IEnumerator IELoadSceneFromAddressable(YamlAndFormat.Manifest manifest)
  {
     var stageName = manifest.Name;
     var authorName = manifest.Author;
     var sceneName = manifest.sceneNames[0];

    
        
     //提前把dll加载好了
     foreach (var VARIABLE in manifest.DllBytesFileNames)
     {
        //加载完了，加载dll
       yield return  DllLoader.DllLoad(authorName, stageName, VARIABLE);
     }


     Debug.Log(String.Format("Stages/{0}/{1}/{2}", authorName, stageName, sceneName));
         
    
      Debug.Log(String.Format("Stages/{0}/{1}/{2}", authorName, stageName, sceneName));
   }*/

   /// <summary>
   /// 释放所有加载的ab中的场景
   /// </summary>
   /// <returns></returns>
   public void IEUnloadAllAddressableScene()
   {
      foreach (var VARIABLE in RecordAllSceneLoaded)
      {
      Addressables.Release(VARIABLE);
      }
   }

   
   /// <summary>
   /// 加加载完了
   /// </summary>
   /// <param name="asyncOperationHandle"></param>
   void StageLoadDone(AsyncOperationHandle<SceneInstance> asyncOperationHandle)
   {
      //保存一下，用于以后release
      RecordAllSceneLoaded.Add(asyncOperationHandle);
   
    
      if (asyncOperationHandle.Task.IsCompleted)
      {
        
      }
   }

}
