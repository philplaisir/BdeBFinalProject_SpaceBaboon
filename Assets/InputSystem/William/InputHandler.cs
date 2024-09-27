using UnityEngine;
using UnityEngine.InputSystem;
using static SpaceBaboon.PlayerInput;

namespace SpaceBaboon
{
    public class InputHandler : MonoBehaviour, IPlayerMovementActions, IPlayerInteractionActions
    {
        public static InputHandler instance;

        public delegate void MoveEvent(Vector2 values);
        public delegate void DashEvent();
        public delegate void CollectResourceEvent();


        public PlayerInput m_Input;
        public MoveEvent m_MoveEvent;
        public DashEvent m_DashStartEvent;
        public CollectResourceEvent m_CollectResourceEvent;
        //public DashEvent m_DashEndEvent;



        void Awake()
        {
            if (instance != null)
            {
                instance.KillInstance();
                return;
            };
            instance ??= this;

            m_Input = new PlayerInput();

        }

        private void KillInstance()
        {
            Destroy(this);
        }

        void Start()
        {
            //m_Input.PlayerMovement.SetCallbacks(this);
            m_Input.PlayerMovement.PlayerDash.started += OnPlayerDash;
            m_Input.PlayerMovement.PlayerDirection.performed += OnPlayerDirection;
            m_Input.PlayerMovement.PlayerDirection.canceled += OnPlayerDirection;
            m_Input.PlayerInteraction.CollectResource.started += OnCollectResource;
        }

        private void OnEnable()
        {
            m_Input.Enable();
        }

        private void OnDisable()
        {
            m_Input.Disable();
        }

        public void OnPlayerDirection(InputAction.CallbackContext context)
        {
            m_MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPlayerDash(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                m_DashStartEvent?.Invoke();
            }
        }

        public void OnCollectResource(InputAction.CallbackContext context)
        {
            m_CollectResourceEvent?.Invoke();
        }

        //public void OnPlayerDashEnd(InputAction.CallbackContext context)
        //{
        //    m_DashEndEvent?.Invoke();
        //}
    }
}