using SpaceBaboon.PoolingSystem;
using UnityEngine;

namespace SpaceBaboon.Crafting
{
    public class ResourceShards : MonoBehaviour, IPoolableGeneric
    {
        //Serialize variables
        [SerializeField] private ResourceData m_resourceData;
        [SerializeField] private float m_timeBeforeAutoCollect;
        [SerializeField] private float m_autoCollectTravelSpeed;
        [SerializeField] private float m_maxVelocity;
        [SerializeField] private float m_MaxStuckSafetyTimer;

        //Variables
        private Player m_recoltingPlayer;
        private float m_currentPushStrenght;
        private Vector2 m_pushDirection;
        private float m_currentAutoCollectTimer;
        private bool m_isAutoCollecting = false;
        private float m_currentStuckSafetyTimer;
        static private int m_currentResourceScalingValue = 1;

        //For ObjectPool        
        protected GenericObjectPool m_parentPool;
        protected bool m_isActive = false;
        protected SpriteRenderer m_renderer;
        protected CircleCollider2D m_collider;
        protected Rigidbody2D m_rb;

        // Update is called once per frame
        void Awake()
        {
            m_renderer = GetComponent<SpriteRenderer>();
            m_collider = GetComponent<CircleCollider2D>();
            m_rb = GetComponent<Rigidbody2D>();
        }
        void Start()
        {

        }
        void Update()
        {
            if (m_isActive && !m_isAutoCollecting)
            {
                WaitingForAutoCollectLogic();
                if (m_isAutoCollecting)
                {
                    StuckSafety();
                }
            }
        }
        void FixedUpdate()
        {
            if (m_isAutoCollecting)
            {
                MoveTowardsPlayer();
            }
        }
        private void StartAutoCollect()
        {
            m_isAutoCollecting = true;
            m_currentStuckSafetyTimer = m_MaxStuckSafetyTimer;
        }
        static public void UpgradeResourceShardsValue()
        {
            m_currentResourceScalingValue++;
        }
        static public void ResetResourceShardsValue()
        {
            m_currentResourceScalingValue = 1;
        }
        private void MoveTowardsPlayer()
        {
            Vector2 directionToPlayer = (m_recoltingPlayer.transform.position - transform.position).normalized;
            m_rb.AddForce(directionToPlayer * m_autoCollectTravelSpeed, ForceMode2D.Impulse);
            m_autoCollectTravelSpeed += Time.fixedDeltaTime;
            m_rb.velocity = Vector2.ClampMagnitude(m_rb.velocity, m_maxVelocity);
        }
        private void WaitingForAutoCollectLogic()
        {
            m_currentAutoCollectTimer -= Time.deltaTime;
            if (m_currentAutoCollectTimer < 0)
            {
                StartAutoCollect();
            }
        }
        public void Initialization(Vector2 direction, float pushStrenght, Player recoltingPlayer)
        {
            m_rb.AddForce(direction * pushStrenght, ForceMode2D.Impulse);
            m_recoltingPlayer = recoltingPlayer;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                //Debug.Log("ResourceShard collided with player");
                GetCollected();
            }
        }
        private void GetCollected()
        {
            m_recoltingPlayer.AddResource(m_resourceData.m_resourceType, m_currentResourceScalingValue);
            m_parentPool.UnSpawn(gameObject);
        }
        private void StuckSafety()
        {
            m_currentStuckSafetyTimer -= Time.fixedDeltaTime;
            if (m_currentStuckSafetyTimer < 0)
            {
                GetCollected();
            }
        }
        #region ObjectPooling
        public bool IsActive
        {
            get { return m_isActive; }
        }

        public virtual void Activate(Vector2 pos, GenericObjectPool pool)
        {
            //Debug.Log("Activate parent grenade appeler");
            ResetValues(pos);
            SetComponents(true);

            m_parentPool = pool;
        }

        public virtual void Deactivate()
        {
            //Debug.Log("Deactivate parent grenade appeler");
            SetComponents(false);
        }
        protected virtual void ResetValues(Vector2 pos)
        {
            transform.position = pos;
            m_currentAutoCollectTimer = m_timeBeforeAutoCollect;
            m_isAutoCollecting = false;
        }
        protected virtual void SetComponents(bool value)
        {
            //Debug.Log("SetComponents parent appeler");
            m_isActive = value;
            m_renderer.enabled = value;
            m_collider.enabled = value;
        }
        #endregion
    }
}
