using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class LaserBeam : PlayerWeapon
    {
        protected override Transform GetTarget()
        {
            //Get furthest enemy
            //Debug.Log("Laser beam get target");
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

        //protected override Transform GetTarget()
        //{
        //    Debug.Log("Laser beam get target");
        //    // Get all colliders within maxRange
        //    var colliders = Physics2D.OverlapCircleAll(transform.position, m_weaponData.maxRange);
        //    Transform furthestEnemy = null;
        //    float furthestDistance = 0.0f;

        //    foreach (var collider in colliders)
        //    {
        //        if (collider.gameObject.CompareTag("Enemy")) // Use CompareTag for efficiency
        //        {
        //            float distanceTowardTarget = Vector2.Distance(transform.position, collider.transform.position);
        //            if (distanceTowardTarget > furthestDistance)
        //            {
        //                furthestEnemy = collider.transform;
        //                furthestDistance = distanceTowardTarget;
        //            }
        //        }
        //    }

        //    return furthestEnemy;
        //}
    }
}
