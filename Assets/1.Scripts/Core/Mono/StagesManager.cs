using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;


#if  UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[DisallowMultipleComponent]
public class StagesManager : MonoBehaviour
{
    public YamlAndFormat.Manifest selfManifestText;

#if UNITY_EDITOR
    
   
    public Sprite ManifestIcon;
    
    
    public AssemblyDefinitionAsset[] stageScriptsStorage;
#endif

    /*
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
*/
    
    /// <summary>
    /// 创建manifest文件（同步）
    /// </summary>
    public void CreateManifest()
    {
        YamlAndFormat.YamlWrite(DefaultDirectory.SubdirectoryTypes.Assets, string
                .Format("Stages/{0}/{1}", selfManifestText.Author, selfManifestText.Name), "Manifest",
            selfManifestText);
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