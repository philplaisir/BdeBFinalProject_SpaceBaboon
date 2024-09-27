using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon.UI_Toolkit
{
    public class InPlayUI : MonoBehaviour
    {
        private GameObject playerGameObject;
        private Player playerRef;
        private Label ressourceone;
        private Label ressourcetwo;
        private Label ressourcetree;

        private VisualElement root;


        private void OnEnable()
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            playerRef = playerGameObject.GetComponent<Player>();
            root = GetComponent<UIDocument>().rootVisualElement;
            ressourceone = root.Q<Label>("ressourcevaluemetal");
            ressourcetwo = root.Q<Label>("ressourcevaluecrystal");
            ressourcetree = root.Q<Label>("ressourcevaluetechno");
        }

        private void Update()
        {
            //hpBar.value = playerRef.m_currentHealth;
            if (playerRef != null)
            {
                ressourceone.text = playerRef.GetResources(0).ToString();
                ressourcetwo.text = playerRef.GetResources(1).ToString();
                ressourcetree.text = playerRef.GetResources(2).ToString();
            }


        }


    }
}