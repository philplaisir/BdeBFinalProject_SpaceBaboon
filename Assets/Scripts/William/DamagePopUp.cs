using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace SpaceBaboon
{
    public class DamagePopUp : MonoBehaviour
    {
        private TextMeshPro m_popUpDmgText;
        private Color m_textColor;
        private float m_popUpDmgTextVerticalSpeed;
        private float m_popUpDmgTextFadingTimer;
        private float m_popUpDmgTextFadingSpeed;   
        
        public static DamagePopUp Create( Vector3 position, float damage)
        {
            // Cree le dmg
            Transform damagePopUpTransform = Instantiate(GameManager.Instance.m_dmgPopUpPrefab,position,Quaternion.identity);
            // Set ref
            DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
            damagePopUp.SetupDmgPopUp(damage);
            // return dmg
            return damagePopUp;
        }
        private void Awake()
        {
            m_popUpDmgText = transform.GetComponent<TextMeshPro>();
        }
       
        public void SetupDmgPopUp(float damageAmount)
        {
            m_popUpDmgText.text = damageAmount.ToString();
            m_textColor = m_popUpDmgText.color;
        }

        private void Update() 
        {
            m_popUpDmgTextVerticalSpeed = 20.0f;
            transform.position += new Vector3(0,m_popUpDmgTextVerticalSpeed) * Time.deltaTime;
            //Debug.Log("Timer is at : " + m_popUpDmgTextFadingTimer); 
            m_popUpDmgTextFadingTimer -= Time.deltaTime;
            if(m_popUpDmgTextFadingTimer < 0)
            {
                m_popUpDmgTextFadingSpeed = 1.5f;
                m_textColor.a -= m_popUpDmgTextFadingSpeed * Time.deltaTime;
                m_popUpDmgText.color = m_textColor;
                if(m_textColor.a < 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
