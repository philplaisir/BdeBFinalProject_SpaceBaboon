using SpaceBaboon.PoolingSystem;
using UnityEngine;

namespace SpaceBaboon
{
    public class VFXInstance : MonoBehaviour, IPoolableGeneric
    {
        private GenericObjectPool m_parentPool;

        private bool m_isActive;
        public bool IsActive { get { return m_isActive; } }

        [SerializeField] private float m_duration;
        private float m_timer;

        void Update()
        {
            if (!m_isActive)
            {
                return;
            }

            HandleTimer();
        }

        private void HandleTimer()
        {
            if (m_timer > 0)
            {
                m_timer -= Time.deltaTime;

                if (m_timer <= 0)
                {
                    m_parentPool.UnSpawn(gameObject);
                }
            }
        }

        public void Activate(Vector2 pos, GenericObjectPool pool)
        {
            m_isActive = true;
            m_parentPool = pool;
            m_timer = m_duration;
            transform.position = pos;

            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            m_isActive = false;

            gameObject.SetActive(false);
        }
    }
}
