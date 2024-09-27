using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "SpaceBaboon/ScriptableObjects/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        //public enum EPlayerWeaponType
        //{
        //    Melee,
        //    FlameThrower,
        //    GrenadeLauncher,
        //    Shockwave,
        //    LaserBeam,
        //    Count
        //}

        public int maxLevel;
        public float maxRange;
        public float attackZone;
        public float attackSpeed;
        public float baseDamage;
        public EPlayerWeaponType weaponName;
        public FXSystem.ESFXType shootAudioCue;
        public Sprite icon;
        public GameObject projectilePrefab;

        //Upgrade variables
        public float m_rangeScaling;
        public float m_speedScaling;
        public float m_zoneScaling;
        public float m_damageScaling;
    }
}
