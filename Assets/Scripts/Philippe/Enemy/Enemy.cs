using SpaceBaboon.PoolingSystem;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace SpaceBaboon.EnemySystem
{
    public class Enemy : Character, IPoolableGeneric, IStatsEditable, ISlowable, IBaitable
    {
        public event Action m_eventEnemyDeath = delegate { };

        protected EnemyData m_enemyUniqueData;

        protected GenericObjectPool m_parentPool;
        protected bool m_isActive = false;

        protected CircleCollider2D m_circleCollider;

        protected Player m_player;
        protected EnemySpawner m_enemySpawner;

        protected float m_distanceToPlayer = 0.0f;
        private float m_contactAttackTimer = 0.0f;
        protected Transform m_currentDestination;
        protected float m_pullTimer;
        protected bool m_isPulled = false;
        protected float m_slowTimer;
        protected bool m_isSlowed;
        protected bool m_contactAttackReady = true;
        protected static float m_healthMultiplier = 1.0f;
        //private float m_bonusDamage = 0.0f;
        //private float m_bonusAcceleration;
        //private float m_bonusMaxVelocity;
        //private float m_bonusAttackDelay;

        protected Vector2 m_noVectorValue = Vector2.zero;

        protected NavMeshAgent m_navMeshAgent;
        protected float m_navMeshAgentInitialSpeed;
        protected float m_currentNavAgentSpeed;

        protected float m_checkIfTooCloseToPlayerAfterBroughtCloserDelay = 0.05f;
        protected float m_checkIfTooCloseToPlayerAfterBroughtCloserTimer = 0.0f;

        protected int m_scaledHP
        {
            get
            {
                return (int)(m_enemyUniqueData.defaultHealth * m_healthMultiplier);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            VariablesSetUpAwake();
        }

        protected virtual void Start()
        {
            VariablesSetUpStart();
        }

        protected override void Update()
        {
            base.Update();

            if (!m_isActive)
                return;

            CalculateDistanceToPlayer();

            if (!m_contactAttackReady)
                ReadyContactAttack();

            StatusUpdate();

            if (m_distanceToPlayer > m_enemyUniqueData.distanceBeforeTeleportingCloser)
            {
                if (m_enemyUniqueData.enemyType != EEnemyTypes.Boss)
                    BringCloserToPlayer();
            }

            if (m_checkIfTooCloseToPlayerAfterBroughtCloserTimer >= 0.0f)
            {
                m_checkIfTooCloseToPlayerAfterBroughtCloserTimer -= Time.deltaTime;

                if (m_distanceToPlayer < 100)
                {
                    //Debug.Log("Needs to move away because to close");
                    BringCloserToPlayer();
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!m_isActive)
                return;

            Move(m_currentDestination.position);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
        }

        protected void StopMovement()
        {
            m_navMeshAgent.acceleration = -m_characterData.defaultAcceleration;
        }

        private void CalculateDistanceToPlayer()
        {
            m_distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        }

        private void StatusUpdate()
        {
            if (m_isSlowed)
            {
                m_slowTimer -= Time.deltaTime;
                if (m_slowTimer < 0)
                {
                    EndSlow();
                }
            }
            if (m_isPulled)
            {
                m_pullTimer -= Time.deltaTime;
                if (m_pullTimer < 0)
                {
                    m_currentDestination = m_player.transform;
                    m_isPulled = false;
                }
            }
        }

        public override void Move(Vector2 value)
        {
            m_navMeshAgent.SetDestination(value);
            CheckForSpriteDirectionSwap(m_navMeshAgent.velocity);
        }

        private void ReadyContactAttack()
        {
            m_contactAttackTimer -= Time.deltaTime;

            if (m_contactAttackTimer < 0.0f)
            {
                m_contactAttackReady = true;
            }
        }

        public void ContactAttack(Vector2 contactPos)
        {
            m_player.OnDamageTaken(m_enemyUniqueData.defaultContactAttackDamage);

            SpawnContactAttackVFX(contactPos, m_player.transform);

            m_contactAttackTimer = m_enemyUniqueData.defaultContactAttackDelay /* + or * bonus */;
            m_contactAttackReady = false;
        }

        // TODO make sure particle system is at foreground
        protected void SpawnContactAttackVFX(Vector2 contactPos, Transform target)
        {
            Vector3 contactPosVec = new Vector3(contactPos.x, contactPos.y, 2);

            Vector2 direction = target.position - contactPosVec;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject vfx = FXSystem.FXManager.Instance.PlayVFX(FXSystem.EVFXType.EnemySlashAttack, contactPosVec);
            vfx.transform.rotation = rotation;
        }

        public override void OnDamageTaken(float damage)
        {
            m_activeHealth -= damage;
            SpriteFlashRed();
            //DamagePopUp.Create(this.transform.position, damage);
            GameObject vfx = FXSystem.FXManager.Instance.PlayVFX(FXSystem.EVFXType.EnemyDamagePopUp, transform.position);
            var script = vfx.GetComponent<FXSystem.AnimateDamagePopUp>();
            if (script != null)
            {
                script.Activate(damage);
            }

            //Debug.Log(gameObject.name + " enemy hit -- now has " + m_activeHealth + " health");
            if (m_activeHealth <= 0)
            {
                m_eventEnemyDeath?.Invoke();
                HealthSpawner();
                if (m_enemyUniqueData.enemyType != EEnemyTypes.Boss)
                {
                    m_parentPool.UnSpawn(gameObject);
                    return;
                }
            }
        }

        protected virtual void BringCloserToPlayer()
        {
            Vector3 teleportPos = m_enemySpawner.FindValidEnemyRandomPos();
            transform.position = teleportPos;
            m_checkIfTooCloseToPlayerAfterBroughtCloserTimer = m_checkIfTooCloseToPlayerAfterBroughtCloserDelay;
        }

        protected void HealthSpawner()
        {
            //Debug.Log("HealthSpawner called");
            float randomNumber = UnityEngine.Random.Range(0f, 100f);
            if (randomNumber < m_enemyUniqueData.healthDropChance)
            {
                // Object should spawn
                //Debug.Log("Object spawned at " + transform.position);
                GameManager.Instance.m_ressourceManager.SpawnHealingHeart(transform);
            }
            else
            {
                // Object should not spawn
                //Debug.Log("No object spawned.");
            }

        }
        static public void UpgradeEnemyHealthMultiplier(float addedScaling)
        {
            m_healthMultiplier += addedScaling;
        }
        static public void ResetEnemyHealthMultiplier()
        {
            m_healthMultiplier = 1;
        }
        public void registerPuzzle(CraftingPuzzle craftstation)
        {
            m_eventEnemyDeath += () => craftstation.PuzzleCounter();
        }
        public void UnregisterPuzzle(CraftingPuzzle craftstation)
        {
            m_eventEnemyDeath = null;
        }

        private void VariablesSetUpAwake()
        {
            m_circleCollider = GetComponent<CircleCollider2D>();
            m_animator = GetComponent<Animator>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_navMeshAgent.updateRotation = false;
            m_navMeshAgent.updateUpAxis = false;
            m_navMeshAgentInitialSpeed = m_navMeshAgent.speed;
            m_currentNavAgentSpeed = m_navMeshAgent.speed;
            m_enemyUniqueData = m_characterData as EnemyData;
        }

        private void VariablesSetUpStart()
        {
            m_player = GameManager.Instance.Player;
            m_enemySpawner = GameManager.Instance.EnemySpawner;
            m_activeHealth = m_scaledHP;
            m_navMeshAgent.speed = m_characterData.defaultMaxVelocity;
            m_navMeshAgent.acceleration = m_characterData.defaultAcceleration;
            m_currentDestination = m_player.transform;
        }

        #region HitBox
        public virtual bool CanAttack()
        {
            return m_contactAttackReady;
        }


        public void StartSlow(float slowAmount, float slowTime)
        {
            m_currentNavAgentSpeed = m_navMeshAgentInitialSpeed * slowAmount;
            m_navMeshAgent.speed = m_currentNavAgentSpeed;
            m_slowTimer = slowTime;
            m_isSlowed = true;
        }
        public void EndSlow()
        {
            m_isSlowed = false;
            m_currentNavAgentSpeed = m_navMeshAgentInitialSpeed;
            m_navMeshAgent.speed = m_currentNavAgentSpeed;
        }
        public void StartBait(Transform baitPosition, float baitTime)
        {
            m_currentDestination = baitPosition;
            m_isPulled = true;
            m_pullTimer = baitTime;
        }
        #endregion

        #region Tools
        public override ScriptableObject GetData() { return m_characterData as EnemyData; }
        #endregion

        #region ObjectPooling
        public bool IsActive
        {
            get { return m_isActive; }
        }

        public virtual void Activate(Vector2 pos, GenericObjectPool pool)
        {
            ResetValues(pos);
            SetComponents(true);
            m_parentPool = pool;

        }

        public void Deactivate()
        {
            SetComponents(false);
        }

        protected void ResetValues(Vector2 pos)
        {
            transform.position = pos;
            m_activeHealth = m_scaledHP;
            m_navMeshAgent.speed = m_characterData.defaultMaxVelocity;
            m_navMeshAgent.acceleration = m_characterData.defaultAcceleration;
        }

        protected virtual void SetComponents(bool value)
        {
            m_isActive = value;
            m_renderer.enabled = value;
            m_circleCollider.enabled = value;
            m_navMeshAgent.isStopped = !value;
        }
        #endregion
    }
}
