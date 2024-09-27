using SpaceBaboon.WeaponSystem;
using UnityEngine;

namespace SpaceBaboon
{
    [CreateAssetMenu(fileName = "NewBossSineProjectileData", menuName = "SpaceBaboon/ScriptableObjects/BossSineProjectileData")]
    public class BossSineProjectileData : ProjectileData
    {
        [Header("BossSineProjectileUniqueStats")]
        public float sineWaveFrequency;
        public float sineWaveAmplitude;

    }
}
