using SpaceBaboon.WeaponSystem;
using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    public class DoSpecialAttack : BossEnemyState
    {
        private float m_chargeSpecialAttackTimer;
        private bool m_specialAttackDone;
        
        private float m_timeBeforeSpecialAttackLaunchedToStartAnim = 0.5f;
        private bool m_specialAttackAnimStarted = false;

        public override void OnEnter()
        {
            //Debug.Log("BossEnemy entering state: DoSpecialAttack\n");
            m_stateMachine.Agent.isStopped = true;
            m_specialAttackDone = false;
            m_specialAttackAnimStarted = false;
            m_chargeSpecialAttackTimer = m_stateMachine.UniqueData.specialAttackChargeDelay;
            m_stateMachine.Animator.SetTrigger("StartChargingSpecialAttack");
        }

        public override void OnExit()
        {
            m_stateMachine.Agent.isStopped = false;
            //Debug.Log("BossEnemy exiting state: DoSpecialAttack\n");
        }

        public override void OnUpdate()
        {
            m_chargeSpecialAttackTimer -= Time.deltaTime;

            if (m_chargeSpecialAttackTimer < m_timeBeforeSpecialAttackLaunchedToStartAnim
                && !m_specialAttackAnimStarted)
            {
                m_stateMachine.Animator.SetTrigger("DoSpecialAttack");
                m_specialAttackAnimStarted = true;
            }

            if (m_chargeSpecialAttackTimer < 0)
            {                
                LaunchSpecialAttack();
                m_specialAttackDone = true;
            }
        }

        public override void OnFixedUpdate()
        {           
        }

        public override bool CanEnter(IState currentState)
        {
            if (currentState is ChasingPlayer && m_stateMachine.SpecialAttackReady)
            {
                return true;
            }

            return false;
        }

        public override bool CanExit()
        {
            if (m_specialAttackDone || m_stateMachine.ReturnToDefaultState) 
            {
                return true;
            }

            return false;
        }

        private void LaunchSpecialAttack()
        {
            //TODO FIXME bug when instantiating a new special projectile boss when there is not enough in the pool



            //Debug.Log("!!!Special attack launched!!!");
                        
            Vector2 spawnPos = new Vector2(m_stateMachine.transform.position.x, m_stateMachine.transform.position.y);

            //Debug.Log("spawn pos is " + spawnPos);

            var projectile = m_stateMachine.EnemySpawner.m_enemyProjectilesPool.Spawn(m_stateMachine.UniqueData.bosses[m_stateMachine.CurrentBossIndex].specialProjectilePrefab, spawnPos);

            //Debug.Log("the projectile is " + projectile);
            //Debug.Log("the projectile has component projectile " + projectile.GetComponent<Projectile>());
            
            projectile.GetComponent<Projectile>()?.Shoot(m_stateMachine.Player.transform, 0, 0, 0);
        }
    }
}

