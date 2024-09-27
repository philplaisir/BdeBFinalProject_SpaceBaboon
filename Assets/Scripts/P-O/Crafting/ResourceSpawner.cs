using SpaceBaboon.PoolingSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SpaceBaboon
{
    public class ResourceSpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> m_resourcesPrefab;
        [SerializeField] private GameObject m_healingCollectable;
        //[SerializeField] private List<ObjectPool> m_resourcePool;
        //[SerializeField] private GameObject m_map;
        //[SerializeField] private float m_spawnRadiusFromScreenCorner = 0.0f;
        [SerializeField] private float m_spawningDelay = 0.0f;
        [SerializeField] private float m_spawningTimer = 0.0f;
        [SerializeField] private int m_poolSize;
        [SerializeField] private float m_mapBorderOffSet;
        private List<Crafting.CraftingStation> m_craftingStationsInScene = new List<Crafting.CraftingStation>();
        private Dictionary<Crafting.InteractableResource.EResourceType, float> m_resourcesWeightDictionary = new Dictionary<Crafting.InteractableResource.EResourceType, float>();

        //Tilemap refacto
        [SerializeField] private Tilemap m_tilemapRef;
        [SerializeField] private List<Vector3> m_spawnPositionsAvailable = new List<Vector3>();

        private Dictionary<GameObject, ObjectPool> m_resourceDictionary = new Dictionary<GameObject, ObjectPool>();
        private List<GameObject> m_resourceShardList = new List<GameObject>();
        private GenericObjectPool m_shardPool = new GenericObjectPool();
        private ObjectPool m_healingCollectablePool = new ObjectPool();
        //private SMapData m_mapData;

        //Resource Scaling
        [SerializeField] private float m_resourceValueUpgradeTimer;
        private float m_lastResourceValueUpgradeTime;
        [SerializeField] private int m_currentResourceValue = 1;

        private void Awake()
        {
            foreach (GameObject resource in m_resourcesPrefab)
            {
                if (!m_resourceDictionary.ContainsKey(resource))
                {
                    m_resourceDictionary.Add(resource, new ObjectPool());
                    m_resourceShardList.Add(resource.GetComponent<Crafting.InteractableResource>().GetResourceShardPrefab());
                }
            }

            //m_mapData = new SMapData(m_map);
        }

        private void Start()
        {
            m_spawningTimer = m_spawningDelay;

            GenerateGrid();
            PoolSetUp();
            m_craftingStationsInScene = Crafting.CraftingStation.GetCraftingStations();
            SetupCraftingStationsIcon();
            DictionarySetUp();
            GameManager.Instance.SetResourceManager(this);
        }
        private void OnDestroy()
        {
            Crafting.ResourceShards.ResetResourceShardsValue();
        }
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.C)) // Press C to add 1 enemy at will (for testing)
            //    CalculateSpawnPosition();

            m_spawningTimer -= Time.deltaTime;

            if (m_spawningTimer <= 0.0f)
            {
                m_spawningTimer = m_spawningDelay;
                CalculateSpawnPosition();
            }

            UpgradeResourcesValue();
        }
        private void PoolSetUp()
        {
            foreach (KeyValuePair<GameObject, ObjectPool> resource in m_resourceDictionary)
            {
                resource.Value.SetPoolSize(m_poolSize);
                resource.Value.CreatePool(resource.Key);
            }

            m_shardPool.SetPoolStartingSize(m_poolSize);
            m_shardPool.CreatePool(m_resourceShardList, "Resource shard");


            m_healingCollectablePool.SetPoolSize(m_poolSize);
            m_healingCollectablePool.CreatePool(m_healingCollectable);
        }
        private void DictionarySetUp()
        {
            float totalValues = 0;
            float reciprocalOfValue;
            float normalizedReciprocal;
            Dictionary<Crafting.InteractableResource.EResourceType, float> tempWeights = new Dictionary<Crafting.InteractableResource.EResourceType, float>();

            //Get all the reciprocal of the values
            foreach (KeyValuePair<Crafting.InteractableResource.EResourceType, ResourceData> resource in Crafting.InteractableResource.GetResourcesData())
            {
                if (resource.Value.m_resourceAmount > 0)
                {
                    reciprocalOfValue = 1.0f / resource.Value.m_resourceAmount;
                    tempWeights.Add(resource.Key, reciprocalOfValue);
                    totalValues += reciprocalOfValue;
                }
                else
                {
                    tempWeights.Add(resource.Key, 0);
                }
            }

            //Get the weight threshold and put it as weight
            foreach (KeyValuePair<Crafting.InteractableResource.EResourceType, float> resource in tempWeights)
            {
                normalizedReciprocal = (resource.Value / totalValues);
                m_resourcesWeightDictionary.Add(resource.Key, normalizedReciprocal);
            }
        }
        private void UpgradeResourcesValue()
        {
            if (GameManager.Instance.GameTimer - m_lastResourceValueUpgradeTime > m_resourceValueUpgradeTimer)
            {
                m_lastResourceValueUpgradeTime = GameManager.Instance.GameTimer;
                m_currentResourceValue++;
                Crafting.ResourceShards.UpgradeResourceShardsValue();
            }
        }
        public void SpawnHealingHeart(Transform enemy)
        {
            m_healingCollectablePool.Spawn(enemy.position);
        }

        #region ResourceSpawning
        private void GenerateGrid()
        {
            foreach (var positions in m_tilemapRef.cellBounds.allPositionsWithin)
            {
                if (m_tilemapRef.HasTile(positions))
                {
                    m_spawnPositionsAvailable.Add(m_tilemapRef.CellToWorld(positions));
                }
            }
        }

        private void CalculateSpawnPosition()
        {
            bool validPosFound = false;
            Vector2 spawnPosition = Vector2.zero;
            int whileiteration = 0;

            while (!validPosFound)
            {
                spawnPosition = RandomPosOnMap();

                validPosFound = CheckPosValidity(spawnPosition);

                //TODO fix this safety better
                whileiteration++;
                if (whileiteration > 50)
                {
                    Debug.Log("Reach max iteration of calculate spawn position");
                    validPosFound = true;
                }
            }

            SpawnResource(spawnPosition);
        }

        private Vector3 RandomPosOnMap()
        {
            int newPos = UnityEngine.Random.Range(0, m_spawnPositionsAvailable.Count);
            return (Vector2)m_spawnPositionsAvailable[newPos] + new Vector2(0.5f, 0.5f);
        }

        private bool CheckPosValidity(Vector2 positionToTest)
        {
            //Check if the position is valid
            Collider2D colliderOnPos = Physics2D.OverlapPoint(positionToTest);

            if (colliderOnPos != null) { return false; }

            return true;
        }

        private void SpawnResource(Vector2 spawnPosition)
        {
            //Get the maximum weight value
            float totalReciprocalNorm = 0.0f;
            foreach (float weight in m_resourcesWeightDictionary.Values)
            {
                totalReciprocalNorm += weight;
            }

            //Generate a weight between 0 and maximum weight value
            float randomWeight = UnityEngine.Random.Range(0, totalReciprocalNorm);
            float currentWeightValue = 0.0f;

            foreach (KeyValuePair<Crafting.InteractableResource.EResourceType, float> resource in m_resourcesWeightDictionary)
            {
                currentWeightValue += resource.Value;
                if (randomWeight < currentWeightValue)
                {
                    foreach (GameObject resourcePrefab in m_resourcesPrefab)
                    {
                        if (resourcePrefab.GetComponent<Crafting.InteractableResource>().GetResourceType() == resource.Key)
                        {
                            GameObject spawnedResource = m_resourceDictionary[resourcePrefab].Spawn(spawnPosition);
                            spawnedResource.GetComponent<Crafting.InteractableResource>().SetShardPoolRef(m_shardPool);
                            return;
                        }
                    }
                }
            }
        }
        #endregion
        #region Crafting
        private void SetupCraftingStationsIcon()
        {
            List<WeaponSystem.PlayerWeapon> weaponToSet = new List<WeaponSystem.PlayerWeapon>();
            foreach (WeaponSystem.PlayerWeapon playerWeapon in GameManager.Instance.Player.GetPlayerWeapons())
            {
                weaponToSet.Add(playerWeapon);
            }

            List<Crafting.CraftingStation> craftingStationsToSetUp = new List<Crafting.CraftingStation>();

            foreach (Crafting.CraftingStation station in m_craftingStationsInScene)
            {
                craftingStationsToSetUp.Add(station);
                //Debug.Log("Added to crafting station to set up" + station);
            }

            Shuffle(craftingStationsToSetUp);
            int index = 0;
            foreach (WeaponSystem.PlayerWeapon weapon in weaponToSet)
            {
                if (index == craftingStationsToSetUp.Count)
                {
                    break;
                }

                craftingStationsToSetUp[index].StationSetup(weapon);
                index++;
            }
        }
        private void Shuffle<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public GameObject SpawnShard(GameObject shardToSpawn, Vector2 posToSpawn)
        {
            return m_shardPool.Spawn(shardToSpawn, posToSpawn);
        }
        #endregion
    }
}
