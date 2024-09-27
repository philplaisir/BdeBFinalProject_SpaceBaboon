using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class FlameThrower : PlayerWeapon
    {
        [SerializeField] private float m_detectionOffSet = 5;

        private float m_detectionRange;
        private Transform m_target;

        protected void Start()
        {
            base.Start();
            m_detectionRange = currentRange + m_detectionOffSet;
        }
        protected override Transform GetTarget()
        {
            for (int i = 0; i < (m_detectionRange * 12); i++)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, i);

                foreach (var collider in colliders)
                {
                    if (collider.gameObject.tag == "Enemy")
                    {
                        return collider.gameObject.transform;
                    }
                }
            }
            return base.GetTarget();
        }
    }
}
