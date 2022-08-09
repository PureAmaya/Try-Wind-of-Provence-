using UnityEngine;
using UnityEngine.Events;

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

    /// <summary>
    /// 节拍器准备开始工作时的事件（startTimeOffset秒之后开始第一次滴答）
    /// </summary>
    public UnityEvent OnReady = new();
/// <summary>
/// 每次滴答一下后调用的事件
/// </summary>
    public UnityEvent<float> AfterTick = new();
    
    private bool isPlaying;
    
    public void StartPlay()
    {
        if(isPlaying) return;
        isPlaying = true;
        OnReady.Invoke();
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
        AfterTick.Invoke(Time.timeSinceLevelLoad);
    }

   
}
