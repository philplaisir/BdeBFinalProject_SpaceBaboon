using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpaceBaboon
{
    //[ExecuteAlways]
    public class ShaderController : MonoBehaviour
    {
        private Material m_playerSpriteRendererMaterial;
        [SerializeField]
        private Color m_color;
        [SerializeField]
        private float m_horizontal;
        [SerializeField]
        private float m_veritcal;

        // Start is called before the first frame update

        void Start()
        {
            m_playerSpriteRendererMaterial = GetComponent<SpriteRenderer>().sharedMaterial;
            SetOutlineColor(m_color);
            //SetOutlineHorizontal(m_horizontal);
            //SetOutlineVertical(m_veritcal);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void SetOutlineColor(Color newcolor)
        {
            m_playerSpriteRendererMaterial.SetColor("_Color", newcolor);
        }
        private void SetOutlineHorizontal(float horizontal)
        {
            m_playerSpriteRendererMaterial.SetFloat("_Horizontal_Thickness", horizontal);
        }
        private void SetOutlineVertical(float vertical)
        {
            m_playerSpriteRendererMaterial.SetFloat("_Vertical_Thikness", vertical);
        }

    }
}
