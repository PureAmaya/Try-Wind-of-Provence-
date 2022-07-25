using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 设置读取与应用（不涉及ui，仅修改读取文件，包含玩家可需改设置与不可修改的通用配置）
/// </summary>
//[CreateAssetMenu(fileName ="Settings",menuName ="Tools/CreateSettingsAsset")]
public class Settings :ScriptableObject
{
    public static Settings GameSettings;
    
    
    #if UNITY_EDITOR


    
    
    #endif
    
    
    [Space(20)]
    /// <summary>
    /// 总音量
    /// </summary>
    public float masterVol = 1f;

    /// <summary>
    /// BGM preBGM音量
    /// </summary>
    public float BGMvol = 0.7f;
    /// <summary>
    /// 音效音量
    /// </summary>
    public float SoundEffectsVol = 0.7f;



}
