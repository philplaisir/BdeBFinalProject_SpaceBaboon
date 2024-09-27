using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class BossSpecialProjectileIce : BossSpecialProjectile
    {
        private void OnTriggerEnter2D(Collider2D other)
        {

            if (other.CompareTag("Player"))
            {
                //m_player.IceZoneEffectsStart(0.5f, 0.5f);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                m_player.StartGlide(0.01f, 3.0f);
                //m_player.IceZoneEffectsStart(0.5f, 0.5f);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                //m_player.IceZoneEffectsEnd();
                m_player.StopGlide();
            }
        }


    }
}
