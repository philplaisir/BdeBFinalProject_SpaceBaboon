using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    public class ChasingPlayer : BossEnemyState
    {
        private float m_basicAttackTimer;
        private int m_basicAttacksDone;

        public override void OnEnter()
        {
            //Debug.Log("BossEnemy entering state: ChasingPlayer\n");

            m_stateMachine.SpecialAttackReady = false;
            m_stateMachine.SpecialAttackTimer = m_stateMachine.UniqueData.basicAttackDelay; //TODO maybe change this delay to something else
            m_basicAttackTimer = m_stateMachine.UniqueData.basicAttackDelay;
            m_basicAttacksDone = 0;
        }

        public override void OnExit()
        {
            //Debug.Log("BossEnemy exiting state: ChasingPlayer\n");
        }

        public override void OnUpdate()
        {
            if (m_stateMachine.UniqueData.maxAttackRangeTriggerWhenChasingPlayer < m_stateMachine.DistanceToPlayer)
                return;

            if (m_basicAttacksDone == m_stateMachine.UniqueData.basicAttacksNeededBeforeSpecial)
            {
                if (!m_stateMachine.SpecialAttackReady)
                {
                    m_stateMachine.SpecialAttackTimer -= Time.deltaTime;
                }

                if (m_stateMachine.SpecialAttackTimer < 0)
                {
                    m_stateMachine.SpecialAttackReady = true;
                }
                return;
            }

            m_basicAttackTimer -= Time.deltaTime;

            if (m_basicAttackTimer < 0)
            {
                ExecuteBasicAttackSine();
                m_basicAttackTimer = m_stateMachine.UniqueData.basicAttackDelay;
                m_basicAttacksDone++;
            }
        }

        public override void OnFixedUpdate()
        {
            m_stateMachine.Move(m_stateMachine.Player.transform.position);
        }

        public override bool CanEnter(IState currentState)
        {
            //Debug.Log("can enter chasing player 1");
            if (currentState is MovingToStation || currentState is AttackingStation || currentState is DoSpecialAttack)
            {
                //Debug.Log("can enter chasing player 2");
                if (m_stateMachine.StationAvailableToTarget && m_stateMachine.PlayerInAggroRange && m_stateMachine.PlayerInTargetedCraftingStationRange)
                {
                    return true;
                }
                //Debug.Log("can enter chasing player 3");

                if (!m_stateMachine.StationAvailableToTarget)
                {
                    return true;
                }
                //Debug.Log(" can enter chasing player 4");
            }

            return false;
        }

        public override bool CanExit()
        {
            //Debug.Log("can exit chasing player 1");
            if (m_stateMachine.StationAvailableToTarget)
            {
                return true;
            }
            //Debug.Log("can exit chasing player 2");

            if (m_stateMachine.SpecialAttackReady)
            {
                return true;
            }
            //Debug.Log("can exit chasing player 3");

            if (!m_stateMachine.PlayerInTargetedCraftingStationRange && m_stateMachine.StationAvailableToTarget)
            {
                return true;
            }
            //Debug.Log("can exit chasing player 4");

            return false;
        }

        private void ExecuteBasicAttackSine()
        {
            m_stateMachine.Animator.SetTrigger("DoBasicAttackSine");
            m_stateMachine.SineGun.GetTarget(m_stateMachine.Player.transform);
        }
    }
}
