using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.GUI;
using UnityEditor.AddressableAssets.Settings;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(StagesManager))]
public class StageManagerEditor : Editor
{
    
    private StagesManager stagesManager;

    #region  状态调整
    /// <summary>
/// manifest用的折叠
/// </summary>
    private bool foldOutForManifest = false;
/// <summary>
/// manifest的图标
/// </summary>
public Sprite manifestIcon;
    #endregion

    private void Awake()
    {
        stagesManager = (StagesManager)target;

    }

    
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("初始化Addressable"))
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            //检查是否存在这个组（以关卡名称命名的）
            AddressableAssetGroup group = settings.FindGroup(stagesManager.selfManifestText.Name);
            //找得到就删除
            if (group != null)
            {
                settings.RemoveGroup(group);
            }

            //之后再新建一个新的，保证资源是新的
            group = settings.CreateGroup(stagesManager.selfManifestText.Name, false, false, false,
                settings.DefaultGroup.Schemas, settings.GetType());

            //添加依赖的标签
            settings.AddLabel("Manifest", false);
            settings.AddLabel("Dll", false);
            settings.AddLabel("Stages", false);
            
         
            
        }
        
        EditorGUILayout.Space(15f);
        //创建manifest图标设置，所用的折叠
        foldOutForManifest = EditorGUILayout.Foldout(foldOutForManifest, "图标信息");

        if (foldOutForManifest)
        {
            //选定封面图片
            manifestIcon = EditorGUILayout.ObjectField(manifestIcon, typeof(Sprite), false) as Sprite;
            //按照选定的图片，写入manifest文件中
            stagesManager.selfManifestText.Icon = AssetDatabase.GetAssetPath(manifestIcon).Split('/')[^1];

            if (manifestIcon != null)
            {
                if(!File.Exists(string.Format("{0}/Stages/{1}/{2}/{3}",Application.dataPath,stagesManager.selfManifestText.Author,stagesManager.selfManifestText.Name,stagesManager.selfManifestText.Icon)))
                {
                EditorGUILayout.HelpBox(string.Format("图标{0}尚未被移动到指定目录\n按下\"创建manifest文件\"以解决此问题",manifestIcon.name),MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox(string.Format("已有同名的图标{0}在指定目录。\n创建manifest后将覆盖源文件",manifestIcon.name),MessageType.Warning);
                }
            }
            
            EditorGUILayout.Space(5f);
          
        }
        
        if (GUILayout.Button("创建manifest文件"))
        {
            //创建manifest
            stagesManager.CreateManifest();

            //把图标文件移动到manifest文件旁边
            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(manifestIcon),
                String.Format("Assets/Stages/{0}/{1}/{2}", stagesManager.selfManifestText.Author,
                    stagesManager.selfManifestText.Name, AssetDatabase.GetAssetPath(manifestIcon).Split('/')[^1]));
           
            AssetDatabase.Refresh();
           
            
        }


        if (GUILayout.Button("将代码转换为二进制文件"))
        {
            stagesManager.CreateDllBytes();
            AssetDatabase.Refresh();
        }
        
        
        EditorGUILayout.Space(40f);

        if (GUILayout.Button("跳转到指定目录"))
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(
                string.Format("Assets/Stages/{0}/{1}", stagesManager.selfManifestText.Author,
                    stagesManager.selfManifestText.Name), typeof(DefaultAsset));
        }
        
        /*
             AssetDatabase.Refresh();
             //得到那个新写出来的文件
             manifestFile = AssetDatabase.LoadAssetAtPath<TextAsset>(string.Format("StreamingAssets/{0}/Manifest.yaml",
                 stagesManager.selfManifest.Name));
             //把新写的manifest加入到group中
            AddressableAssetEntry manifestEntry= settings.CreateOrMoveEntry(
                AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(manifestFile)).ToString(), group, false,
                false);
            //Manifest添加标签
            manifestEntry.SetLabel("Manifest",true,true,false);*/
       
    }

}