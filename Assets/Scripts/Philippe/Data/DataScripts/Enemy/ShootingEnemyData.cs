using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    [CreateAssetMenu(fileName = "NewShootingEnemyData", menuName = "SpaceBaboon/ScriptableObjects/ShootingEnemyData")]
    public class ShootingEnemyData : EnemyData
    {
        [Header("ShootingEnemyUniqueStats")]
        public float maxTargetAcquisitionRange;
        public float targetAcquisitionDelay;
    }
}
