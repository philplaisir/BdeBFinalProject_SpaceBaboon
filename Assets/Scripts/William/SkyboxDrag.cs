using System;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;


namespace SpaceBaboon
{
    public class SkyboxDrag : MonoBehaviour
    {
        [SerializeField] private Image m_skyboxImage;
        private Image m_skyboxImageClone;
        [SerializeField] private Vector2 m_imageDragVector;

        private void Start()
        {
            m_skyboxImage = GetComponent<Image>();
            m_skyboxImage.material = new Material(m_skyboxImage.material);
        }

        private void FixedUpdate()
        {
            m_skyboxImage.material.mainTextureOffset += m_imageDragVector * Time.deltaTime;
        }
    }
}
