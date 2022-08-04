using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace UPRProfiler
{
    #if UNITY_EDITOR
    public class ProfileManager : MonoBehaviour
    {
        public  List<ParticleSystemPrefab> m_PrefabList;
        public  int                        m_PlayingIndex;
        public  string                     m_ResultsPersistencePath;
        
        // For Whole Profile Process
        private MethodInfo     m_CalculateEffectUIDataMethod;
        private MethodInfo     m_GetTextureMemorySizeMethod;
        private ProfileResults m_PersistenceProfileResults;
        private float          minFov      = 0.01f;
        private float          maxFov      = 1000f;
        private float          sensitivity = 5f;
        
        // For Single Prefab
        private Dictionary<string, int>   m_textureDict;
        private ParticleSystem[]          m_ParticleSystems;
        private ParticleSystemCheckResult m_CurrentResult;
        private float                     m_CurrentStartTime;
        private float                     m_PrevFrameTime;
        private PlayController            m_PlayController;
        private GameObject                m_PlayingObj;
        
        private void Awake()
        {
            m_textureDict = new Dictionary<string, int>();
            m_PlayingIndex -= 1;
            m_PersistenceProfileResults = ProfileResults.GetInstance(m_ResultsPersistencePath);
            m_CalculateEffectUIDataMethod = typeof(ParticleSystem).GetMethod("CalculateEffectUIData",
                BindingFlags.Instance | BindingFlags.NonPublic);
            m_GetTextureMemorySizeMethod = typeof(AssetDatabase).Assembly.GetType("UnityEditor.TextureUtil")
                .GetMethod("GetStorageMemorySize", BindingFlags.Public | BindingFlags.Static);
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Application quit: " + m_PlayingIndex);
            m_PersistenceProfileResults.Save();
        }

        private void PlayNextPrefab()
        {
            m_PlayingIndex += 1;
            if (m_PrefabList.Count <= m_PlayingIndex)
            {
                EditorApplication.isPlaying = false;
                return;
            }
            m_PersistenceProfileResults.UpdateIndex(m_PlayingIndex);
            m_CurrentResult = new ParticleSystemCheckResult();
            m_CurrentResult.ParticleSystemPath = m_PrefabList[m_PlayingIndex].path;

            m_PlayingObj = Instantiate(m_PrefabList[m_PlayingIndex].prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            m_CurrentStartTime = Time.realtimeSinceStartup;
            m_PrevFrameTime = m_CurrentStartTime;
            m_ParticleSystems = m_PlayingObj.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in m_ParticleSystems)
            {
                ps.loop = false;
            }
            m_PlayController = m_PlayingObj.AddComponent<PlayController>();
            m_textureDict.Clear();
        }
        
        
        private void Start()
        {
            PlayNextPrefab();
        }

        private void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            var curFov = Camera.main.orthographicSize;
            Camera.main.orthographicSize = Mathf.Clamp(curFov + scroll * sensitivity, minFov, maxFov);
        }

        void LateUpdate()
        {
            if (Time.realtimeSinceStartup - m_CurrentStartTime > Math.Min(15.0, m_PrefabList[m_PlayingIndex].duration + 5.0)
                || !m_PlayController.m_Playing)
            {
                Destroy(m_PlayingObj);
                Debug.Log($"Play done: {m_PrefabList[m_PlayingIndex].path} ({m_PlayingIndex+1}/{m_PrefabList.Count})");
                m_PersistenceProfileResults.Append(m_CurrentResult);
                PlayNextPrefab();
            }
            else
            {
                try
                {
                    RecordDrawCall();
                    RecordParticleCount();
                    RecordFrameTime();
                    RecordMemoryUsage();
                    m_PrevFrameTime = Time.realtimeSinceStartup;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Catch Exception when play: {m_PrefabList[m_PlayingIndex].path} ({m_PlayingIndex+1}/{m_PrefabList.Count}");
                    Debug.LogWarning(e.Message);
                }
                
            }
        }

        private void RecordParticleCount()
        {
            var frameCount = 0;
            foreach (var ps in m_ParticleSystems)
            {
                var count = 0;
                object[] invokeArgs = {count, 0.0f, Mathf.Infinity};
                try
                {
                    m_CalculateEffectUIDataMethod.Invoke(ps, invokeArgs);
                    count = (int) invokeArgs[0];
                }
                catch (Exception)
                {
                    count = 0;
                }
               
                frameCount += count;
            }
            m_CurrentResult.ParticleCounts.Add(frameCount);
        }
        

        private void RecordDrawCall()
        {
            m_CurrentResult.DrawCalls.Add(UnityStats.batches / 2);
        }

        private void RecordFrameTime()
        {
            m_CurrentResult.FrameTimes.Add(Time.realtimeSinceStartup - m_PrevFrameTime);
        }

        private void RecordMemoryUsage()
        {
            int sumSize = 0;
            var rendererList = m_PlayingObj.GetComponentsInChildren<ParticleSystemRenderer>(true);

            foreach (var item in rendererList)
            {
                if (item.sharedMaterial)
                {
                    foreach (var name in item.sharedMaterial.GetTexturePropertyNames())
                    {
                        if (m_textureDict.TryGetValue(name, out int memorySize))
                        {
                            sumSize += memorySize;
                        }
                        else
                        {
                            Texture texture = item.sharedMaterial.GetTexture(name);
                            if (texture != null)
                            {
                                memorySize = GetTextureMemorySize(texture);
                                m_textureDict[name] = memorySize;
                                sumSize += memorySize;
                            }
                        }
                    }
                }
            }
            m_CurrentResult.MemoryUsages.Add(sumSize);
        }

        private int GetTextureMemorySize(Texture texture)
        {
            object[] invokeArgs = {texture};
            return (int)m_GetTextureMemorySizeMethod.Invoke(null, invokeArgs);
        }
    }
    #endif
}
