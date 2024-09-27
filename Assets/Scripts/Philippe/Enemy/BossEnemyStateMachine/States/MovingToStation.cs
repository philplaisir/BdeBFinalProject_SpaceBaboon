using UnityEngine;
using UnityEngine.AI;

namespace SpaceBaboon.EnemySystem
{
    public class MovingToStation : BossEnemyState
    {
        public override void OnEnter()
        {
            //Debug.Log("BossEnemy entering state: MovingToStation\n");
        }

        public override void OnExit()
        {
            //Debug.Log("BossEnemy exiting state: MovingToStation\n");
        }

        public override void OnUpdate()
        {
        }

        public override void OnFixedUpdate()
        {
            if (!m_stateMachine.StationAvailableToTarget)
                return;
            
            m_stateMachine.Move(m_stateMachine.TargetedCraftingStation.transform.position);
        }

        public override bool CanEnter(IState currentState)
        {
            //Debug.Log("can enter moving to station 1");
            if (!m_stateMachine.StationAvailableToTarget)
            {
                return false;
            }
            //Debug.Log("can enter moving to station 2");

            if (currentState is DoSpecialAttack)
            { 
                if (!m_stateMachine.PlayerInAggroRange || !m_stateMachine.PlayerInTargetedCraftingStationRange)
                {
                    return true;
                }
            }

            //Debug.Log("can enter moving to station 3");

            if (m_stateMachine.SpecialAttackReady)
            {
                return false;
            }
            //Debug.Log("can enter moving to station 4");

            if(m_stateMachine.PlayerInAggroRange && m_stateMachine.PlayerInTargetedCraftingStationRange)
            {
                return false;
            }

            return true;
        }

        public override bool CanExit()
        {
            //Debug.Log("can exit moving to station 1");
            if (!m_stateMachine.StationAvailableToTarget)
            {
                return true;
            }
            //Debug.Log("can exit moving to station 2");
            if (m_stateMachine.InTargetedCraftingStationAttackRange)
            {
                return true;
            }
            //Debug.Log("can exit moving to station 3");
            if (m_stateMachine.PlayerInAggroRange && m_stateMachine.PlayerInTargetedCraftingStationRange)
            {
                return true;
            }
            //Debug.Log("can exit moving to station 4");

            return false;
        }
    }
}
