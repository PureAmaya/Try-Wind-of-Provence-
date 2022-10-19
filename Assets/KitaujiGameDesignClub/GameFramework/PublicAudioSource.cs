using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PublicAudioSource : MonoBehaviour
{
    public static PublicAudioSource publicAudioSource;

    /// <summary>
    /// 点击音效（长度大于1则随机播放一个）
    /// </summary>
    public AudioClip[] clicks;

    public enum AudioType
    {
        Click, 
    }
    
    private  AudioSource Music;
    private  AudioSource Effect;
    
    private void Awake()
    {
        publicAudioSource = this;
        DontDestroyOnLoad(gameObject);
        
        AudioSource[] audio = GetComponents<AudioSource>();
        if (audio[0].playOnAwake)
        {
            Music = audio[0];
            Effect = audio[1];
        }
        else
        {
            Music = audio[1];
            Effect = audio[0];
        }
        
        //停止播放
        Music.Stop();
    }
    

    // Start is called before the first frame update
   /// <summary>
   /// 
   /// </summary>
   /// <param name="clip">null=继续播放之前的音频</param>
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (Music == null)
        {
            return;
        }

        //先更新一下音量
        UpdateMusicVolume();
        if (clip == null)
        {
            Music.Play();
        }
        else
        {
            //停止之前的播放
            StopMusicPlaying();
            Music.clip = clip;
            Music.Play();
        }
        
      
    }

    public  void UpdateMusicVolume()
    {
        if (Music == null)
        {
            return;
        }
        
        Music.volume = Settings.SettingsContent.MusicVolume;
    }

    public void StopMusicPlaying()
    {
        if (Music == null)
        {
            return;
        }
        
        Music.Stop();
        Music.clip = null;
    }

    public void PauseMusicPlaying()
    {
        if (Music == null)
        {
            return;
        }
        
        Music.Pause();
       
    }
    
    
    

    /// <summary>
    /// 播放音效（公用）
    /// </summary>
    /// <param name="clip"></param>
    public  void PlaySoundEffect(AudioType type)
    {
        if (Effect == null)
        {
            return;
        }
        
        switch (type)
        {
            case AudioType.Click:
               PlaySoundEffect(clicks[Random.Range(0,clicks.Length)]);
                break;
                
        }
      
    }

    /// <summary>
    /// 播放音效（指定）
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySoundEffect(AudioClip clip)
    {
        if (Effect == null)
        {
            return;
        }
        
        Effect.Stop();
        Effect.PlayOneShot(clip,Settings.SettingsContent.SoundEffectVolume);
    }

    public void SoundEffectStop()
    {
        if (Effect == null)
        {
            return;
        }
        
        Effect.Stop();
    }
    
}
