using SpaceBaboon.PoolingSystem;
using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class Projectile : BaseStats<MonoBehaviour>, IPoolableGeneric, IStatsEditable
    {
        [SerializeField] protected ProjectileData m_projectileData;

        protected Vector2 m_direction;
        protected Transform m_target;
        protected float m_lifetime = 0.0f;
        protected float m_bonusDmg = 0;
        protected float m_damage = 0;

        //For ObjectPool        
        protected GenericObjectPool m_parentPool;
        protected bool m_isActive = false;
        protected SpriteRenderer m_renderer;
        protected CircleCollider2D m_collider;

        protected Rigidbody2D m_rb;
        protected Animator m_animator;

        protected virtual void Awake()
        {
            m_renderer = GetComponent<SpriteRenderer>();
            m_collider = GetComponent<CircleCollider2D>();
            m_rb = GetComponent<Rigidbody2D>();
            m_animator = GetComponent<Animator>();
        }
        protected virtual void Update()
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
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision) { }
        public virtual void Shoot(Transform target, float maxRange, float attackZone, float damage, Transform playerPosition = null)
        {
            //m_direction = target.position;
            m_damage = damage;
        }

        public virtual float OnHit(Character characterHit)
        {
            //Debug.Log("OnHit called by :  " + gameObject.name + "with " + m_damage + " damage");
            return m_damage;
        }

        protected virtual void Move()
        {
            m_rb.AddForce(m_direction * m_projectileData.defaultAcceleration /* + or * bonus */, ForceMode2D.Force);

            if (m_direction.magnitude > 0)
                RegulateVelocity();
        }

        protected void RegulateVelocity()
        {
            if (m_rb.velocity.magnitude > m_projectileData.defaultMaxVelocity)
            {
                m_rb.velocity = m_rb.velocity.normalized;
                m_rb.velocity *= m_projectileData.defaultMaxVelocity;
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
            //m_isActive = true;

            m_parentPool = pool;
        }

        public virtual void Deactivate()
        {
            //Debug.Log("Deactivate parent grenade appeler");
            //m_isActive = false;
            SetComponents(false);
        }

        protected virtual void ResetValues(Vector2 pos)
        {
            m_lifetime = 0.0f;
            transform.position = pos;
        }
        protected virtual void SetComponents(bool value)
        {
            //Debug.Log("SetComponents parent appeler");
            m_isActive = value;
            m_renderer.enabled = value;
            m_collider.enabled = value;
        }
        #endregion

        public override ScriptableObject GetData()
        {
            return m_projectileData;
        }


    }
}
