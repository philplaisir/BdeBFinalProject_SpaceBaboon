using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    public abstract class BossEnemyState : IState
    {
        protected BossEnemyControllerSM m_stateMachine;

        public void OnStart()
        {
            //throw new System.NotImplementedException();
        }

        public virtual void OnStart(BossEnemyControllerSM stateMachine)
        {
            m_stateMachine = stateMachine;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual bool CanEnter(IState currentState)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool CanExit()
        {
            throw new System.NotImplementedException();
        }
    }
}