using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class ShockWaveProjectile : Projectile, IExplodable
    {
        private Transform m_weaponPos;

        //LineRenderer value
        [SerializeField] protected int m_segments = 100;
        [SerializeField] protected float m_innerRadius = 5f;
        [SerializeField] protected float m_thickness = 0.5f;
        [SerializeField] protected float m_slowAmount = 0.5f;
        [SerializeField] protected float m_slowTime = 3f;
        private LineRenderer m_damageZoneDisplay;

        //IExplodable data
        [SerializeField] private ExplodableData m_ExplodableData;
        protected float m_currentExplosionTime = 0.0f;
        protected float m_currentExplosionSize = 0.0f;
        protected float m_explosionSizeRatio;
        protected bool m_isExploding = false;

        protected override void Awake()
        {
            base.Awake();
            m_damageZoneDisplay = GetComponent<LineRenderer>();
        }
        protected override void Update()
        {
            if (m_isActive)
            {
                base.Update();
                FollowPlayer();
                IExplodableUpdate();
                GenerateCircle();
            }
        }
        protected void FollowPlayer()
        {
            transform.position = m_weaponPos.position;
        }
        protected void GenerateCircle()
        {
            m_damageZoneDisplay.loop = true;
            // Set the number of points
            m_damageZoneDisplay.positionCount = m_segments;
            m_damageZoneDisplay.useWorldSpace = false;

            m_damageZoneDisplay.startWidth = m_thickness;
            m_damageZoneDisplay.endWidth = m_thickness;

            float deltaTheta = (2f * Mathf.PI) / m_segments;
            float currentTheta = 0f;

            for (int i = 0; i < m_segments; i++)
            {
                float x = m_currentExplosionSize * Mathf.Cos(currentTheta);
                float y = m_currentExplosionSize * Mathf.Sin(currentTheta);
                Vector3 pos = new Vector3(x, y, 0);
                m_damageZoneDisplay.SetPosition(i, pos);
                currentTheta += deltaTheta;
            }
        }
        protected override void SetComponents(bool value)
        {
            //Debug.Log("SetComponents parent appeler");
            m_isActive = value;
            m_renderer.enabled = value;
            m_collider.enabled = value;
            m_damageZoneDisplay.enabled = value;
        }
        protected override void ResetValues(Vector2 pos)
        {
            base.ResetValues(pos);
            IExplodableSetUp();
        }
        public override void Shoot(Transform target, float maxRange, float attackZone, float damage, Transform playerPosition)
        {
            base.Shoot(target, maxRange, attackZone, damage, playerPosition);
            m_weaponPos = target;
            m_thickness = attackZone;
            m_innerRadius = maxRange;
            //StartExplosion();
        }
        public void Explode()
        {
            throw new System.NotImplementedException();
        }

        public void IExplodableSetUp()
        {
            StartExplosion();
            m_currentExplosionSize = 0.0f;
            m_explosionSizeRatio = m_innerRadius / m_ExplodableData.m_maxExplosionTime;
        }

        public void IExplodableUpdate()
        {
            if (m_isExploding)
            {
                m_currentExplosionTime -= Time.deltaTime;

                if (m_currentExplosionTime < 0)
                {
                    m_parentPool.UnSpawn(gameObject);
                }

                m_currentExplosionSize = m_explosionSizeRatio * (m_ExplodableData.m_maxExplosionTime - m_currentExplosionTime);
                m_collider.radius = m_currentExplosionSize + m_thickness;
            }
        }
        public override float OnHit(Character characterHit)
        {
            if (characterHit != null)
            {
                if (characterHit.GetComponent<ISlowable>() != null)
                {
                    characterHit.GetComponent<ISlowable>().StartSlow(m_slowAmount, m_slowTime);
                    //Debug.Log(characterHit + " was slowed");
                }
            }
            return base.OnHit(characterHit);
        }
        public void StartExplosion()
        {
            m_currentExplosionTime = m_ExplodableData.m_maxExplosionTime;
            m_isExploding = true;
        }
    }
}
