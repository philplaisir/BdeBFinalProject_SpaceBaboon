using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    public class AttackingStation : BossEnemyState
    {
        private float m_craftingStationAttackTimer;        

        public override void OnEnter()
        {
            //Debug.Log("BossEnemy entering state: AttackingStation\n");

            m_craftingStationAttackTimer = m_stateMachine.UniqueData.craftingStationAttackDelay;
            m_stateMachine.Agent.isStopped = true;
            m_stateMachine.Animator.SetBool("AttackingStation", true);
        }

        public override void OnExit()
        {
            m_stateMachine.Agent.isStopped = false;
            m_stateMachine.Animator.SetBool("AttackingStation", false);
            //Debug.Log("BossEnemy exiting state: AttackingStation\n");
        }

        public override void OnUpdate()
        {
            m_craftingStationAttackTimer -= Time.deltaTime;

            if (m_craftingStationAttackTimer < 0)
            {
                //Debug.Log("Crafting station attacked");
                m_stateMachine.AttackTargetedCraftingStation();                
                m_craftingStationAttackTimer = m_stateMachine.UniqueData.craftingStationAttackDelay;
            }
        }

        public override void OnFixedUpdate()
        {            
        }

        public override bool CanEnter(IState currentState)
        {
            if (currentState is MovingToStation)
            {
                if (m_stateMachine.InTargetedCraftingStationAttackRange)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override bool CanExit()
        {
            if (m_stateMachine.PlayerInAggroRange && m_stateMachine.PlayerInTargetedCraftingStationRange)
            {
                return true;
            }

            if (m_stateMachine.TargetedStationDisabled)
            {
                m_stateMachine.TargetedStationDisabled = false;
                return true;
            }

            if (m_stateMachine.GetDistanceToTargetedCraftingStation() > m_stateMachine.UniqueData.craftingStationAttackRange )
            {                
                return true;
            }

            return false;
        }
    }
}
