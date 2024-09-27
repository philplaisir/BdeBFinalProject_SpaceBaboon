using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon
{
    public class DropPointCerclePercent : MonoBehaviour
    {
        private float m_ressourcesTypeOne;
        private float m_ressourcesTypeTwo;
        private float m_ressourcesTypeTree;
        private float m_transparentCirclePercentage;
        Vector3 m_transparentCircleNewPosition;
        private bool m_transparentCircleMorphing;
        [SerializeField]
        private GameObject m_circle;
        // Start is called before the first frame update
        void Start()
        {
            // IDK
            m_ressourcesTypeOne = GameManager.Instance.Player.GetResources((int)0);
            m_ressourcesTypeTwo = GameManager.Instance.Player.GetResources((int)1);
            m_ressourcesTypeTree = GameManager.Instance.Player.GetResources((int)2);
        }
      
        // Update is called once per frame
        void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, m_transparentCircleNewPosition, Time.deltaTime * 1.0f);
        }

        private void SetCircleFillerPercent()
        {
            //m_transparentCirclePercentage = (float)m_ressourcesTypeOne / //GET_DROPPOINT_RESSOURCE_NEEDED * m_circle.transform.localScale.x;
            m_transparentCircleNewPosition = new Vector3(m_transparentCirclePercentage, m_transparentCirclePercentage, m_transparentCirclePercentage);
        }
    }
}
