using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class PublicUI : MonoBehaviour
{
    public static PublicUI publicUI;

    public EventSystem eventSystem;

    AudioSource BGMPlayer;
    AudioSource PreMusicPlayer;
    AudioSource SEPlayer;




    private void Awake()
    {
        publicUI = this;

        DontDestroyOnLoad(gameObject);

        //获取组件
        AudioSource[] audioSources = GetComponents<AudioSource>();
        BGMPlayer = audioSources[0];
        SEPlayer = audioSources[1];
        PreMusicPlayer = audioSources[2];

        //修正组件
        BGMPlayer.loop = false;
        BGMPlayer.playOnAwake = false;
        SEPlayer.loop = false;
        SEPlayer.playOnAwake = false;
        PreMusicPlayer.loop = true;
        PreMusicPlayer.playOnAwake = false;
    }

    /// <summary>
    /// 设定EventSystem的FirstSelected参数
    /// </summary>
    /// <param name="go"></param>
    public void SetEventSystemFirstSelected(GameObject go)
    {
        eventSystem.firstSelectedGameObject = go;
    }

    /// <summary>
    /// 应用音量
    /// </summary>
    /// <param name="index">0=BGM&preBGM 1=SE  其他值：静音</param>
    public void ApplyVolume(int index)
    {
        switch (index)
        {
            case 0:
                PreMusicPlayer.volume = Settings.MasterVol * Settings.BGMvol;
                BGMPlayer.volume = PreMusicPlayer.volume;
                break;

            case 1:
                SEPlayer.volume = Settings.MasterVol * Settings.SoundEffectsVol;
                break;

            default:
                Settings.MasterVol = 0f;
                SEPlayer.volume = 0f;
                BGMPlayer.volume = 0f;
                PreMusicPlayer.volume = 0f;
                break;


        }
    }


    /// <summary>
    /// 播放完整BGM
    /// </summary>
    /// <param name="audioClip"></param>
   public void PlayFullBGM(AudioClip audioClip)
    {

    }

   

    /// <summary>
    /// 播放预览BGM
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlayPreBGM(AudioClip audioClip)
    {
        //先停止之前的bgm
        PreMusicPlayer.Stop();
        //更换已有的clip
        PreMusicPlayer.clip = audioClip;
        //播放
        PreMusicPlayer.Play();


        //这里应该有淡入效果的
    }
}
