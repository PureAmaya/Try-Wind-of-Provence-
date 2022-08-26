using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningCtrl : MonoBehaviour
{
    public Slider MusicVolSlider;
    public Slider EffectVolSlider;
    
    /// <summary>
    /// 欢迎界面的BGM
    /// </summary>
    public AudioClip Music;
    
    private void Awake()
    {
        Application.targetFrameRate = -1;
        //读取设置文件
        Settings.ReadSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        //按照文件调整滑块
        MusicVolSlider.value = Settings.SettingsContent.MusicVolume;
        EffectVolSlider.value = Settings.SettingsContent.SoundEffectVolume;

        //注册事件
        MusicVolSlider.onValueChanged.AddListener(delegate(float arg0)
        {
            Settings.SettingsContent.MusicVolume = arg0; PublicAudioSource.UpdateMusicVolume();
        });
        EffectVolSlider.onValueChanged.AddListener(delegate(float arg0)
        {
            Settings.SettingsContent.SoundEffectVolume = arg0;
        });
        
        //播放BGM
        PublicAudioSource.PlayBackgroundMusic(Music);
        
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
        PublicAudioSource.StopMusicPlaying();
        SceneManager.LoadScene("LOADING");
    }

 
}
