using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    public static void LoadSceneAsync(string sceneName,LoadSceneMode loadSceneMode)
    {
        
    }
    
    public static void LoadSceneSync(string sceneName,LoadSceneMode loadSceneMode)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, loadSceneMode);
    }
    
    public static void AddressableAsyncLoadScene(string key)
    {
        Addressables.LoadSceneAsync(key);
    }
}
