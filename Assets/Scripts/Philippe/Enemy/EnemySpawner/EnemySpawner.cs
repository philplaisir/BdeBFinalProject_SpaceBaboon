using SpaceBaboon.PoolingSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SpaceBaboon.EnemySystem
{
    public enum EEnemyTypes
    {
        Melee,
        Shooting,
        Kamikaze,
        Boss,
        Count
    }

    [System.Serializable]
    public struct EnemyToPool
    {
        public GameObject enemyPrefab;
        public EEnemyTypes enemyType;
        [Range(0, 10)] public int spawnProbability;
        public bool canSpawn;

        public void SetCanSpawn(bool value)
        {
            canSpawn = value;
        }
    }

    public class EnemySpawner : MonoBehaviour, IGameDifficultyScaling
    {
        //TODO maybe add a scriptable object for data

        const int MAX_ITERATIONS_TO_FIND_VALID_POS = 100;

        private Player m_player;

        [field: Header("OBJECT POOLS")]
        [SerializeField] private GenericObjectPool m_enemyPool = new GenericObjectPool();
        [SerializeField] private List<EnemyToPool> m_pooledEnemies = new List<EnemyToPool>();
        [SerializeField] private List<GameObject> m_pooledBoss = new List<GameObject>();
        private GenericObjectPool m_bossPool = new GenericObjectPool();
        [SerializeField] private List<GameObject> m_pooledElite = new List<GameObject>();
        private GenericObjectPool m_elitePool = new GenericObjectPool();

        [SerializeField] private Transform m_bossSpawningZone;

        [SerializeField] public GenericObjectPool m_enemyProjectilesPool = new GenericObjectPool();
        [SerializeField] public List<GameObject> m_pooledEnemyProjectiles = new List<GameObject>();

        [field: Header("SPAWNER LOGIC")]
        [SerializeField] private Camera m_cam;
        [SerializeField] private float m_spawnWRadiusModifierFromScreenCorner = 0.0f; // TODO check this out, not sure why the radius is so huge that I need to add negative distance 
        [SerializeField] private float m_spawnWRadiusThreshold = 10.0f;
        [SerializeField] private Transform m_lastResortWSpawnPosOne;
        [SerializeField] private Transform m_lastResortWSpawnPosTwo;
        [SerializeField] private float m_spawningDelay = 0.0f;
        [SerializeField] private bool m_isSpawning = true;
        [SerializeField] private int m_enemiesAmountToSpawnOneShot = 0;
        // TODO maybe remove [SerializeField] of bool
        [SerializeField] private bool m_spawnGroup = false;
        private float m_spawnWRadius = 0.0f;

        [field: Header("PROGRESSION LOGIC")]
        [SerializeField] private float m_SpawnTimeUpgradeTimer;
        [SerializeField] private float m_spawnTimeUpgradeRatio;
        private float m_spawningTimer = 0.0f;
        private float m_lastSpawnTimeUpgrade = 0.0f;
        private float m_initialSpawnTimer;
        private int m_amountOfEnemySpawned = 1;
        private float m_lastAmountSpawnUpgrade = 0.0f;
        [SerializeField] private float m_amountSpawnedUpgradeTimer;
        private float m_lastSpawnEvent = 0.0f;
        [SerializeField] private float m_spawnEventTimer;
        [SerializeField] private int m_spawnEventAmountMultiplier;
        [SerializeField] private float m_bossSpawnTimer;
        private float m_lastBossSpawn = 0.0f;
        [SerializeField] private float m_enemyHealthUpgradeTimer;
        private float m_lastEnemyHealthUpgradeTimer = 0.0f;
        [SerializeField] private float m_enemyHealthScaling;

        [SerializeField] private Tilemap m_tilemapRef;
        [SerializeField] public Tilemap m_obstacleTilemapRef;
        [SerializeField] public Tilemap m_bossTileMap;
        private List<Vector3> m_spawnWorldPositionsAvailable = new List<Vector3>();

        private static bool s_popUpHasBeenCalled = false;

        private List<Vector3Int> m_validSpawnTilePositions = new List<Vector3Int>();

        private void Awake()
        {
            CreateEnemySpawnerPools();
        }

        private void Start()
        {
            RegisterToGameManager();
            m_player = GameManager.Instance.Player;
            m_spawnWRadius = CalculateValidWSpawnRadius();
            GetValidTilemapPositions();
            GenerateValidWorldSpawnPositionsGrid();
            m_spawningTimer = m_spawningDelay;
            m_initialSpawnTimer = m_spawningDelay;
        }
        private void OnDestroy()
        {
            Enemy.ResetEnemyHealthMultiplier();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) // Press C to add 1 enemy at will (for testing)
                SpawnOneEnemy();

            if (m_isSpawning)
            {
                SpawnWithTimer();
                CheckGameTimer();
            }

            if (m_spawnGroup)
                SpawnGroup(m_enemiesAmountToSpawnOneShot);
        }

        private void RegisterToGameManager()
        {
            GameManager.Instance.SetEnemySpawner(this);
        }

        private float CalculateValidWSpawnRadius()
        {
            Vector3 playerStartPos = m_player.transform.position;
            Vector3 screenCornerWPos = m_cam.ScreenToWorldPoint(new Vector3(0, 0, m_cam.nearClipPlane));

            float radiusBetweenCenterAndCorner = Vector3.Distance(playerStartPos, screenCornerWPos);

            //Debug.Log("Radius between center and corner " + radiusBetweenCenterAndCorner);
            //Debug.Log("Radius modifier is " + m_spawnWRadiusModifierFromScreenCorner);
            //Debug.Log("Start spawnig radius is " + (radiusBetweenCenterAndCorner + m_spawnWRadiusModifierFromScreenCorner));
            return radiusBetweenCenterAndCorner + m_spawnWRadiusModifierFromScreenCorner;
        }

        private void GetValidTilemapPositions()
        {
            foreach (var position in m_tilemapRef.cellBounds.allPositionsWithin)
            {
                if (m_tilemapRef.HasTile(position))
                {
                    m_validSpawnTilePositions.Add(position);
                }

                //if (IsTilePositionValid(position) && m_tilemapRef.HasTile(position))
                //{
                //    m_validSpawnTilePositions.Add(position);
                //}
            }

            //Debug.Log("Number of valid tile position " + m_validSpawnTilePositions.Count);
        }

        private void GenerateValidWorldSpawnPositionsGrid()
        {
            foreach (var position in m_validSpawnTilePositions)
            {
                m_spawnWorldPositionsAvailable.Add(m_tilemapRef.CellToWorld(position));
            }

            //foreach (var position in m_spawnWorldPositionsAvailable)
            //{
            //    Instantiate(m_testingPrefab, position, Quaternion.identity);
            //}
        }

        private void SpawnOneEnemy()
        {
            //Debug.Log("Try to spawn one");
            Vector3 spawnWorldPos = FindValidEnemyRandomPos();
            m_enemyPool.Spawn(GetRandomEnemyType(), spawnWorldPos);
        }

        private void SpawnWithTimer()
        {
            m_spawningTimer -= Time.deltaTime;

            if (m_spawningTimer <= 0.0f)
            {
                m_spawningTimer = m_spawningDelay;
                for (int i = 0; i < m_amountOfEnemySpawned; i++)
                {
                    SpawnOneEnemy();
                }
            }
        }

        private void SpawnGroup(int numberOfEnemies)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                SpawnOneEnemy();
            }
            m_spawnGroup = false;
        }

        public Vector3 FindValidEnemyRandomPos()
        {
            //float spawnRadius = CalculateValidSpawnRadius();            
            return RandomValidPosOnCircleAroundPlayer(m_spawnWRadius);
        }

        private GameObject GetRandomEnemyType()
        {
            int totalProbabilities = 0;
            for (int i = 0; i < m_pooledEnemies.Count; i++)
            {
                if (m_pooledEnemies[i].canSpawn)
                {
                    totalProbabilities += m_pooledEnemies[i].spawnProbability;
                }
            }

            int randomValue = Random.Range(0, totalProbabilities);
            int cumulativeProbability = 0;

            for (int i = 0; i < m_pooledEnemies.Count; i++)
            {
                if (!m_pooledEnemies[i].canSpawn)
                {
                    continue;
                }

                cumulativeProbability += m_pooledEnemies[i].spawnProbability;

                if (randomValue < cumulativeProbability)
                {
                    if (i < m_pooledEnemies.Count && m_pooledEnemies[i].enemyPrefab != null)
                    {
                        return m_pooledEnemies[i].enemyPrefab;
                    }
                    break;
                }
            }

            Debug.LogError("Did not find an enemy type");
            return null;
        }

        private Vector3 RandomValidPosOnCircleAroundPlayer(float radius)
        {
            //Profiler.BeginSample("RandomValidPosOnCircleAroundPlayer");

            // Could add a way to not get the same number while in the loop, needs testing to know if it's better for optimization
            //List<int> checkedIndexes = new List<int>();
            //for (int j = 0; j < checkedIndexes.Count; j++)
            //checkedIndexes.Add(randomIndex);

            int validTilemapPosIndex = 0;
            bool noValidPosFound = false;

            for (int i = 0; i < MAX_ITERATIONS_TO_FIND_VALID_POS; i++)
            {
                int randomIndex = Random.Range(0, m_spawnWorldPositionsAvailable.Count);

                float distanceWPlayerAndRandomTile = Vector3.Distance(m_player.transform.position, m_spawnWorldPositionsAvailable[randomIndex]);

                if (distanceWPlayerAndRandomTile > radius - m_spawnWRadiusThreshold && distanceWPlayerAndRandomTile < radius + m_spawnWRadiusThreshold)
                {
                    //Debug.Log("position is " + m_spawnPositionsAvailable[randomIndex]);
                    //Debug.Log("distance between player and new spawn position" + Vector3.Distance(m_player.transform.position, m_spawnPositionsAvailable[randomIndex]));
                    validTilemapPosIndex = randomIndex;
                    break;
                }

                if (i == MAX_ITERATIONS_TO_FIND_VALID_POS - 1)
                    noValidPosFound = true;
            }

            if (noValidPosFound)
            {
                //Debug.Log("No position found");

                float furthestDistance = 0.0f;
                Transform furthestPosition = null;
                float distanceWPlayerAndLastResortPosOne = Vector3.Distance(m_player.transform.position, m_lastResortWSpawnPosOne.position);
                float distanceWPlayerAndLastResortPosTwo = Vector3.Distance(m_player.transform.position, m_lastResortWSpawnPosTwo.position);

                if (distanceWPlayerAndLastResortPosOne > furthestDistance)
                {
                    furthestDistance = distanceWPlayerAndLastResortPosOne;
                    furthestPosition = m_lastResortWSpawnPosOne;
                }

                if (distanceWPlayerAndLastResortPosTwo > furthestDistance)
                {
                    furthestPosition = m_lastResortWSpawnPosTwo;
                }

                //Profiler.EndSample();

                return furthestPosition.position;
            }

            //Profiler.EndSample();

            return m_spawnWorldPositionsAvailable[validTilemapPosIndex]/* + new Vector3(0.5f, 0.5f, 0f)*/;
        }

        private bool CheckPosValidity(Vector2 positionToTest)
        {
            //Check if the position is valid
            Collider2D colliderOnPos = Physics2D.OverlapPoint(positionToTest);

            if (colliderOnPos != null) { return false; }

            return true;
        }

        private void CreateEnemySpawnerPools()
        {
            List<GameObject> enemyPrefabs = new List<GameObject>();
            foreach (EnemyToPool enemy in m_pooledEnemies)
                enemyPrefabs.Add(enemy.enemyPrefab);

            m_enemyPool.CreatePool(enemyPrefabs, "Enemies");
            m_enemyProjectilesPool.CreatePool(m_pooledEnemyProjectiles, "Enemy Projectiles");

            m_bossPool.SetPoolStartingSize(10);
            m_bossPool.CreatePool(m_pooledBoss, "Boss");

            m_elitePool.SetPoolStartingSize(10);
            m_elitePool.CreatePool(m_pooledElite, "Elite");
        }

        public int[] GetEnemyKillStats()
        {
            int[] stats = new int[4];

            for (int i = 0; i < 4; i++)
            {
                stats[i] = m_enemyPool.GetUnspawnedTotalByObject(m_pooledEnemies[i].enemyPrefab);
            }
            return stats;
        }

        #region Cheats

        public void SetIsSpawning(bool value)
        {
            m_isSpawning = value;
        }

        public void SetDelay(float value)
        {
            m_spawningDelay = value;
        }

        public float GetDelay()
        {
            return m_spawningDelay;
        }

        public void ToggleSpawnByEnemyType(EEnemyTypes type, bool value)
        {
            //little gymnastics because List<Structs> can't be directly modified

            EnemyToPool currentEnemy = m_pooledEnemies[(int)type];
            currentEnemy.canSpawn = value;
            m_pooledEnemies[(int)type] = currentEnemy;
        }

        public void CheatSpawnGroup(EEnemyTypes type, int amount)
        {
            //Debug.Log(m_pooledEnemies[(int)type].enemyPrefab.name + "   " + amount);


            for (int i = 0; i < amount; i++)
            {
                Vector2 spawnWorldPos = FindValidEnemyRandomPos();
                m_enemyPool.Spawn(m_pooledEnemies[(int)type].enemyPrefab, spawnWorldPos);
            }
        }

        public void CheckGameTimer()
        {
            if (GameManager.Instance.GameTimer - m_lastSpawnTimeUpgrade > m_SpawnTimeUpgradeTimer)
            {
                //Debug.Log("It has been 5 seconds");
                m_lastSpawnTimeUpgrade = GameManager.Instance.GameTimer;
                m_spawningDelay -= m_spawnTimeUpgradeRatio * m_spawningDelay;
            }
            if (GameManager.Instance.GameTimer - m_lastAmountSpawnUpgrade > m_amountSpawnedUpgradeTimer)
            {
                //Debug.Log("Amount of enemy spawned is " + m_amountOfEnemySpawned);
                m_lastAmountSpawnUpgrade = GameManager.Instance.GameTimer;
                m_amountOfEnemySpawned++;
            }
            if (GameManager.Instance.GameTimer - m_lastSpawnEvent > m_spawnEventTimer)
            {
                //Debug.Log("Amount of enemy spawned is " + m_amountOfEnemySpawned);
                m_lastSpawnEvent = GameManager.Instance.GameTimer;
                SpawnEvent();
            }
            if (GameManager.Instance.GameTimer - m_lastBossSpawn > m_bossSpawnTimer)
            {
                //Debug.Log("Spawn boss");
                m_lastBossSpawn = GameManager.Instance.GameTimer;
                SpawnRandomBoss();
            }
            if (GameManager.Instance.GameTimer - m_lastEnemyHealthUpgradeTimer > m_enemyHealthUpgradeTimer)
            {
                m_lastEnemyHealthUpgradeTimer = GameManager.Instance.GameTimer;
                Enemy.UpgradeEnemyHealthMultiplier(m_enemyHealthScaling);
            }
        }
        private void SpawnEvent()
        {
            int eventChoice = Random.Range(0, 1);
            if (eventChoice == 0)
            {
                //-1 to avoid to spawn boss
                EEnemyTypes enemyType = (EEnemyTypes)Random.Range(0, (int)EEnemyTypes.Count - 1);
                CheatSpawnGroup(enemyType, (m_spawnEventAmountMultiplier * m_amountOfEnemySpawned));
            }
            else if (eventChoice == 1)
            {
                Vector2 spawnWorldPos = FindValidEnemyRandomPos();
                m_elitePool.Spawn(m_pooledElite[0], spawnWorldPos);
            }
        }
        private void SpawnRandomBoss()
        {
            int spawnIndex = Random.Range(0, m_pooledBoss.Count);

            Vector3 validTilePos = Vector3.zero;
            Vector3Int currentPlayerTilePos = m_bossTileMap.WorldToCell(m_cam.transform.position);
            int radiusThreshold = 1;

            List<Vector3Int> positionsNearRadius = new List<Vector3Int>();

            foreach (var tilePos in m_bossTileMap.cellBounds.allPositionsWithin)
            {
                if (m_bossTileMap.HasTile(tilePos))
                {
                    float distance = Vector3Int.Distance(currentPlayerTilePos, tilePos);

                    positionsNearRadius.Add(tilePos);

                }

            }

            if (positionsNearRadius.Count > 0)
            {
                int randomIndex = Random.Range(0, positionsNearRadius.Count);
                validTilePos = m_bossTileMap.CellToWorld(positionsNearRadius[randomIndex]) + new Vector3(0.5f, 0.5f, 0f);
            }
            else
            {
                Debug.Log("No valid pos found on circle");
            }
            m_bossPool.Spawn(m_pooledBoss[spawnIndex], validTilePos);

            if (!s_popUpHasBeenCalled)
            {
                GameManager.Instance.DisplayTutorialWindow(TutorialSystem.ETutorialType.BossSpawning, transform.position);
                s_popUpHasBeenCalled = true;
            }

        }
        public void UpdateStats()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
