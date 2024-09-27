using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceBaboon
{
    [CreateAssetMenu(fileName = "New ResourceData", menuName = "SpaceBaboon/ScriptableObjects/PlayerData", order = 0)]
    public class PlayerData : CharacterData
    {
        [Header("PlayerUniqueStats")]
        public float defaultDashCd;
        public float defaultDashAcceleration;
        public float defaultDashDuration;
        public AnimationCurve defaultDashCurve;
    }


}
