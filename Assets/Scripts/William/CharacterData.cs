using UnityEngine;

namespace SpaceBaboon
{
    public class CharacterData : ScriptableObject
    {
        [Header("CharacterStats")]
        public int index;
        public string characterName;
        public int defaultHealth;
        public Sprite sprite;
        public GameObject prefab;
        public float defaultAcceleration;
        public float defaultMaxVelocity;
        public float dashMaximumVelocity;
    }
}
