using UnityEngine;
using Cinemachine;

namespace SpaceBaboon
{
    public class CameraShake : MonoBehaviour
    {
        private CinemachineVirtualCamera m_virtualCam;
        private CinemachineBasicMultiChannelPerlin m_cbmcp;

        private float m_timer;

        private void Awake()
        {
            m_virtualCam = GetComponent<CinemachineVirtualCamera>();
            m_cbmcp = m_virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Start()
        {
            FXSystem.FXManager.Instance.RegisterCameraShakeController(this);
            StopShake(); //Initialize
        }

        private void Update()
        {
            HandleTimer();
        }

        public void ShakeCamera(float intensity, float frequency, float duration = 0.5f)
        {
            m_cbmcp.m_AmplitudeGain = intensity;
            m_cbmcp.m_FrequencyGain = frequency;
            m_timer = duration;
        }

        private void StopShake()
        {
            m_cbmcp.m_AmplitudeGain = 0f;
            m_timer = 0;
        }

        private void HandleTimer()
        {
            if (m_timer > 0)
            {
                m_timer -= Time.deltaTime;

                if (m_timer <= 0)
                {
                    StopShake();
                }
            }
        }
    }
}
