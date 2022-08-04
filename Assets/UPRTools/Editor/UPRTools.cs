using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditorInternal;
using UPRLuaProfiler;
using UPRTools.Editor;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UPRProfiler.Common;


namespace UPRProfiler
{
    public class UPRTools : EditorWindow
    {
        private static bool                       useLua            = false;
        private static bool                       uprLuaProfiler    = false;
        private        bool                       psProfilerRunning = false;
        private        bool                       profilingDone     = false;
        private        List<ParticleSystemPrefab> particleSystemPrefabs;
        private        List<ParticleSystemPrefab> selectedPrefabs; 
        // private        List<int>                  enabledIds;
        private        int                        psPlayIndex;
        private        string                     profileResultPath;
        private        string                     projectId;
        private        string                     particlePlayScene;
        
        [SerializeField] private ProfileManager              psProfileManager;
        [SerializeField] private string                      m_CustomizedServer;
        [SerializeField] private bool                        m_AdvancedSettings;
        [SerializeField] private bool                        m_ParticleSystem;
        [SerializeField]         List<ParticleSystemElement> particleSystemElements;
        [NonSerialized]          bool                        m_Initialized;

        [SerializeField]
        TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading

        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        MultiColumnTreeView m_TreeView;
        ParticleSystemElementsAsset m_MyTreeAsset;

        private static readonly float ui_DefaultMargin = 5;
        private static readonly float ui_ModuleMargin = 5 * ui_DefaultMargin;
        private static readonly float ui_SettingsAreaHeight = 210;

        Rect multiColumnTreeViewRect
        {
            get
            {
                var y = ui_SettingsAreaHeight;
                try
                {
                    y = GUILayoutUtility.GetLastRect().y;
                }
                catch (Exception)
                {
                }

                return new Rect(ui_DefaultMargin, y + ui_ModuleMargin, position.width - 2 * ui_DefaultMargin,
                    position.height - y - ui_SettingsAreaHeight - ui_ModuleMargin);
            }
        }

        Rect uploadResultViewRect
        {
            get
            {
                return new Rect(ui_DefaultMargin, position.height - ui_SettingsAreaHeight,
                    position.width - 2 * ui_DefaultMargin, ui_SettingsAreaHeight);
            }
        }


        UPRTools()
        {
            titleContent = new GUIContent("UPRTools");
            EditorApplication.playModeStateChanged += ParticleSystemProfilerQuitHandler;
        }

        [MenuItem("Tools/UPRTools")]
        static void showWindow()
        {
            EditorWindow.GetWindow(typeof(UPRTools));
        }


        private void OnEnable()
        {
            if (particleSystemElements == null || particleSystemPrefabs == null)
            {
                particleSystemElements = new List<ParticleSystemElement>();
            }

            projectId = Utils.GetProjectId();
            if (string.IsNullOrEmpty(UPRToolSetting.Instance.customizedServer))
            {
                UPRToolSetting.Instance.Reset();
            }

            m_CustomizedServer = UPRToolSetting.Instance.customizedServer;
            profileResultPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "ProfileResults.json";
            var guids = AssetDatabase.FindAssets("UPRParticlePlay t:Scene");
            if (guids.Length > 0)
            {
                particlePlayScene = AssetDatabase.GUIDToAssetPath(guids[0]);
            }

            InitIfNeeded();
        }

        void InitIfNeeded()
        {
            if (!m_Initialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                bool firstInit = m_MultiColumnHeaderState == null;
                var headerState =
                    MultiColumnTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;
                var multiColumnHeader = new MultiColumnHeader(headerState);
                multiColumnHeader.canSort = false;
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<ParticleSystemElement>(particleSystemElements);
                m_TreeView = new MultiColumnTreeView(m_TreeViewState, multiColumnHeader, treeModel);

                m_Initialized = true;
            }
        }


        [System.Obsolete]
        private void OnGUI()
        {
            string version = Application.unityVersion;
            GUILayout.BeginVertical();

            // draw the title
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("UPRTools");

            // draw the version
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.alignment = TextAnchor.LowerCenter;
            GUILayout.Label("Current Version: " + Utils.PackageVersion);

            //draw the text
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;

            doPackage();
            GUILayout.EndVertical();
        }

        private void SetAllParticleSystemEnabled(bool enable)
        {
            if (particleSystemElements != null)
            {
                foreach (var ele in particleSystemElements)
                {
                    ele.enabled = enable;
                }
            }
        }

        private void StartProfileParticleSystems(int internalIndex = 0)
        {
            UPRToolSetting.Instance.enableLuaProfiler = false;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(particlePlayScene);
            if (internalIndex == 0)
            {
                var selectedElements = new List<ParticleSystemElement>();
                selectedElements.Add(particleSystemElements[0]);
                if (selectedPrefabs == null)
                    selectedPrefabs = new List<ParticleSystemPrefab>();
                selectedPrefabs.Clear();
                for (var i = 1; i < particleSystemElements.Count; i++)
                {
                    if (particleSystemElements[i].enabled)
                    {
                        selectedElements.Add(particleSystemElements[i]);
                        selectedPrefabs.Add(particleSystemPrefabs[particleSystemElements[i].index-1]);
                    }
                }

                particleSystemElements.Clear();
                particleSystemElements.AddRange(selectedElements);
                if (selectedPrefabs.Count > 0)
                {
                    Debug.Log(selectedPrefabs.Count + " particle system assets selected for profiling.");
                }
                else
                {
                    Debug.Log("No particle system selected for profiling.");
                    return;
                }
                if (File.Exists(profileResultPath))
                {
                    File.Delete(profileResultPath);
                }
                File.Create(profileResultPath);
            }
            else
            {
                Debug.Log("Continue play particle systems from position: " + internalIndex);
            }
            
            psProfilerRunning = true;
            profilingDone = false;
            var profilerManagerObj  = GameObject.Find("ProfilerManager");
            psProfileManager = profilerManagerObj.GetComponent<ProfileManager>();
            psProfileManager.m_PrefabList = selectedPrefabs;
            psProfileManager.m_ResultsPersistencePath = profileResultPath;
            psProfileManager.m_PlayingIndex = internalIndex;
            EditorApplication.isPlaying = true;
        }

        private void DetectParticleSystems()
        {
            profilingDone = false;
            particleSystemPrefabs = new List<ParticleSystemPrefab>();
            particleSystemElements = new List<ParticleSystemElement>
            {
                new ParticleSystemElement("", "", -1, 0)
            };
            var idCounter = 0;
            var allGameObjects = AssetDatabase.FindAssets("t:GameObject");
            foreach (var guid in allGameObjects)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (gameObj.GetComponentsInChildren<ParticleSystem>().Length == 0) continue;
                idCounter++;
                var particleSystemEle = new ParticleSystemElement(path, "", 0, idCounter);
                particleSystemElements.Add(particleSystemEle);
                particleSystemPrefabs.Add(new ParticleSystemPrefab
                {
                    duration = particleSystemEle.duration, path = path, prefab = gameObj
                });
            }

            Debug.Log("Detected Particle Systems Count: " + particleSystemPrefabs.Count);
            m_Initialized = false;
            InitIfNeeded();
        }
        
        private void ParticleSystemProfilerQuitHandler(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                GC.Collect();
                psPlayIndex = ProfileResults.GetInstance(profileResultPath).GetIndex() + 1;
                if (psProfilerRunning)
                {
                    if (psPlayIndex == selectedPrefabs.Count)
                    {
                        Debug.Log("Particle systems profiling ended with succeed.");
                    }
                    else
                    {
                        Debug.Log("Particle systems profiling paused on: " + psPlayIndex + ". (Current profiling results have been saved)");
                    }
                    
                    psProfilerRunning = false;
                    profilingDone = true;
                }
            }
        }

        void doPackage()
        {
            var setting = UPRToolSetting.Instance;
            var luaSetting = LuaDeepProfilerSetting.Instance;
            #region  settings

            GUILayout.Space(10);
            GUILayout.Label("Settings");
            GUILayout.BeginVertical("Box");
            GUILayout.Space(ui_DefaultMargin);
            GUILayout.BeginHorizontal();
            GUILayout.Label("ProjectId");
            projectId = GUILayout.TextField(projectId, GUILayout.Width(position.width - 70));
            if (UPRToolSetting.Instance.projectId != projectId)
                UPRToolSetting.Instance.projectId = projectId;
            GUILayout.EndHorizontal();

            #region advanced settings

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            m_AdvancedSettings = EditorGUILayout.Foldout(m_AdvancedSettings, "Advanced Settings");
            if (m_AdvancedSettings)
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reset"))
                {
                    UPRToolSetting.Instance.Reset();
                    m_CustomizedServer = UPRToolSetting.Instance.customizedServer;
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(ui_DefaultMargin);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Custom Server");
                m_CustomizedServer = GUILayout.TextField(m_CustomizedServer, GUILayout.Width(position.width - 120));
                if (!string.IsNullOrEmpty(m_CustomizedServer))
                {
                    UPRToolSetting.Instance.customizedServer = m_CustomizedServer;
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }

            GUILayout.EndHorizontal();

            #endregion

            GUILayout.EndVertical();

            #endregion

            #region deep function profiler

            GUILayout.Space(10);
            GUILayout.Label("Deep Function Profiler(Mono or il2cpp with Editor 2020.3 or lower)");
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            setting.loadScene = GUILayout.Toggle(setting.loadScene, "LoadScene",
                GUILayout.Width(position.width / 2 - ui_DefaultMargin));

            setting.loadAsset = GUILayout.Toggle(setting.loadAsset, "LoadAsset");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            setting.loadAssetBundle = GUILayout.Toggle(setting.loadAssetBundle, "LoadAssetBundle",
                GUILayout.Width(position.width / 2 - ui_DefaultMargin));
            setting.instantiate = GUILayout.Toggle(setting.instantiate, "Instantiate");

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            #endregion

            #region  deep lua profiler

            GUILayout.Space(10);
            GUILayout.Label("Deep Lua Profiler");
            GUILayout.BeginVertical("Box");

            setting.enableMonoProfiler = GUILayout.Toggle(setting.enableMonoProfiler, "Enable Deep Mono");
            if (setting.enableMonoProfiler)
            {
                InjectMethods.Recompile("MIKU_RECOMPILE", true);
                luaSetting.isDeepMonoProfiler = true;
                EditorGUILayout.HelpBox(
                    "Package will inject the necessary code to collect Mono information, which will cause some performance loss.",
                    MessageType.Warning);
                EditorGUILayout.HelpBox("This function doesn't support ios devices currently.", MessageType.Warning);
                if (!uprLuaProfiler)
                {
                    uprLuaProfiler = true;
                    InjectMethods.Recompile("UPR_LUA_PROFILER", uprLuaProfiler);
                }
            }
            else
            {
                luaSetting.isDeepMonoProfiler = false;
                InjectMethods.Recompile("MIKU_RECOMPILE", false);
                if (!setting.enableLuaProfiler && uprLuaProfiler)
                {
                    uprLuaProfiler = false;
                    InjectMethods.Recompile("UPR_LUA_PROFILER", uprLuaProfiler);
                }
            }

            setting.enableLuaProfiler = GUILayout.Toggle(setting.enableLuaProfiler, "Enable Lua");
            if (setting.enableLuaProfiler)
            {
                luaSetting.isDeepLuaProfiler = true;
                EditorGUILayout.HelpBox(
                    "Package will load some simple lua function to collect lua information. You can easy control whether to send data on UPR website. If you choose il2cpp to build, it will auto allow unsafe code",
                    MessageType.Warning);
                if (!useLua)
                {
                    string[] dir =
                        Directory.GetDirectories(Application.dataPath, "*LuaProfiler*", SearchOption.TopDirectoryOnly);

                    if (dir.Length > 0)
                    {
                        useLua = false;
                        EditorGUILayout.HelpBox("LuaProfiler is Find in the directory", MessageType.Warning);
                    }
                    else
                    {
                        useLua = true;
                    }

                    InjectMethods.Recompile("USE_LUA", useLua);
                }

                if (!uprLuaProfiler)
                {
                    uprLuaProfiler = true;
                    InjectMethods.Recompile("UPR_LUA_PROFILER", uprLuaProfiler);
                }

                if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) ==
                    ScriptingImplementation.IL2CPP)
                {
                    PlayerSettings.allowUnsafeCode = true;
                }
            }
            else
            {
                luaSetting.isDeepLuaProfiler = false;
                if (useLua)
                {
                    useLua = false;
                    InjectMethods.Recompile("USE_LUA", false);
                }

                if (!setting.enableMonoProfiler && uprLuaProfiler)
                {
                    uprLuaProfiler = false;
                    InjectMethods.Recompile("UPR_LUA_PROFILER", uprLuaProfiler);
                }

                PlayerSettings.allowUnsafeCode = false;
            }

            GUILayout.EndVertical();

            #endregion

#if UNITY_2018_2_OR_NEWER

            #region Universal RP 
            GUILayout.Space(10);
            GUILayout.Label("Enable Universal RP overdraw");
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            setting.enableURPOverdraw = GUILayout.Toggle(setting.enableURPOverdraw, "Enable Overdraw Calculation in Universal RP(version 7.4.3 or above)");
            GUILayout.EndHorizontal();
            InjectMethods.Recompile("URP", setting.enableURPOverdraw);
            GUILayout.EndVertical();
            #endregion
            
            
            #region shader variants check region

            GUILayout.Space(10);
            GUILayout.Label("Shader Variants Report");
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            setting.enableShaderVariantsCheck =
                GUILayout.Toggle(setting.enableShaderVariantsCheck, "Enable Shader Variants Report");

            GUILayout.EndHorizontal();
            if (setting.enableShaderVariantsCheck)
            {
                if (string.IsNullOrEmpty(UPRToolSetting.Instance.projectId))
                {
                    EditorGUILayout.HelpBox(
                        "ProjectId not specified, you could manually upload ShaderVariantsReport file on UPR website after build.",
                        MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "ShaderVariantsReport will be automatically uploaded to UPR server after build.",
                        MessageType.Info);
                }
            }

            GUILayout.EndVertical();

            #endregion
#endif

            #region particle system profiler

            GUILayout.Space(2 * ui_DefaultMargin);
            m_ParticleSystem = EditorGUILayout.Foldout(m_ParticleSystem, "Particle System Profiler");
            if (m_ParticleSystem)
            {
                GUILayout.BeginVertical("Box");

                
                GUILayout.BeginHorizontal();
                GUI.enabled = !psProfilerRunning;
                if (GUILayout.Button(" Detect ", GUILayout.Height(25)))
                {
                    DetectParticleSystems();
                }
                GUI.enabled = true;
                if (!psProfilerRunning)
                {
                    if (GUILayout.Button("  Start  ", GUILayout.Height(25)))
                    {
                        StartProfileParticleSystems();
                    }
                }
                else
                {
                    if (GUILayout.Button("  Pause ", GUILayout.Height(25)))
                    {
                        EditorApplication.isPlaying = false;
                    }
                }
                GUI.enabled = !psProfilerRunning && profilingDone;
                if (GUILayout.Button("Continue", GUILayout.Height(25)))
                {
                    StartProfileParticleSystems(psPlayIndex);
                }
                GUILayout.EndHorizontal();

                GUI.enabled = !psProfilerRunning;
                m_TreeView.OnGUI(multiColumnTreeViewRect);
                GUI.enabled = true;

                GUILayout.BeginArea(uploadResultViewRect);
                GUILayout.Space(ui_DefaultMargin * 0.8f);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("All", GUILayout.Height(20), GUILayout.MaxWidth(70)))
                {
                    SetAllParticleSystemEnabled(true);
                }
                if (GUILayout.Button("None", GUILayout.Height(20), GUILayout.MaxWidth(70)))
                {
                    SetAllParticleSystemEnabled(false);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2 * ui_DefaultMargin);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Result Path");
                GUILayout.TextArea(profileResultPath, GUILayout.Width(position.width - 90), GUILayout.Height(30));
                GUILayout.EndHorizontal();
                GUI.enabled = profilingDone;
                GUILayout.Space(ui_DefaultMargin);

                var uploadBtn = GUILayout.Button("Upload Result", GUILayout.Height(25));
                GUILayout.Space(ui_DefaultMargin);
                GUILayout.EndArea();
                if (uploadBtn)
                {
                    Debug.Log("Particle System Profile Result: " + profileResultPath);
                    Debug.Log("UPR ProjectId: " + UPRToolSetting.Instance.projectId);
                    ProfileResults.GetInstance(profileResultPath).UploadResults();
                }

                GUI.enabled = true;
                GUILayout.EndVertical();
            }

            #endregion
        }
    }
#endif
}