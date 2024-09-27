using UnityEngine;

namespace SpaceBaboon
{
    public interface IBaitable
    {
        public void StartBait(Transform baitPosition, float baitTime);
    }
}
