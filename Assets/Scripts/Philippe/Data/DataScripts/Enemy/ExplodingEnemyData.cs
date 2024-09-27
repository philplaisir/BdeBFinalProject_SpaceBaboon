using UnityEngine;

namespace SpaceBaboon.EnemySystem
{
    [CreateAssetMenu(fileName = "NewExplodingEnemyData", menuName = "SpaceBaboon/ScriptableObjects/ExplodingEnemyData")]
    public class ExplodingEnemyData : EnemyData
    {
        [Header("ExplodingEnemyUniqueStats")]
        public Sprite chargingExplosionSprite;
        public Color imminentExplosionColor;
        public AnimationCurve colorChangeCurve;
        public float minDistanceForTriggeringBomb;
        public float delayBeforeExplosion;
    }
}
