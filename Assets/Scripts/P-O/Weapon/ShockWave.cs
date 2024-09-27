using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class ShockWave : PlayerWeapon
    {
        protected override Transform GetTarget()
        {
            return transform;
        }
    }
}
