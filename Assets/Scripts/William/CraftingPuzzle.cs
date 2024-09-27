using SpaceBaboon.Crafting;
using SpaceBaboon.EnemySystem;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon
{
    public class CraftingPuzzle : MonoBehaviour
    {
        [SerializeField]
        private bool m_craftingPuzzleEnabled;
        [SerializeField]
        public int m_killneeded;
        [SerializeField]
        private int m_currentKill;
        [SerializeField]
        private List<ResourceDropPoint> m_dropPointList;        
        private CraftingStation m_craftingStationScript;
        [SerializeField]
        private ResourceDropPoint m_ressourceDropPointScript;
        private GameObject m_zoneCircle;
        [SerializeField]
        private GameObject m_blueCircle;
        [SerializeField]
        private GameObject m_circleMask;
        [SerializeField]
        private GameObject m_blueCircleFiller;        

        private float m_transparentCirclePercentage;
        Vector3 m_transparentCircleNewPosition;
        private bool m_transparentCircleMorphing;

        private float m_ressourcesTypeOne;

        public void Initialisation()
        {
            m_craftingStationScript = GetComponent<CraftingStation>();
            m_ressourceDropPointScript = GetComponentInChildren<ResourceDropPoint>();            
            m_dropPointList = m_craftingStationScript.GetDropPopint();            
            m_zoneCircle = GameObject.Find("Circle");
            //SetDropPoints();
            m_transparentCirclePercentage = 0.0f;
            m_transparentCircleNewPosition = new Vector3();
            m_transparentCircleMorphing = false;
        }

        void Update()
        {
            if(!m_craftingPuzzleEnabled)
            {
                return;
            }

            UpdatePuzzleState(); 

            //CircleLerping();
            m_blueCircleFiller.transform.localScale = Vector3.Lerp(m_blueCircleFiller.transform.localScale, m_transparentCircleNewPosition, Time.deltaTime * 1.0f);
            //m_blueCircleFiller.transform.localScale = m_transparentCircleNewPosition;
        }

        private void SetDropPoints()
        {
            foreach (var dropPoint in m_dropPointList)
            {
                dropPoint.gameObject.SetActive(false);
            }
        }

        private void UpdatePuzzleState()
        {
            if (m_currentKill >= m_killneeded)
            {
                m_craftingStationScript.ReceivePuzzleCompleted();
            }
        }

        public void PuzzleCounter()
        {
            if (m_craftingPuzzleEnabled == true)
            {
                m_currentKill += 1;
                m_transparentCirclePercentage = (float)m_currentKill / m_killneeded * m_blueCircle.transform.localScale.x;
                m_transparentCircleNewPosition = new Vector3(m_transparentCirclePercentage, m_transparentCirclePercentage, m_transparentCirclePercentage);
                m_transparentCircleMorphing = true;
            }
        }

        public void CircleLerping()
        {
            if (m_transparentCircleMorphing == true)
            {
                m_blueCircleFiller.transform.localScale = m_transparentCircleNewPosition;
                //Vector3.Lerp(m_blueCircleFiller.transform.localScale, m_transparentCircleNewPosition, Time.deltaTime * 2.0f);
                m_transparentCircleMorphing = false;
            }
        }

        public void SetPuzzle(bool value)
        {
            m_craftingPuzzleEnabled = value;            
            m_blueCircle.gameObject.SetActive(value);
            m_circleMask.gameObject.SetActive(value);
            m_blueCircleFiller.gameObject.SetActive(value);

            if (value)
            {
                m_currentKill = 0;
            }
        }

        private void OnEnemyDeathSubsribe(GameObject collider)
        {
            collider.GetComponent<Enemy>().registerPuzzle(this);
        }
        private void OnEnemyDeathUnsubscribe(GameObject collider)
        {
            collider.GetComponent<Enemy>().UnregisterPuzzle(this);
        }
        private void OnEnemyDetected(GameObject collider)
        {
            if (m_craftingPuzzleEnabled)
            {
                OnEnemyDeathSubsribe(collider);
            }
        }
        private void OnEnemyExit(GameObject collider)
        {
            if (m_craftingPuzzleEnabled)
            {
                OnEnemyDeathUnsubscribe(collider);
            }
        }
    }
}
