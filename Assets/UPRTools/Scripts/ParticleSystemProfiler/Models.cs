using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
//using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using Utils = UPRProfiler.Common.Utils;

namespace UPRProfiler
{
    [Serializable]
    [ExecuteAlways]
    public class ProfileResults
    {
        public         ProfileResultsObject m_ResultsObj;
        private static string               m_PersistencePath;
        private static ProfileResults       m_Instance;

        public static ProfileResults GetInstance(string persistencePath)
        {
            if (m_Instance != null && persistencePath == m_PersistencePath)
            {
                return m_Instance;
            }
            return new ProfileResults(persistencePath);
        }

        private ProfileResults(string persistencePath)
        {
            m_PersistencePath = persistencePath;
            m_ResultsObj = ScriptableObject.CreateInstance<ProfileResultsObject>();
            Load();
        }
        
        public void Save()
        {
            string json = JsonUtility.ToJson(m_ResultsObj);
            File.WriteAllText(m_PersistencePath, json);
        }

        private void Load()
        {
            if (File.Exists(m_PersistencePath))
            {
                string json = File.ReadAllText(m_PersistencePath);
                JsonUtility.FromJsonOverwrite(json, m_ResultsObj);
            }
            if (m_ResultsObj.ParticleSystemCheckResults == null)
            {
                m_ResultsObj.ParticleSystemCheckResults = new List<ParticleSystemCheckResult>();
            }
        }

        public void UpdateIndex(int playIndex)
        {
            m_ResultsObj.PlayIndex = playIndex;
        }

        public int GetIndex()
        {
            return m_ResultsObj.PlayIndex;
        }

        public void Append(ParticleSystemCheckResult item)
        {
            try
            {
                Summarize(item);
                m_ResultsObj.ParticleSystemCheckResults.Add(item);
            }
            catch (Exception e)
            {
                Debug.Log("Catch exception when summarizing result: " + e.Message);
            }
            
            
        }

        private static void Summarize(ParticleSystemCheckResult item)
        {
            item.Summary = new Summary();
            item.Summary.PrefabName = item.ParticleSystemPath;
            item.Summary.DrawCallAvg = item.DrawCalls.Average();
            item.Summary.DrawCallMax = item.DrawCalls.Max();
            
            item.Summary.FrameTimeAvg = item.FrameTimes.Average();
            item.Summary.FrameTimeMax = item.FrameTimes.Max();
            
            item.Summary.MemoryUsageAvg = item.MemoryUsages.Average();
            item.Summary.MemoryUsageMax = item.MemoryUsages.Max();
            
            item.Summary.ParticleCountAvg = item.ParticleCounts.Average();
            item.Summary.ParticleCountMax = item.ParticleCounts.Max();
            item.Summary.ParticleCountSum = item.ParticleCounts.Sum();

        }
        
        public List<ParticleSystemCheckResult> GetResults()
        {
            return m_ResultsObj.ParticleSystemCheckResults;
        }

        public void UploadResults()
        {
            m_ResultsObj.ProjectId = UPRToolSetting.Instance.projectId;
            m_ResultsObj.Properties = new ProfileResultsObject.PropertiesField(Utils.PackageVersion);
            var json = JsonUtility.ToJson(m_ResultsObj);
//            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var uploadUrl = Utils.UploadHost + "/particle-system-check";
//            var client = new HttpClient();
            var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            var response = client.UploadString(uploadUrl, json);
//            var response = client.PostAsync(uploadUrl, data);
//            var result = response.Content.ReadAsStringAsync().Result;
            var resp = JsonUtility.FromJson<ResultUploadResp>(response);
            Debug.Log("Browse profile result via UPR Web: "+ Utils.BrowserHost + "/particle-system-check/" + resp.ParticleSystemCheckId);
        }
    }
    
    [Serializable]
    public class ProfileResultsObject : ScriptableObject
    {
        public string                          ProjectId;
        public PropertiesField                 Properties;
        public List<ParticleSystemCheckResult> ParticleSystemCheckResults;
        public int                             PlayIndex;

        [Serializable]
        public class PropertiesField
        {
            public string PackageVersion;

            public PropertiesField(string packageVersion)
            {
                PackageVersion = packageVersion;
            }
        }
    }

    public class ResultUploadResp
    {
        public string ParticleSystemCheckId;
    }
    

    [Serializable]
    public class ParticleSystemCheckResult
    {
        public ParticleSystemCheckResult()
        {
            FrameTimes = new List<float>();
            ParticleCounts = new List<int>();
            MemoryUsages = new List<int>();
            DrawCalls = new List<int>();
        }

        public string ParticleSystemPath;
        public List<float> FrameTimes;
        public List<int> ParticleCounts;
        public List<int> MemoryUsages;
        public List<int> DrawCalls;
        public Summary Summary;
    }

    [Serializable]
    public class Summary
    {
        public string PrefabName;
        public double FrameTimeAvg;
        public double FrameTimeMax;
        public double ParticleCountSum;
        public double ParticleCountAvg;
        public double ParticleCountMax;
        public double MemoryUsageAvg;
        public double MemoryUsageMax;
        public double DrawCallAvg;
        public double DrawCallMax;
    }

    [Serializable]
    public class ParticleSystemPrefab
    {
        public GameObject prefab;
        public float      duration;
        public string     path;
    }
}