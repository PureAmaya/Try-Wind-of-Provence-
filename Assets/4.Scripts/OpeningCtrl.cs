using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningCtrl : MonoBehaviour
{
    /// <summary>
    /// 欢迎界面的BGM
    /// </summary>
    public AudioClip Music;
    
    private void Awake()
    {
        //读取设置文件
        Settings.ReadSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        //播放BGM
        PublicAudioSource.PlayBackgroundMusic(Music);
        
    }

    public void ExitGame()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    public void GameSetting()
    {
        
    }
    

    public void StartTutorial()
    {
        
    }
    
    public void StartGame()
    {
        PublicAudioSource.StopMusicPlaying();
        SceneManager.LoadScene("LOADING");
    }
}
