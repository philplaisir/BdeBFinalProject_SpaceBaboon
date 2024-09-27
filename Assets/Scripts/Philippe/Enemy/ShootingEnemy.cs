using UnityEngine;
using SpaceBaboon.WeaponSystem;

namespace SpaceBaboon.EnemySystem
{
    public class ShootingEnemy : Enemy
    {
        private ShootingEnemyData m_uniqueData;
        private EnemyWeapon m_weapon;
                
        private float m_targetAcquisitionTimer = 0.0f;        

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

            if (m_distanceToPlayer > m_uniqueData.maxTargetAcquisitionRange)
                return;

            TryToAcquireTarget(); // TODO check the logic, may be possible to simplify
        }

        private void TryToAcquireTarget()
        {
            if (m_targetAcquisitionTimer < 0.0f && m_contactAttackReady)
            {
                AimWeapon();
                return;
            }

            if (m_distanceToPlayer < m_uniqueData.maxTargetAcquisitionRange)
            {
                m_targetAcquisitionTimer -= Time.deltaTime;
            }            
        }
        
        private void AimWeapon()
        {            
            m_targetAcquisitionTimer = m_uniqueData.targetAcquisitionDelay;
            m_weapon.GetTarget(m_player.transform);
        }

        public override void Move(Vector2 value)
        {
            // TODO see if I can bring improvements
            if(m_distanceToPlayer > m_uniqueData.maxTargetAcquisitionRange)
            {
                m_navMeshAgent.isStopped = false;
                base.Move(value);
                return;
            }

            if (m_rB.velocity.magnitude < 0.1f)
            {
                m_navMeshAgent.isStopped = true;                
                return;
            }                

            StopMovement();
        }

        private void VariablesSetUp()
        {
            m_uniqueData = m_characterData as ShootingEnemyData;
            m_weapon = GetComponentInChildren<EnemyWeapon>();
            m_targetAcquisitionTimer = m_uniqueData.targetAcquisitionDelay;
        }
    }
}
