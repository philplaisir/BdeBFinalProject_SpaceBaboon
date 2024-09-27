using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class LaserTest : PlayerWeapon
    {
        protected override Transform GetTarget()
        {
            //Get furthest enemy

            Transform furthestEnemy = null;
            float furthestDistance = 0.0f;

            for (int i = 0; i < m_weaponData.maxRange; i++)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, i);

                foreach (var collider in colliders)
                {
                    if (collider.gameObject.tag == "Enemy")
                    {
                        float distanceTowardTarget = Vector2.Distance(transform.position, collider.transform.position);
                        if (distanceTowardTarget > furthestDistance)
                        {
                            furthestEnemy = collider.gameObject.transform;
                            furthestDistance = distanceTowardTarget;
                        }
                    }
                }
            }

            //Didn't find an enemy
            return furthestEnemy != null ? furthestEnemy : transform;
        }
    }
}
