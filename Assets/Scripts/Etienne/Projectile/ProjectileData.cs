using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public enum EProjectileOwner
    {
        Player,
        Enemy,
        Boss
    }


    [CreateAssetMenu(fileName = "NewProjectileData", menuName = "SpaceBaboon/ScriptableObjects/ProjectileData")]
    public class ProjectileData : ScriptableObject
    {
        public EProjectileOwner owner;
        public Sprite sprite;
        public string projectileName;
        public float speed;
        public float damage;
        public float maxLifetime;
        public float defaultAcceleration;
        public float defaultMaxVelocity;
    }
}
