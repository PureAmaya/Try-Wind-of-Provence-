using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class Loading: MonoBehaviour
{
    
    public int  GameScene;
    
   public TMP_Text loadingState;

   /// <summary>
   /// 此游戏有小剧场吗
   /// </summary>
   [Tooltip(" 此游戏有小剧场吗")]
    public bool UseDiaglogue = false;
    
    /// <summary>
    /// 本轮游戏的小剧场
    /// </summary>
    public static YamlReadWrite.Dialogue selectedDialogue;
    /// <summary>
    /// 本轮游戏的小剧场的背景图
    /// </summary>
     public  static Sprite dialogueImage;
    
    
    // Start is called before the first frame update
    private IEnumerator Start()
    {
      if(PublicAudioSource.publicAudioSource != null)  PublicAudioSource.publicAudioSource.StopMusicPlaying();
        
        
        loadingState.text = "少女调音中.....\n清除无用资源";
        Settings.SaveSettings();  
        GC.Collect();
        yield return Resources.UnloadUnusedAssets();
        GC.Collect();
        yield return Resources.UnloadUnusedAssets();
        
 
        
        loadingState.text = "少女调音中.....\n载入游戏场景";
        yield return  SceneManager.LoadSceneAsync(GameScene);
     
        
    }
    
 
}