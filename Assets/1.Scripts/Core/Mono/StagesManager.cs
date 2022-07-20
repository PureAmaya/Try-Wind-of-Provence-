using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

#if  UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[DisallowMultipleComponent]
public class StagesManager : MonoBehaviour
{
    public YamlAndFormat.Manifest selfManifestText;

#if UNITY_EDITOR
    public AssemblyDefinitionAsset[] StageScripts;
    
    public void CreateDllBytes()
    {
        foreach (var VARIABLE in StageScripts)
        {
            //unity已经帮着编译好了的dll的位置
            var path = string.Format("{0}/Library/ScriptAssemblies/{1}.dll", DefaultDirectory.UnityButNotAssets,
                VARIABLE.name);

            //二进制保存路径，保存在StreamingAssets/{作者名称}/{关卡名称}/DllBytes文件夹中
            var savePath = string.Format("{0}/{1}/{2}/DllBytes", Application.streamingAssetsPath, selfManifestText.Author,
                selfManifestText.Name);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            //补上文件名和拓展名
            savePath = string.Format("{0}/{1}.DllBytes", savePath, VARIABLE.name);
            DllToBytes.DLLToBytesExternal(path, savePath);
            Debug.Log(savePath);
        }
    }
#endif

    /// <summary>
    /// 把这个关卡的所有二进制文件还原为dll
    /// </summary>
    /// 
    [ContextMenu("把二进制文件还原为dll")]
    public void BytesToDll()
    {
        foreach (var VARIABLE in StageScripts)
        {
            StartCoroutine(DllBytesLoadFromAddressable(VARIABLE.name));
        }
    }

    /// <summary>
    /// 创建manifest文件
    /// </summary>
    public void CreateManifest()
    {
        //添加manifest
        //在StreamingsAssets/{关卡名称}文件夹中写一个manifest文件
        StartCoroutine(YamlAndFormat.YamlWrite(DefaultDirectory.SubdirectoryTypes.Assets, string
                .Format("Assets/Stages/{0}/{1}", selfManifestText.Author, selfManifestText.Name), "Manifest",
            selfManifestText));
    }

    /// <summary>
    /// 从ab中加载二进制文件
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    IEnumerator DllBytesLoadFromAddressable(string assemblyName)
    {
        //异步加载二进制文件
        yield return DllLoader.LoadFromAddressables(string.Format(
            "Stages/{0}/{1}/Scripts/Stages.PureAmaya.Kaguya3rdED.DllBytes",
            selfManifestText.Author, selfManifestText.Name));
    }


    #region 私有

    #endregion
}