using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "SpaceBaboon/ScriptableObjects/EnemyData")]
    public class EnemyData : CharacterData
    {
        [Header("EnemyUniqueStats")]   
        public EEnemyTypes enemyType;
        public int defaultContactAttackDamage;
        public float defaultContactAttackDelay;
        public float distanceBeforeTeleportingCloser;
        public float healthDropChance;
    }
}
