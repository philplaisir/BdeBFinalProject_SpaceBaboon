using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    public class ExplodingEnemy : Enemy, IExplodable
    {
        [SerializeField] private GameObject m_projectilePrefab;
        
        private ExplodingEnemyData m_uniqueData;        

        private Sprite m_baseSprite;
        private Color m_baseColor;
        private float m_chargingExplosionTimer = 0.0f;
        private bool m_isChargingExplosion = false;
        
        protected override void Start()
        {
            base.Start();
            VariablesSetUp();
        }        

        protected override void Update()
        {
            base.Update();

            if (!m_isActive)
                return;

            if (m_isChargingExplosion)
            {
                IExplodableUpdate();
                return;
            }

            if (m_distanceToPlayer < m_uniqueData.minDistanceForTriggeringBomb)
            {
                IExplodableSetUp();                
            }  
        }

        protected override void FixedUpdate()
        {
            if (!m_isActive)
                return;
        
            if (m_isChargingExplosion)
                return;

            Move(m_player.transform.position);            
        }

        private void UpdateColorBasedOnAnimCurve()
        {
            float colorScale = m_uniqueData.colorChangeCurve.Evaluate(1 - (m_chargingExplosionTimer / m_uniqueData.delayBeforeExplosion));

            float r = Mathf.Lerp(m_baseColor.r, m_uniqueData.imminentExplosionColor.r, colorScale);
            float g = Mathf.Lerp(m_baseColor.g, m_uniqueData.imminentExplosionColor.g, colorScale);
            float b = Mathf.Lerp(m_baseColor.b, m_uniqueData.imminentExplosionColor.b, colorScale);

            Color newColor = m_renderer.color;
            newColor.r = r;
            newColor.g = g;
            newColor.b = b;
            m_renderer.color = newColor;
        }

        private void VariablesSetUp()
        {
            m_uniqueData = m_characterData as ExplodingEnemyData;

            // Maybe randomize distance to trigger bomb from data             
            m_baseSprite = m_renderer.sprite;
            m_baseColor = m_renderer.color;
            m_chargingExplosionTimer = m_uniqueData.delayBeforeExplosion;
        }

        public void IExplodableSetUp()
        {
            m_isChargingExplosion = true;
            m_animator.enabled = false;
            m_rB.constraints = RigidbodyConstraints2D.FreezeAll;
            m_renderer.sprite = m_uniqueData.chargingExplosionSprite;            
        }

        public void IExplodableUpdate()
        {
            m_navMeshAgent.isStopped = true;
            m_chargingExplosionTimer -= Time.deltaTime;

            UpdateColorBasedOnAnimCurve();

            if (m_chargingExplosionTimer < 0)
            {                
                ResetVariables();
                Explode();
            }
        }

        public void Explode()
        {
            m_enemySpawner.m_enemyProjectilesPool.Spawn(m_projectilePrefab, transform.position);
            m_activeHealth = 0;
            m_parentPool.UnSpawn(gameObject);
            
        }

        public void StartExplosion() { /* Unused */ }        

        private void ResetVariables()
        {
            m_isChargingExplosion = false;
            m_animator.enabled = true;
            m_rB.constraints = RigidbodyConstraints2D.None;
            m_rB.constraints = RigidbodyConstraints2D.FreezeRotation;
            m_renderer.sprite = m_baseSprite;
            m_renderer.color = m_baseColor;
            m_chargingExplosionTimer = m_uniqueData.delayBeforeExplosion;
        }

        public override bool CanAttack()
        {
            return false;
        }

        protected override void BringCloserToPlayer()
        {
            if (!m_isChargingExplosion)
            {
                Vector3 teleportPos = m_enemySpawner.FindValidEnemyRandomPos();
                transform.position = teleportPos;
                m_checkIfTooCloseToPlayerAfterBroughtCloserTimer = m_checkIfTooCloseToPlayerAfterBroughtCloserDelay;
            }
        }
    }
}
