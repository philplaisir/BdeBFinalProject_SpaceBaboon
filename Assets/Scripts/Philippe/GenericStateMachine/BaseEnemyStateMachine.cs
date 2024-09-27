using System.Collections.Generic;

namespace SpaceBaboon.EnemySystem
{
    public abstract class BaseEnemyStateMachine<T> : Enemy where T : IState
    {
        protected T m_currentState;
        protected List<T> m_possibleStates;

        protected override void Awake()
        {
            base.Awake();
            CreatePossibleStates();                      
        }        

        protected override void Start()
        {
            base.Start();
        
            //foreach (IState state in m_possibleStates)
            //{
            //    state.OnStart();
            //}
            //m_currentState = m_possibleStates[0];
            //m_currentState.OnEnter();                       
        }

        protected override void Update()
        {
            base.Update();
            m_currentState.OnUpdate();
            TryStateTransition();      
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            m_currentState.OnFixedUpdate();            
        }

        protected virtual void CreatePossibleStates()
        {
        }

        protected void TryStateTransition()
        {
            if (!m_currentState.CanExit())
            {
                return;
            }

            //I can quit current state
            foreach (var state in m_possibleStates)
            {
                if (m_currentState.Equals(state))
                {
                    continue;
                }

                if (state.CanEnter(m_currentState))
                {
                    //Quit current state
                    m_currentState.OnExit();
                    m_currentState = state;
                    //Enter state state
                    m_currentState.OnEnter();
                    return;
                }
            }
        }

        public override void OnDamageTaken(float damage)
        {
            base.OnDamageTaken(damage);
        }
    }
}
