using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
#endif

[DisallowMultipleComponent]
public class StagesManager : MonoBehaviour
{
    public YamlAndFormat.Manifest selfManifestText;

    //仅仅用于写入manifest.
#if UNITY_EDITOR

    /// <summary>
    /// 选择关卡的图标
    /// </summary>
    public Sprite ManifestIcon;

    /// <summary>
    /// 关卡用到的场景
    /// </summary>
    public SceneAsset[] stageScenesStorage;

    /// <summary>
    /// 关卡用到的所有脚本的程序集
    /// </summary>
    public AssemblyDefinitionAsset[] stageScriptsStorage;
#endif


    #region 私有

    #endregion
}