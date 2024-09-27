using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class BossSpecialProjectileMud : BossSpecialProjectile
    {

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {

            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                m_player.StartSlow(0.5f, 1.0f);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                //m_player.EndSlow();
            }
        }



    }
}
