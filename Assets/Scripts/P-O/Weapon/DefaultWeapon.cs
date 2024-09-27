using SpaceBaboon.WeaponSystem;
using UnityEngine;

namespace SpaceBaboon
{
    public class DefaultWeapon : PlayerWeapon
    {
        // Start is called before the first frame update
        protected override Transform GetTarget()
        {
            //Aim for closest enemy
            for (int i = 0; i < m_weaponData.maxRange; i++)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, i);

                foreach (var collider in colliders)
                {
                    if (collider.gameObject.tag == "Enemy")
                    {
                        //Vector2 enemyPosition = collider.gameObject.transform;
                        //Vector2 enemyDirection = enemyPosition - new Vector2(transform.position.x, transform.position.y);
                        Debug.Log("Targeting " + collider.gameObject.transform.position);
                        return collider.gameObject.transform;
                    }
                }


            }

            //Didn't find an enemy
            Debug.Log("Didnt find enemy to target");
            return transform;
        }
    }
}
