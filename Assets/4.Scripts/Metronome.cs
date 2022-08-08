using UnityEngine;

public class Metronome : MonoBehaviour
{
    [Range(30,244)]
    public int bpm;

    [Range(0f,1f)]
    public float volume = 1f;
   
    /// <summary>
    /// 开始时间偏移（仅推迟）
    /// </summary>
    public float startTimeOffset = 0f;
    

    public AudioSource audioSource;
    /// <summary>
    /// 节拍器的音效
    /// </summary>
    public AudioClip sound;

    
    
    
    private bool isPlaying;
    
    public void StartPlay()
    {
        if(isPlaying) return;
        isPlaying = true;
        InvokeRepeating("Play",startTimeOffset,60f / bpm);
    }

    public void Stop()
    {
        isPlaying = false;
        CancelInvoke();
    }
 
   /// <summary>
   /// 按照拍子播放音效
   /// </summary>
    void Play()
    {
        audioSource.PlayOneShot(sound,volume);
    }

   
}
