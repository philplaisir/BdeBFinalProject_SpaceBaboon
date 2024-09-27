using Cinemachine;
using UnityEngine;

namespace SpaceBaboon
{
    public class AdjustCameraZoom : MonoBehaviour
    {
        private CinemachineVirtualCamera m_camera;

        private float m_scale;

        private const float MIN_ORTHO_SIZE = 40;
        private const float ORTHO_SIZE_MAX_ADDED_VALUE = 24;

        private void Start()
        {
            m_camera = GetComponent<CinemachineVirtualCamera>();
            //m_scale = GameManager.Instance.GetWindowSizeScale();
            m_scale = GameManager.Instance.WindowSizeScale;

            //m_camera.Follow = GameManager.Instance.Player;

            AdjustZoom();
        }


        private void AdjustZoom()
        {
            //recoit %
            //opposé du %
            float reverseScale = 1.0f - m_scale;
            //utiliser pour savoir à quel point on augmente orthosize

            m_camera.m_Lens.OrthographicSize = MIN_ORTHO_SIZE + (ORTHO_SIZE_MAX_ADDED_VALUE * reverseScale);
            //m_camera.m_Lens.OrthographicSize = MIN_ORTHO_SIZE;
        }
    }
}
