
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon.UI_Toolkit
{
    public class HpBarUI : MonoBehaviour
    {
        private GameObject playerGameObject;
        private Player playerRef;
        private ProgressBar hpBar;
        private VisualElement root;
        private float lerpSpeed;
        
        private void OnEnable()
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            playerRef = playerGameObject.GetComponent<Player>();
            root = GetComponent<UIDocument>().rootVisualElement;
            hpBar = root.Q<ProgressBar>("HpBar");
            lerpSpeed = 10.0f;
        }

        private void Update()
        {
            float currentHealth = playerRef.GetCurrentHealth();
            //hpBar.value = playerRef.m_currentHealth;
            hpBar.value = Mathf.Lerp(hpBar.value, currentHealth, lerpSpeed * Time.deltaTime);
        }
            
        
    }
}
