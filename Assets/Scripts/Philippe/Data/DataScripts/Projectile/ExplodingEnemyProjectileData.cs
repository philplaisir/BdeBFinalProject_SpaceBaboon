using SpaceBaboon.WeaponSystem;
using UnityEngine;

namespace SpaceBaboon
{
    [CreateAssetMenu(fileName = "NewExplodingEnemyProjectileData", menuName = "SpaceBaboon/ScriptableObjects/ExplodingEnemyProjectileData")]
    public class ExplodingEnemyProjectileData : ProjectileData
    {
        [Header("ExplodingEnemyProjectileUniqueStats")]
        public float maxExplosionSize;
        public AnimationCurve explosionSizeScalingCurve;
    }
}
