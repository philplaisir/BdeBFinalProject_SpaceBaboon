using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class GrenadeLauncher : PlayerWeapon
    {
        protected override Transform GetTarget()
        {
            for (int i = 0; i < currentRange; i++)
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
            //Didn't find an enemy
            return null;
        }
    }
}
