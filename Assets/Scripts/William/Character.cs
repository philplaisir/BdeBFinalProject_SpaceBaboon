using System;
using UnityEngine;

namespace SpaceBaboon
{
    public class Character : BaseStats<MonoBehaviour>, SpaceBaboon.IDamageable, IStatsEditable
    {
        //BaseRef
        [SerializeField] protected CharacterData m_characterData;
        protected GameObject m_characterPrefab;
        protected SpriteRenderer m_renderer;        
        protected Color m_defaultRendererColor;        
        protected Rigidbody2D m_rB;
        protected Animator m_animator;
        
        //BaseVariables
        protected Vector2 m_movementDirection;
        protected float m_activeHealth;
        protected float m_activeVelocity;
        protected float m_accelerationMulti = 1;
        protected float m_angularVelocityMulti = 1;
        
        //BonusVariables
        protected float m_bonusHealth;
        protected float m_bonusVelocity;

        //Timer
        protected float m_spriteFlashTimer = 0.5f;
        private const float FLASH_TIMER_RESET_VALUE = 0.5f;

        //Cheats related
        //private bool m_isInvincible = false; // TODO make it more genral also
        protected float m_speedMultiplierCheat = 1.0f;

        protected float AccelerationValue
        {
            get { return m_characterData.defaultAcceleration * m_speedMultiplierCheat * m_accelerationMulti; }
        }
        protected float MaxVelocity
        {
            get { return m_characterData.defaultMaxVelocity * m_speedMultiplierCheat * m_angularVelocityMulti; }
        }

        protected virtual void Awake()
        {
            m_renderer = GetComponent<SpriteRenderer>();
            m_defaultRendererColor = m_renderer.material.color;
            m_rB = GetComponent<Rigidbody2D>();
            m_animator = GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            HandleSpriteFlashTimer();
        }

        protected void HandleSpriteFlashTimer()
        {
            if (m_spriteFlashTimer > 0)
            {
                //Debug.Log("In Handle " + m_spriteFlashTimer);
                m_spriteFlashTimer -= Time.deltaTime;

                if (m_spriteFlashTimer <= 0)
                {
                    m_renderer.material.color = m_defaultRendererColor;
                    m_spriteFlashTimer = 0;
                }
            }
        }

        //Methods        
        public virtual void Move(Vector2 values) {}

        protected void RegulateVelocity() 
        {
            if (m_rB.velocity.magnitude > MaxVelocity)
            {
                m_rB.velocity = m_rB.velocity.normalized;
                m_rB.velocity *= MaxVelocity;
            }
        }

        protected void CheckForSpriteDirectionSwap(Vector2 direction)
        {
            if (direction.x > 0)
            {
                m_renderer.flipX = false;
            }
            if (direction.x < 0)
            {
                m_renderer.flipX = true;
            }
        }   
        
        protected void SpriteFlashRed()
        {
            m_renderer.material.color = Color.red;
            m_spriteFlashTimer = FLASH_TIMER_RESET_VALUE;
        }

        public virtual void OnDamageTaken(float values) {}

        public override ScriptableObject GetData() { return m_characterData; }
    }

}
