using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class ExplodingEnemyExplosion : Projectile
    {
        private ExplodingEnemyProjectileData m_uniqueData;
        private Vector3 m_initialScale;

        private bool m_animationPlayed = false;        

        protected void Start()
        {
            VariablesSetUp();
        }

        protected override void Update()
        {
            base.Update();

            if (!m_isActive)
            {
                m_animationPlayed = false;
                return;
            }

            if (m_isActive && !m_animationPlayed)
            {
                m_animator.SetTrigger("StartExplosion");
                m_animationPlayed = true;
            }

            UpdateScaleBasedOnAnimCurve();
        }

        private void UpdateScaleBasedOnAnimCurve()
        {
            float curveValue = m_uniqueData.explosionSizeScalingCurve.Evaluate(m_lifetime);

            float newScale = Mathf.Lerp(m_initialScale.x, m_uniqueData.maxExplosionSize, curveValue);
            transform.localScale = new Vector3(newScale, newScale, 0);
        }

        private void UpdateDamageBasedOnAnimCurve() // TODO maybe
        {            
        }

        private void VariablesSetUp()
        {
            m_uniqueData = m_projectileData as ExplodingEnemyProjectileData;
            m_animator = GetComponent<Animator>();
            m_initialScale = transform.localScale;
            m_damage = m_uniqueData.damage;
        }

        public override float OnHit(Character characterHit)
        {
            //Debug.Log("OnHit called by :  " + gameObject.name + "with " + m_uniqueData.damage + " damage");
            return m_uniqueData.damage;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            m_animator.SetTrigger("LifetimeOver");
        }
    }
}
