#if URP && UNITY_2018_2_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UPRProfiler.Common;
using UnityEngine.Rendering.Universal;
namespace UPRProfiler.OverdrawMonitor
{
    public class URPCameraOverdrawMonitor : MonoBehaviour, IOverCamera
    {
        // Start is called before the first frame update
        private int DefaultRendererIndex = 0;
        private int OverdrawRendererIndex = 0;
        private bool enable = true;
        private bool initialized = false;
        public static bool EnableOverdrawScreenshot = true;
        private UniversalAdditionalCameraData cameraData;
        private ScriptableRendererData overdrawRendererData;
        private ScriptableRendererData[] rendererDataList;
        private Camera _targetCamera;
        private String _cameraName;
        public static int MonitorFrequency = 30;
        int _index;
        RenderTexture _overdrawTexture;
        Texture2D _readingTexture;
        Rect _readingRect;
        int _calBreakSize;
        bool _allBlack;
        int _allBlackFrameCount;
        static float oneDrawTimeG = 0.04f;
        public static string customDataSubjectName = "UPROverdraw";
        public static string customDataGroupName = "OverdrawRate";
        Dictionary<string, string> _toSendData = new Dictionary<string, string>(1);
        public Camera targetCamera
        {
            get => _targetCamera;
            set => _targetCamera = value;
        }
        public void SetTargetCamera(Camera targetCamera)
        {
            _targetCamera = targetCamera;
            _cameraName = targetCamera.name;
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }
        }
        void Initialize()
        {
            _index = MonitorFrequency - 1;
            cameraData = targetCamera.GetComponent<UniversalAdditionalCameraData>();
            DefaultRendererIndex = GetDefaultRendererIndex(GraphicsSettings.currentRenderPipeline);

#if UNITY_EDITOR
            overdrawRendererData =
                (ScriptableRendererData) AssetDatabase.LoadAssetAtPath(
                    "Assets/UPRTools/Resources/OverdrawRenderer.asset", typeof(ScriptableRendererData));
#else
            overdrawRendererData = Resources.Load<ScriptableRendererData>("OverdrawRenderer");
            // overdrawRendererData = Resources.LoadAll("OverdrawRenderer")[0] as ScriptableRendererData;
#endif
            if (overdrawRendererData == null)
            {
                enable = false;
                return;
            }
            var proInfo =
                typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            if (proInfo != null)
            {
                rendererDataList = (ScriptableRendererData[])proInfo.GetValue(UniversalRenderPipeline.asset);
                if (rendererDataList[rendererDataList.Length - 1].name == "OverdrawRenderer")
                {
                    OverdrawRendererIndex = rendererDataList.Length - 1;
                }
                else
                {
                    var newList = new ScriptableRendererData[rendererDataList.Length+1];
                    for (int i = 0; i < rendererDataList.Length; i++)
                    {
                
                        newList[i] = rendererDataList[i];
                        newList[rendererDataList.Length] = overdrawRendererData;
                    }
                    proInfo.SetValue(GraphicsSettings.currentRenderPipeline, newList);
                    OverdrawRendererIndex = rendererDataList.Length;
                }
            
            }
            // cameraData.SetRenderer(OverdrawRendererIndex);
        }
        private int GetDefaultRendererIndex(RenderPipelineAsset asset)
        {
            var proInfo = typeof(UniversalRenderPipelineAsset).GetField("m_DefaultRendererIndex",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if ( proInfo != null)
            {
                return (int) proInfo.GetValue(asset);
            }

            return 0;
        }
        
        

        void InitTexture()
        {
            int overDrawRTWidth, overDrawRTHeight;
            if (_targetCamera.pixelHeight > 480)
            {
                overDrawRTWidth =
                    Mathf.FloorToInt(480.0f * _targetCamera.pixelWidth / _targetCamera.pixelHeight);
                overDrawRTHeight = 480;
            }
            else
            {
                overDrawRTWidth = _targetCamera.pixelWidth;
                overDrawRTHeight = _targetCamera.pixelHeight;
            }

            _overdrawTexture = new RenderTexture(overDrawRTWidth, overDrawRTHeight, 0, RenderTextureFormat.ARGBFloat);
            _readingTexture = new Texture2D(overDrawRTWidth, overDrawRTHeight, TextureFormat.RGBAFloat, false);
            _readingRect = new Rect(0, 0, _overdrawTexture.width, _overdrawTexture.height);
            _calBreakSize = overDrawRTWidth * overDrawRTHeight / (MonitorFrequency - 2);
            _toSendData[_cameraName] = "0";
            UPROpen.SendCustomizedData(customDataSubjectName, customDataGroupName, "line", _toSendData);
        }

        void ReleaseTexture()
        {
            if (_overdrawTexture != null)
            {
                _overdrawTexture.Release();
                _overdrawTexture = null;
                Destroy(_readingTexture);
                _readingTexture = null;
                _readingRect = Rect.zero;
            }
        }
        void OnDestroy()
        {
            _toSendData[_cameraName] = "0";
            UPROpen.SendCustomizedData(customDataSubjectName, customDataGroupName, "line", _toSendData);
            ReleaseTexture();
        }
    
        private void LateUpdate()
        {
            
            _index++;
            if (_index == MonitorFrequency)
            {
                _index = 0;
            }
            else
            {
                return;
            }

            if (_targetCamera == null)
            {
                return;
            }
            Profiler.BeginSample("Profiler.UPRCameraOverdrawMonitor");
            CameraClearFlags originalClearFlags = _targetCamera.clearFlags;
            Color originalClearColor = _targetCamera.backgroundColor;
            RenderTexture originalTargetTexture = _targetCamera.targetTexture;
            bool originalIsCameraEnabled = _targetCamera.enabled;

            if (_overdrawTexture == null)
            {
                ReleaseTexture();
                InitTexture();
            }

            _targetCamera.clearFlags = CameraClearFlags.SolidColor;
            _targetCamera.backgroundColor = Color.black;
            _targetCamera.targetTexture = _overdrawTexture;
            _targetCamera.enabled = false;

            cameraData.SetRenderer(OverdrawRendererIndex);
            
            _targetCamera.Render();
            
            RenderTexture.active = _overdrawTexture;
            _readingTexture.ReadPixels(_readingRect, 0, 0);
            _readingTexture.Apply();
            
            // var jpg = _readingTexture.EncodeToJPG();
            // System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/"+_index+".jpg", jpg);
            RenderTexture.active = null;

            StartCoroutine(CalculateOverdraw());

            _targetCamera.targetTexture = originalTargetTexture;
            _targetCamera.clearFlags = originalClearFlags;
            _targetCamera.backgroundColor = originalClearColor;
            _targetCamera.enabled = originalIsCameraEnabled;
            
            cameraData.SetRenderer(default);
            Profiler.EndSample();
        }

        IEnumerator CalculateOverdraw()
        {
            _allBlack = true;
            yield return null;
            Profiler.BeginSample("Profiler.UPROverdrawMonitorCal");
            var overdrawColors = _readingTexture.GetRawTextureData<Color>();
            int totalSize = overdrawColors.Length;
            var breakPoint = _calBreakSize;
            float drawTimesInG = 0f;
            for (var i = 0; i < totalSize; i++)
            {
                if (overdrawColors[i].g <= oneDrawTimeG)
                {
                    drawTimesInG += oneDrawTimeG;
                }
                else
                {
                    if (_allBlack)
                    {
                        _allBlack = false;
                    }

                    drawTimesInG += overdrawColors[i].g;
                }

                if (i == breakPoint)
                {
                    breakPoint += _calBreakSize;
                    Profiler.EndSample();
                    yield return null;
                    Profiler.BeginSample("Profiler.UPROverdrawMonitorCal");
                }
            }

            if (_allBlack)
            {
                _allBlackFrameCount++;
                // if (_allBlackFrameCount > 3)
                // {
                //     UPROverdrawMonitor.NotSupportedPlatform = true;
                //     UPROverdrawMonitor.NotSupportedMessageGroupName = "allBlack";
                // }
            }
            else
            {
                var overdrawRate = Convert.ToInt32(drawTimesInG / oneDrawTimeG) / (float) totalSize;
                _toSendData[_cameraName] = overdrawRate < 1f ? "1" : overdrawRate.ToString();
                UPROpen.SendCustomizedData(customDataSubjectName, customDataGroupName, "line", _toSendData);
                if (EnableOverdrawScreenshot)
                {
                    NetworkServer.SendOverdrawScreenshot(_readingTexture.EncodeToJPG(), _cameraName);
                }
            }
            
            Profiler.EndSample();
        }
    }
}
#endif