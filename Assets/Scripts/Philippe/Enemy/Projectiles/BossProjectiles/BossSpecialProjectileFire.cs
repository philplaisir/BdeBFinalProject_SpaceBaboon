using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class BossSpecialProjectileFire : BossSpecialProjectile
    {
        private float m_damageTimer = 0.0f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) 
            {
                m_damageTimer = m_uniqueData.damageDelay;
                m_player.OnDamageTaken(m_uniqueData.damageAOE);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                m_damageTimer -= Time.deltaTime;

                if (m_damageTimer <= 0.0f)
                {
                    m_damageTimer = m_uniqueData.damageDelay;
                    m_player.OnDamageTaken(m_uniqueData.damageAOE);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {

            }
        }

    }
}
