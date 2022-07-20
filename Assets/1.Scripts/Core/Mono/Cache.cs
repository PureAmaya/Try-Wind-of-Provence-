using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 缓存数据，防止反复从addressable中读取，拖慢速度（仅存在内存中，重启后丢失）
/// </summary>
[DisallowMultipleComponent]
public class Cache : MonoBehaviour
{
    public static Cache cache;

    #region 缓存内容

    /// <summary>
    /// 缓存创建的manifest列表
    /// </summary>
    [HideInInspector] public List<YamlAndFormat.Manifest> CashForManifestsList = new List<YamlAndFormat.Manifest>();

    /// <summary>
    /// 缓存的manifest的图标
    /// </summary>
    [HideInInspector] public List<Sprite> CacheManifestImage = new List<Sprite>();

    #endregion

    private void Awake()
    {
        cache = this;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 清除掉所有缓存
    /// </summary>
    public void ClearALlCash()
    {
        CashForManifestsList.Clear();
        CacheManifestImage.Clear();
    }
}