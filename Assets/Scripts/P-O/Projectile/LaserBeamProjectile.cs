using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    [RequireComponent(typeof(LineRenderer), typeof(EdgeCollider2D))]
    public class LaserBeamProjectile : Projectile, IPiercing
    {
        private LineRenderer m_laserDisplay;
        private EdgeCollider2D m_laserHitBox;
        private Transform m_initialWeaponPosition;
        [SerializeField] private float m_laserWidth;
        private float m_currentLaserDuration;
        [SerializeField] private float m_maxLaserDuration;
        //IPiercing variables
        [SerializeField] PiercingData m_PiercingData;
        private int m_currentPiercingLeft;
        private Transform m_lastEnemyPosition;

        protected override void Awake()
        {
            base.Awake();
            m_laserDisplay = GetComponent<LineRenderer>();
            m_laserHitBox = GetComponent<EdgeCollider2D>();
        }
        protected void Start()
        {
            IPiercingSetUp();
        }
        public override void Shoot(Transform target, float maxRange, float attackZone, float damage, Transform playerPosition)
        {
            base.Shoot(target, maxRange, attackZone, damage, playerPosition);
            //Debug.Log("Laser was shot");
            Vector2 directionVector = (target.position - transform.position).normalized;
            m_currentPiercingLeft = (int)attackZone * m_PiercingData.m_piercingMultiplier;
            RaycastHit2D[] enemyHits = Physics2D.RaycastAll(transform.position, directionVector, maxRange);

            foreach (RaycastHit2D hit in enemyHits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    m_currentPiercingLeft--;
                    m_lastEnemyPosition = hit.collider.transform;
                    if (m_currentPiercingLeft <= 0)
                    {
                        //Debug.Log("Laser raycast hit an enemy");                        
                        break;
                    }
                }
            }
            if (m_initialWeaponPosition != null && m_lastEnemyPosition != null)
            {
                SetLaserPosition();
                OnPiercing();
            }
        }
        public void IPiercingSetUp()
        {
            m_currentPiercingLeft = m_PiercingData.m_piercingMultiplier;
            m_initialWeaponPosition = transform;
            m_laserDisplay.startWidth = m_laserWidth;
            m_laserDisplay.endWidth = m_laserWidth;
        }
        protected override void Update()
        {
            base.Update();
            if (m_isActive)
            {

                m_currentLaserDuration -= Time.deltaTime;

                if (m_currentLaserDuration < 0)
                {
                    //Debug.Log("calling last pierce");
                    LastPierce();
                }
            }
        }
        public void LastPierce()
        {
            m_parentPool.UnSpawn(gameObject);
        }

        public void OnPiercing()
        {
            m_currentLaserDuration = m_maxLaserDuration;
        }

        protected override void ResetValues(Vector2 pos)
        {
            m_lifetime = 0.0f;
            m_initialWeaponPosition.position = pos;
            m_currentLaserDuration = m_maxLaserDuration;
        }
        protected override void SetComponents(bool value)
        {
            //Debug.Log("SetComponents laserbeam appeler avec " + value);
            m_isActive = value;
            m_laserDisplay.enabled = value;
            m_laserHitBox.enabled = value;
        }
        private void SetLaserPosition()
        {
            //Debug.Log("Laser entre les points : " + m_initialWeaponPosition.position + " et " + m_lastEnemyPosition.position);
            m_laserDisplay.positionCount = 2;
            m_laserDisplay.SetPosition(0, m_initialWeaponPosition.position);
            m_laserDisplay.SetPosition(1, m_lastEnemyPosition.position);

            Vector2 laserStartPoint = m_laserHitBox.transform.InverseTransformPoint(m_initialWeaponPosition.position);
            Vector2 laserEndPoint = m_laserHitBox.transform.InverseTransformPoint(m_lastEnemyPosition.position);

            m_laserHitBox.SetPoints(new List<Vector2> { laserStartPoint, laserEndPoint });
        }
    }
}
