using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public  class basicEvents  : MonoBehaviour,IUpdate
{
    public int gameScene;
    public int LoadScene;
    public int OpeningScene;

    [Header("用于游戏暂停")]
    public UnityEvent OnEscapeClick;
    
    private void Start()
    {
        UpdateManager.RegisterUpdate(this);
    }

    public void ExitGame()
    {
        Settings.SaveSettings();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    
    public void StartGame()
    {
        clickSound();
        SceneManager.LoadScene(LoadScene);//load这边已经停止音乐播放了
    }

    public  static void clickSound()
    {
        if (PublicAudioSource.publicAudioSource != null)   PublicAudioSource.publicAudioSource.PlaySoundEffect(PublicAudioSource.AudioType.Click);
    }

    public  static void OpenWeb()
    {
        clickSound();
        Application.OpenURL("https://kitaujigamedesign.top");
    }
    
    public  void ReturnToTitle()
    {
        if (PublicAudioSource.publicAudioSource != null)   PublicAudioSource.publicAudioSource.StopMusicPlaying();
        clickSound();
        SceneManager.LoadScene(OpeningScene);
        Settings.SaveSettings();
    }

    public void PlayAgain()
    {
        if (PublicAudioSource.publicAudioSource != null)   PublicAudioSource.publicAudioSource.StopMusicPlaying();
        clickSound();
        SceneManager.LoadScene(LoadScene);
        Settings.SaveSettings();
    }
    
    public static void Pause()
    {
        
        
        if (Time.timeScale >= 0.5F)
        {
           
          
        
            Time.timeScale = 0f;
            
            if( StaticVideoPlayer.staticVideoPlayer != null) StaticVideoPlayer.staticVideoPlayer.Pause();

            if (PublicAudioSource.publicAudioSource != null)
            {
                PublicAudioSource.publicAudioSource.PauseMusicPlaying();
                PublicAudioSource.publicAudioSource.PlaySoundEffect(PublicAudioSource.AudioType.Click);
            }
         
        }
        else
        {

           
                Time.timeScale = 1f;
                if( StaticVideoPlayer.staticVideoPlayer != null)   StaticVideoPlayer.staticVideoPlayer.Play();
                if (PublicAudioSource.publicAudioSource != null)  PublicAudioSource.publicAudioSource.PlayBackgroundMusic(null);

        }
     
    }


    public void Debug(string text)
    {
        UnityEngine.Debug.Log(text);
    }



    public void FastUpdate()
    {
     
        
      //响应玩家的按键
      if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > 0.5f)
      {
          OnEscapeClick.Invoke();
          
       
      }
    }
}
