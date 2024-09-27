using UnityEngine;

namespace SpaceBaboon
{
    [CreateAssetMenu(fileName = "New ResourceData", menuName = "SpaceBaboon/ScriptableObjects/ResourceData")]
    public class ResourceData : ScriptableObject
    {
        // Collect data
        public float m_cooldownMax;

        //Resource
        public Sprite m_icon;
        public Crafting.InteractableResource.EResourceType m_resourceType;
        public int m_resourceAmount;
        public GameObject m_shardPrefab;
    }
}
