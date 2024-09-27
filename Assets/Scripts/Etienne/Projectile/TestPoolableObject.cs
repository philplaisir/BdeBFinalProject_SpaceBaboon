using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceBaboon.PoolingSystem;

namespace SpaceBaboon.WeaponSystem
{
    public class TestPoolableObject : MonoBehaviour, IPoolableGeneric
    {
        [SerializeField] private ProjectileData m_projectileData;

        private Vector2 m_direction;
        private float m_lifetime = 0.0f;

        //For ObjectPool
        private bool m_isActive = false;
        SpriteRenderer m_renderer;
        CircleCollider2D m_collider;
        GenericObjectPool m_parentPool;


        public bool IsActive
        {
            get { return m_isActive; }
        }


        private void Awake()
        {
            m_renderer = GetComponent<SpriteRenderer>();
            m_collider = GetComponent<CircleCollider2D>();
        }

        private void Update()
        {
            if (!m_isActive)
            {
                return;
            }
            

            if (m_lifetime > m_projectileData.maxLifetime)
            {
                m_parentPool.UnSpawn(gameObject);
                //Debug.Log("UnSpawning (lifetime)");

            }
            m_lifetime += Time.deltaTime;

            transform.Translate(m_direction * m_projectileData.speed * Time.deltaTime);
        }

        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    m_parentPool.UnSpawn(gameObject);
        //    //Debug.Log("projectile hit: " + collision.gameObject.name);
        //}

        public void Shoot(Vector2 direction)
        {
            m_direction = direction.normalized;
        }

        public void Activate(Vector2 pos, GenericObjectPool pool)
        {
            ResetValues(pos);
            SetComponents(true);
        
            m_parentPool = pool;
        }


        public void Deactivate()
        {
            SetComponents(false);
        }

        private void ResetValues(Vector2 pos)
        {
            m_lifetime = 0.0f;
            transform.position = pos;
        }
        private void SetComponents(bool value)
        {
            m_isActive = value;
            
            m_renderer.enabled = value;
            m_collider.enabled = value;
        }


    }
}
