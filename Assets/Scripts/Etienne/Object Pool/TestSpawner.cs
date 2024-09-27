using System.Collections.Generic;
using UnityEngine;
using SpaceBaboon.PoolingSystem;
using SpaceBaboon.WeaponSystem;

namespace SpaceBaboon
{
    public class TestSpawner : MonoBehaviour
    {
        [SerializeField] private GenericObjectPool m_pool = new GenericObjectPool();
        [SerializeField] private GameObject m_prefab1;
        [SerializeField] private GameObject m_prefab2;
        [SerializeField] private GameObject m_prefabToSpawn;

        [SerializeField] private float m_spawnDistance;
        [SerializeField] private float m_attackSpeed;

        private float m_attackingCooldown = 0.0f;


        private void Awake()
        {
            //m_pool = new GenericObjectPool();


            m_prefabToSpawn = m_prefab1;

            List<GameObject> list = new List<GameObject>();
            list.Add(m_prefab1);
            list.Add(m_prefab2);

            m_pool.CreatePool(list, "test");

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (m_prefabToSpawn == m_prefab1)
                {
                    m_prefabToSpawn = m_prefab2;
                }
                else
                {
                    m_prefabToSpawn = m_prefab1;
                }
            }


            if (m_attackingCooldown > m_attackSpeed)
            {
                Attack();
                m_attackingCooldown = 0.0f;
            }
            m_attackingCooldown += Time.deltaTime;

        }

        private void Attack()
        {
            Vector2 direction = Vector2.up;

            Vector2 directionWithDistance = direction.normalized * m_spawnDistance;
            Vector2 spawnPos = new Vector2(transform.position.x + directionWithDistance.x, transform.position.y + directionWithDistance.y);
            var projectile = m_pool.Spawn(m_prefabToSpawn, spawnPos);
            //Debug.Log("spawning");

            projectile.GetComponent<TestPoolableObject>()?.Shoot(direction);
        }

    }
}
