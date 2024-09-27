using UnityEngine;

namespace SpaceBaboon
{
    [CreateAssetMenu(fileName = "New ExplodableData", menuName = "SpaceBaboon/ScriptableObjects/ExplodableData")]

    public class ExplodableData : ScriptableObject
    {
        public float m_explosionRadius;
        public float m_maxExplosionTime;
    }
}
