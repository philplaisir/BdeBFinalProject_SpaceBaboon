using UnityEngine;

namespace SpaceBaboon
{
    public abstract class BaseStats<T> : MonoBehaviour, IStatsEditable where T : UnityEngine.Object
    {
        public abstract ScriptableObject GetData();
    }
}
