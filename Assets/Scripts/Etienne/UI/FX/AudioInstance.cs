using SpaceBaboon.PoolingSystem;
using UnityEngine;

namespace SpaceBaboon.FXSystem
{
    public class AudioInstance : MonoBehaviour, IPoolable
    {
        private ObjectPool m_parentPool;
        
        private bool m_isActive;
        public bool IsActive { get { return m_isActive; } }
        
        private AudioSource m_audioSource;
        public AudioSource AudioSource { get { return m_audioSource; } }


        void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
        
        }

        void Update()
        {
            if (!m_isActive)
            {
                return;
            }

            if (!m_audioSource.isPlaying)
            {
                m_parentPool.UnSpawn(gameObject);
                //Debug.Log("audio unspawning");
            }
        
        }

        public void Activate(Vector2 pos, ObjectPool pool)
        {
            m_isActive = true;
            m_parentPool = pool;
            m_audioSource.enabled = true;
        }

        public void Deactivate()
        {
            m_isActive = false;
            m_audioSource.enabled = false;
        }

    }
}
