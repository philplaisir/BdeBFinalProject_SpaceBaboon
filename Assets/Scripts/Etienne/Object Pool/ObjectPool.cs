using SpaceBaboon.PoolingSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon
{
    public class ObjectPool
    {
        [SerializeField] private GameObject m_prefab;
        [SerializeField] private GameObject m_container;
        [SerializeField] private int m_poolSize;

        [SerializeField] private Queue<GameObject> m_pooledObjects = new Queue<GameObject>();


        public void CreatePool(GameObject prefab)
        {
            m_prefab = prefab;
            m_container = new GameObject();
            m_container.name = prefab.name + " pool";

            if (m_poolSize <= 0)
            {
                Debug.LogError("Invalid pool size");
                m_poolSize = 10;
            }

            for (int i = 0; i < m_poolSize; i++)
            {
                GameObject obj = GameObject.Instantiate(m_prefab, m_container.transform);

                obj.GetComponent<IPoolable>()?.Deactivate();
                m_pooledObjects.Enqueue(obj);

                //Debug.Log("Awake : " + m_pooledObjects.Count);
            }
        }

        public GameObject Spawn(Vector2 pos)
        {
            if (m_pooledObjects.Count != 0)
            {
                var obj = m_pooledObjects.Dequeue();

                var pooledObj = obj.GetComponent<IPoolable>();
                pooledObj.Activate(pos, this);

                return obj;
            }

            //If pool is empty 
            GameObject newObj = GameObject.Instantiate(m_prefab, m_container.transform);
            newObj.GetComponent<IPoolable>()?.Activate(pos, this);
            //Debug.Log("activating new : " + pooledObjects.Count);

            return newObj;
        }

        public void UnSpawn(GameObject obj)
        {
            var pooledObject = obj.GetComponent<IPoolable>();
            if (pooledObject == null)
            {
                Debug.LogError(obj.name + "is not poolable");
                return;
            }
            pooledObject.Deactivate();

            m_pooledObjects.Enqueue(obj);
        }

        public void SetPoolSize(int poolSize)
        {
            m_poolSize = poolSize;
        }

        public int GetPoolQueueSize()
        {
            return m_pooledObjects.Count;
        }
    }
}
