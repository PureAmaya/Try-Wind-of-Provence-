using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

/// <summary>
/// 
/// </summary>
public class StaticVideoPlayer : MonoBehaviour, IUpdate
{
    public static StaticVideoPlayer staticVideoPlayer;

    /// <summary>
    /// 每个视频帧帧都会执行的事件
    /// </summary>
    [Tooltip("每个视频帧帧都会执行的事件")]
    public UnityEvent eachFrame;

    public long frame => VideoPlayer.frame;
    
    
    /// <summary>
    /// 记录视频上一帧是第几帧
    /// </summary>
    private long PreviousFrameInVideo;

   public  VideoPlayer VideoPlayer;

    private AudioSource audioSource;


    private void Awake()
    {
        staticVideoPlayer = this;
        VideoPlayer = GetComponent<VideoPlayer>();

        if (VideoPlayer.playOnAwake)
        {
            Debug.LogError("不允许PlayOnAwake");
            return;
        }

        audioSource = GetComponent<AudioSource>();


        audioSource.volume = Settings.SettingsContent.MusicVolume;
    }

    public void PrepareVideo(bool autoPlay)
    {
        if (VideoPlayer == null || staticVideoPlayer == null)
        {
            return;
        }

        VideoPlayer.Prepare();
        if (autoPlay)
        {
            Play();
        }
    }


    public void Play()
    {
        if (VideoPlayer == null)
        {
            return;
        }

        VideoPlayer.Play();
        PreviousFrameInVideo = VideoPlayer.frame;
        UpdateManager.RegisterUpdate(this);
    }

    public void Pause()
    {
        if (VideoPlayer == null)
        {
            return;
        }


        UpdateManager.Remove(this);
        VideoPlayer.Pause();
    }

    /// <summary>
    /// 跳跃之后，每帧执行的事件失效
    /// </summary>
    /// <param name="frame"></param>
    public void Jump(long frame)
    {
        if (VideoPlayer == null)
        {
            return;
        }

        VideoPlayer.frame = frame;
    }

    /// <summary>
    /// 注册事件：每帧执行的要运行
    /// </summary>
    public void RegisterEachFrame()
    {
        PreviousFrameInVideo = VideoPlayer.frame - 1;
    }


    //音量控制，用事件组
    public void FastUpdate()
    {
        //每多一帧，调用一次方法
        if (VideoPlayer.frame - PreviousFrameInVideo == 1)
        {
            PreviousFrameInVideo++;
            eachFrame.Invoke();
        }

        else if (VideoPlayer.frame - PreviousFrameInVideo > 1)
        {
            PreviousFrameInVideo = VideoPlayer.frame;
            eachFrame.Invoke();
        }
    }

    [ContextMenu("使视频结束")]
    public void VideoEnd()
    {
        VideoPlayer.frame = (long)VideoPlayer.frameCount;
    }
}