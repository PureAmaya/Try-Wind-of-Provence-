using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 
/// </summary>
public class StaticVideoPlayer : MonoBehaviour
{
    public static VideoPlayer videoPlayer;

    private AudioSource audioSource;
    private void Awake()
    {
       
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
    }

  
    //音量控制，用事件组
}
