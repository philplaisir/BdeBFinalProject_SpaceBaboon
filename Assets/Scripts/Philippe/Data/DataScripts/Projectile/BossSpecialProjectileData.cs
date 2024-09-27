using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    [CreateAssetMenu(fileName = "NewBossSpecialProjectileData", menuName = "SpaceBaboon/ScriptableObjects/BossSpecialProjectileData")]
    public class BossSpecialProjectileData : ProjectileData
    {
        [Header("BossSpecialProjectileUniqueStats")]        
        public AnimationCurve sizeScalingCurve;
        public float radiusSizeMultiplier;
        public float sizeScalingDuration;

        [Header("Boss Type Ice")]
        public int test;

        [Header("Boss Type Fire")]
        public float damageAOE;
        public float damageDelay;

        [Header("Boss Type Mud")]
        public int test3;
    }
}
