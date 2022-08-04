using System;
using UnityEngine;

namespace UPRProfiler
{
    public class PlayController : MonoBehaviour
    {
        private ParticleSystem[] m_ParticleSystems;
        public bool              m_Playing;

        private void Start()
        {
            m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
            m_Playing = true;
        }

        private void LateUpdate()
        {
            m_Playing = false;
            foreach (var ps in m_ParticleSystems)
            {
                if (ps == null)
                {
                    continue;
                }
                if (ps.isPlaying)
                {
                    m_Playing = true;
                    break;
                }
            }
        }
    }
}