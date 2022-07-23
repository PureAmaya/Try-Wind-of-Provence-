using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(StagesManager))]
public class StageManagerEditor : Editor
{
    
    private StagesManager stagesManager;

    /// <summary>
    /// addressable的默认设置
    /// </summary>
    private AddressableAssetSettings settings;

    /// <summary>
    /// 本关卡用的addressable的组
    /// </summary>
    private AddressableAssetGroup group;

   
    private SerializedProperty StageScripts;
    
    #region 状态调整

    /// <summary>
    /// manifest用的折叠
    /// </summary>
    private bool foldOutForManifest = false;
    /// <summary>
    /// dll转换为bytes用的折叠
    /// </summary>
    private bool foldOutForDllConvert = false;


    #endregion

    private void Awake()
    {
        stagesManager = (StagesManager)target;

    }
    

    public override void OnInspectorGUI()
    {
        
        //先更新一下序列化的脚本
        serializedObject.Update();
        
       //ab包的默认设置
        settings = AddressableAssetSettingsDefaultObject.Settings;
        group = settings.FindGroup(stagesManager.selfManifestText.Name);
        
        EditorGUILayout.LabelField("基本信息设置");
        stagesManager.selfManifestText.Author =
            EditorGUILayout.TextField("关卡作者", stagesManager.selfManifestText.Author);
        stagesManager.selfManifestText.Name =
            EditorGUILayout.TextField("关卡名称", stagesManager.selfManifestText.Name);
        stagesManager.selfManifestText.Version =
            EditorGUILayout.TextField("关卡版本号", stagesManager.selfManifestText.Version);
       
        EditorGUILayout.Space(40f);

        //初始化addressable的分组（不会影响其他分组）
        InitializeAddressable();

        EditorGUILayout.Space(15f);

        //将将程序集转换为二进制文件
        ConvertAsmdefToBytesFile();
        EditorGUILayout.Space(15f);

        //创建此关卡的manifest
        CreateThisStageManifest();
        EditorGUILayout.Space(40f);

        //跳转到指定目录
        JumpToDirectory();
        
       
       
        serializedObject.ApplyModifiedProperties();
    }


    #region 每个按钮用的具体方法

    /// <summary>
    /// 初始化addressable
    /// </summary>
    private void InitializeAddressable()
    {
        EditorGUILayout.HelpBox("创建的addressable group的配置与Default Local Group一致。本操作不会影响其他group", MessageType.Info);
        
        if (GUILayout.Button("初始化Addressable"))
        {
            //检查是否存在这个组（以关卡名称命名的）
            group = settings.FindGroup(stagesManager.selfManifestText.Name);

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
    }

    /// <summary>
    /// 创建本文件的manifest文件
    /// </summary>
    private void CreateThisStageManifest()
    {
        //创建manifest图标设置，所用的折叠
        foldOutForManifest = EditorGUILayout.Foldout(foldOutForManifest, "清单设置");

        if (foldOutForManifest)
        {
            
            //选定封面图片用的交互组件
            EditorGUILayout.LabelField("图标设置");
            stagesManager.ManifestIcon = EditorGUILayout.ObjectField(stagesManager.ManifestIcon, typeof(Sprite), false) as Sprite;
            EditorGUILayout.Space(3f);
            EditorGUILayout.LabelField("其他设置");
 stagesManager.selfManifestText.StageName =
                EditorGUILayout.TextField("关卡友好名称", stagesManager.selfManifestText.StageName);
            stagesManager.selfManifestText.ShortInstr =
                EditorGUILayout.TextField("简短介绍", stagesManager.selfManifestText.ShortInstr);
            EditorGUILayout.LabelField("详细介绍（不支持富文本）");
            stagesManager.selfManifestText.Instruction =
                EditorGUILayout.TextArea(stagesManager.selfManifestText.Instruction);
           
           
            
            EditorGUILayout.Space(5f);
        }


        if (GUILayout.Button("创建manifest文件"))
        {
            //按照选定的图片，先修改manifest的icon设置（默认无拓展名）
            stagesManager.selfManifestText.Icon = stagesManager.ManifestIcon.name;
            
            //创建manifest
            stagesManager.CreateManifest();
           
            AssetDatabase.Refresh();

            //把manifest文件添加到ab包中
            AddressableAssetEntry manifestEntry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(
                String.Format("Assets/Stages/{0}/{1}/Manifest.yaml", stagesManager.selfManifestText.Author,
                    stagesManager.selfManifestText.Name)), group);

            Debug.Log(AssetDatabase.AssetPathToGUID(
                String.Format("Assets/Stages/{0}/{1}/Manifest.yaml", stagesManager.selfManifestText.Author,
                    stagesManager.selfManifestText.Name)));
            Debug.Log(manifestEntry.MainAsset);
            
            
            //将manifest的address修正为规定的样式和label
          manifestEntry.SetAddress("Manifest");
           manifestEntry.SetLabel("Manifest", true);

           //将图标添加到ab包
           manifestEntry =
               settings.CreateOrMoveEntry(
                   AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(stagesManager.ManifestIcon)), group);
            //修正图标的address
           manifestEntry.SetAddress(string.Format("Stages/{0}/{1}/{2}", stagesManager.selfManifestText.Author,
               stagesManager.selfManifestText.Name, stagesManager.ManifestIcon.name));
        }
    }

    /// <summary>
    /// 将程序集转化为二进制文件
    /// </summary>
    private void ConvertAsmdefToBytesFile()
    {
        foldOutForDllConvert = EditorGUILayout.Foldout(foldOutForDllConvert, "asmdef文件设置");
        if (foldOutForDllConvert)
        {
            EditorGUILayout.HelpBox("请保证asmdef的文件名与Name属性相同",MessageType.Warning);
            StageScripts = serializedObject.FindProperty("stageScriptsStorage");
            EditorGUILayout.PropertyField(StageScripts);
        }

        if (GUILayout.Button("将程序集转换为二进制文件") && stagesManager.stageScriptsStorage.Length != 0)
        {
            //初始化manifest中保存的dll二进制文件名
            stagesManager.selfManifestText.DllBytesFileNames = new string[stagesManager.stageScriptsStorage.Length];
            
            for (int i = 0; i < stagesManager.stageScriptsStorage.Length; i++)
            {
                //稍微缩短一下，要不然有点长
                var VARIABLE = stagesManager.stageScriptsStorage[i];
                
                //unity已经帮着编译好了的dll的位置
                var path = string.Format("{0}/Library/ScriptAssemblies/{1}.dll", DefaultDirectory.UnityButNotAssets,
                    VARIABLE.name);

                //二进制保存路径，保存在Assets/Stages/{作者名称}/{关卡名称}/DllBytes文件夹中
                var savePath = string.Format("{0}/Stages/{1}/{2}/DllBytes", Application.dataPath,stagesManager.selfManifestText.Author,
                    stagesManager.selfManifestText.Name);
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                //补上文件名和拓展名
                savePath = string.Format("{0}/{1}.DllBytes", savePath, VARIABLE.name);
                DllToBytes.DLLToBytesExternal(path, savePath);
                Debug.Log(savePath);
                    
                //把保存路径修正一下，把多余的头去掉，从Assets开始
                savePath = string.Format("Stages/{0}/{1}/DllBytes/{2}.DllBytes",
                    stagesManager.selfManifestText.Author, stagesManager.selfManifestText.Name, VARIABLE.name);
                //将程序集转换成的二进制文件储存在ab包中
                AddressableAssetEntry manifestEntry =
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(string.Format("Assets/{0}",savePath)), group);
                //更改为标准的address
                 manifestEntry.SetAddress(savePath);
                //保存到manifest中
                stagesManager.selfManifestText.DllBytesFileNames[i] = VARIABLE.name;

            }
           
            AssetDatabase.Refresh();
        }
        

    }

    /// <summary>
    /// 跳转到指定目录
    /// </summary>
    private void JumpToDirectory()
    {
        if (GUILayout.Button("跳转到指定目录"))
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(
                string.Format("Assets/Stages/{0}/{1}", stagesManager.selfManifestText.Author,
                    stagesManager.selfManifestText.Name), typeof(DefaultAsset));
        }

    }
    
    #endregion
}