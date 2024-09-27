using TMPro;
using UnityEngine;

namespace SpaceBaboon.FXSystem
{
    public class AnimateDamagePopUp : MonoBehaviour
    {
        private TextMeshPro m_text;
        private Color m_textColor;
        private float m_textVerticalSpeed = 20.0f;
        private float m_textFadingSpeed = 1.5f;

        private void Awake()
        {
            m_text = GetComponent<TextMeshPro>();
            m_textColor = m_text.color;
        }

        private void Update()
        {
            if (m_text.text == "0")
                return;

            transform.position += new Vector3(0, m_textVerticalSpeed) * Time.deltaTime;
            m_textColor.a -= m_textFadingSpeed * Time.deltaTime;
            m_text.color = m_textColor;
        }

        public void Activate(float damage)
        {
            m_text.text = damage.ToString();
            m_textColor.a = 1;
        }
    }
}
