#if UNITY_2018_2_OR_NEWER
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UPRProfiler.Common;
using Object = System.Object;

namespace UPRProfiler.OverdrawMonitor
{
    public interface IOverCamera {
        Camera targetCamera { get; set; }
    }
    
    public class UPROverdrawMonitor : MonoBehaviour
    {
        private static UPROverdrawMonitor instance;

        public static bool Enabled = false;
        public static bool Cleaned = true;
        
        public static bool NotSupportedPlatform = false;
        public static string NotSupportedMessageGroupName;

        public static bool NotSupportedFlagSent = false;
        private static bool IsURP = false;
        private static string notSupportedGroupName = "overdrawNotSupported";

        public static UPROverdrawMonitor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UPROverdrawMonitor>();
                    if (instance == null)
                    {
                        if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
                        {
                            Debug.Log("Current platform dose not support render texture format ARGBFloat");
                            NotSupportedMessageGroupName = "ARGBFloatNotSupported";
                            NotSupportedPlatform = true;
                        } else if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
                        {
                            Debug.Log("Current platform dose not support Texture format RGBAFloat");
                            NotSupportedMessageGroupName = "RGBFloatNotSupported";
                            NotSupportedPlatform = true;
                        } else if (!SystemInfo.supportsComputeShaders)
                        {
                            Debug.Log("Current platform dose not support compute shaders");
                            NotSupportedMessageGroupName = "computeShadersNotSupported";
                            NotSupportedPlatform = true;
                        } 

                        var go = new GameObject("UPROverdrawMonitor");
                        DontDestroyOnLoad(go);
                        go.hideFlags = HideFlags.HideAndDontSave;
                        instance = go.AddComponent<UPROverdrawMonitor>();
                        instance._monitorsGo = go;
                        
                        //for urp
                        if (GraphicsSettings.renderPipelineAsset!=null && GraphicsSettings.renderPipelineAsset.GetType().Name == "UniversalRenderPipelineAsset")
                        {
                            IsURP = true;
                            // instance._monitorsGo.AddComponent<URPCameraOverdrawMonitor>();
                        }
                    }
                }

                return instance;
            }
        }

        private GameObject _monitorsGo;

        void Update()
        {
            if (!Enabled)
            {
                if (!Cleaned)
                {
                    Clean();
                    Cleaned = true;
                }
                return;
            }
            
            if (NotSupportedFlagSent)
            {
                return;
            }

            if (NotSupportedPlatform)
            {
                var toSent = new Dictionary<string, string>();
                toSent["supported"] = "-1";
                if(UPROpen.SendCustomizedData(UPRCameraOverdrawMonitor.customDataSubjectName, NotSupportedMessageGroupName,
                    "line", toSent))
                {
                    NotSupportedFlagSent = true;
                }
                
                Clean();
                return;
            }

            Camera[] activeCameras = Camera.allCameras;

            var monitors = GetAllMonitors();
            foreach (var monitor in monitors)
            {
                if (!Array.Exists(activeCameras, c => monitor.targetCamera == c))
                {
                    DestroyImmediate((UnityEngine.Object) monitor);
                }
                
            }
            

            monitors = GetAllMonitors();
            foreach (Camera activeCamera in activeCameras)
            {
                if (!Array.Exists(monitors, m => m.targetCamera == activeCamera))
                {
                    #if URP
                    if (IsURP)
                    {
                        var monitor = _monitorsGo.AddComponent<URPCameraOverdrawMonitor>();
                        monitor.SetTargetCamera(activeCamera);
                    }
                    else
                    {
                        var monitor = _monitorsGo.AddComponent<UPRCameraOverdrawMonitor>();
                        monitor.SetTargetCamera(activeCamera);
                    }
                    #else
                        var monitor = _monitorsGo.AddComponent<UPRCameraOverdrawMonitor>();
                        monitor.SetTargetCamera(activeCamera);
                    #endif
                    
                }
            }
            
        }

        IOverCamera[] GetAllMonitors()
        {
            #if URP
            if (IsURP)
                return _monitorsGo.GetComponentsInChildren<URPCameraOverdrawMonitor>(true);
            #endif
            return _monitorsGo.GetComponentsInChildren<UPRCameraOverdrawMonitor>(true);
        }

        void Clean()
        {
            var monitors = GetAllMonitors();
            foreach (UnityEngine.Object monitor in monitors)
                DestroyImmediate(monitor);
        }
        
    }
}
#endif