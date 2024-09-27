using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    [System.Serializable]
    public enum EBossTypes
    {
        Ice,
        Fire,
        Rock,
        Count
    }

    [System.Serializable]
    public struct SBoss
    {
        public EBossTypes bossType;
        public GameObject animControllerObject;
        //public Animator animator;
        //public AnimatorOverrideController controller;
        //public Sprite sprite;
        public Color color;                
        public GameObject specialProjectilePrefab;
    }

    [CreateAssetMenu(fileName = "NewBossEnemyData", menuName = "SpaceBaboon/ScriptableObjects/BossEnemyData")]
    public class BossEnemyData : EnemyData
    {
        [Header("BossEnemyGeneralUniqueStats")] 
        public List<SBoss> bosses;        
        public float possibleAggroRange;
        public float playerAggroRange;
        public float maxAttackRangeTriggerWhenChasingPlayer;
        public float craftingStationAttackRange;
        public float craftingStationAttackDelay;
        public float craftingStationAttackDamage;
        public float craftingStationAttackFXDistanceThreshold;
        public float specialAttackDelay;
        public float specialAttackChargeDelay;        
        public float basicAttackDelay;
        public float deadOnGroundDelay;
        public int basicAttacksNeededBeforeSpecial;
    }
}
