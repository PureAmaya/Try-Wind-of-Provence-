using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UPRLuaProfiler;
#if UNITY_2018_2_OR_NEWER
using UnityEditor.Build.Reporting;

namespace  UPRProfiler
{
    public class IL2PPRofiler : IPostBuildPlayerScriptDLLs
    {
        public int callbackOrder { get { return 0; } }
        public static ScriptingImplementation scriptBackend;
        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            string AssemblyPath = "";
            var platform = report.summary.platform;


            if (platform == BuildTarget.StandaloneWindows64 || platform == BuildTarget.StandaloneWindows)
            {
                scriptBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone);
                if (scriptBackend == ScriptingImplementation.IL2CPP)
                {
                    if (Directory.Exists("./Temp/StagingArea/Il2Cpp/Managed/"))
                    {
                        AssemblyPath = "./Temp/StagingArea/Il2Cpp/Managed/";
                    }
                    if (Directory.Exists("./Temp/StagingArea/Data/Managed/"))
                    {
                        AssemblyPath = "./Temp/StagingArea/Data/Managed/";
                    }
                }
                    
            } 
            if(platform == BuildTarget.Android)
            {
                scriptBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
                if (scriptBackend == ScriptingImplementation.IL2CPP)
                {
                    if (Directory.Exists("./Temp/StagingArea/Il2Cpp/Managed/"))
                    {
                        AssemblyPath = "./Temp/StagingArea/Il2Cpp/Managed/";
                    }
                    if (Directory.Exists("./Temp/StagingArea/Data/Managed/"))
                    {
                        AssemblyPath = "./Temp/StagingArea/Data/Managed/";
                    }
                }
            }
            
            if(platform == BuildTarget.iOS || platform == BuildTarget.WebGL)
            {
                if (Directory.Exists("./Temp/StagingArea/Il2Cpp/Managed/"))
                {
                    AssemblyPath = "./Temp/StagingArea/Il2Cpp/Managed/";
                }
                if (Directory.Exists("./Temp/StagingArea/Data/Managed/"))
                {
                    AssemblyPath = "./Temp/StagingArea/Data/Managed/";
                }
                
            }

            if (AssemblyPath != "")
            {
                var setting = UPRToolSetting.Instance;
                
                if (setting.enableMonoProfiler)
                {
                    InjectMethods.InjectAllMethods(AssemblyPath + "Assembly-CSharp.dll");
                    Debug.Log("Listening Mono Profiler Success");
                }
                #if !UNITY_2020_3_OR_NEWER
                if (setting.loadScene)
                {
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.CoreModule.dll", "SceneManager", "LoadScene");
                }
                if (setting.loadAsset)
                {
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.AssetBundleModule.dll", "AssetBundle", "LoadAsset");
                }
                if (setting.loadAssetBundle)
                {
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.AssetBundleModule.dll", "AssetBundle", "LoadFromFile");
                }
                if (setting.instantiate)
                {
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.CoreModule.dll", "Object", "Instantiate");
                }
                #endif
            }
         
        }
    }

}
#endif