using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon.UISystem
{
    public class GameStaticUI : MonoBehaviour
    {
        private Player m_player;
        private UIDocument m_uiDoc;

        private bool m_displayBool = false;

        private Label m_crystalAmount;
        private Label m_technologyAmount;
        private Label m_metalAmount;

        private Label m_currentUpgrade;
        private Label m_gameTimer;

        private Button m_pauseButton;
        private VisualElement m_pauseMenu;
        private PauseMenu m_pauseMenuScript = new PauseMenu();


        private void Awake()
        {
            
            m_uiDoc = GetComponent<UIDocument>();
            VisualElement visualElement = m_uiDoc.rootVisualElement;

            m_crystalAmount = visualElement.Q<Label>("CrystalRessourceAmount");
            m_technologyAmount = visualElement.Q<Label>("TechnologyRessourceAmount");
            m_metalAmount = visualElement.Q<Label>("MetalRessourceAmount");

            m_currentUpgrade = visualElement.Q<Label>("UpgradeName");
            m_gameTimer = visualElement.Q<Label>("TimerAmount");

            m_pauseButton = visualElement.Q<Button>("PauseButton");
            m_pauseMenu = visualElement.Q<VisualElement>("PauseMenu");
            //m_pauseMenuScript.Create(visualElement);

        }

        private void Start()
        {
            m_player = GameManager.Instance.Player;
        }

        private void Update()
        {
            if (m_player == null)
            {
                Debug.Log(GameManager.Instance.Player);
            }
            m_metalAmount.text = m_player.GetResources(0).ToString();
            m_crystalAmount.text = m_player.GetResources(1).ToString();
            m_technologyAmount.text = m_player.GetResources(2).ToString();

            m_currentUpgrade.text = Crafting.CraftingStation.CurrentUpgrade.ToString();
            int time = (int)GameManager.Instance.GameTimer;
            m_gameTimer.text = time.ToString();
        }

        private void OnEnable()
        {
            m_pauseButton.clicked += OnPauseButtonClicked;
        }

        private void OnDisable()
        {
            m_pauseButton.clicked -= OnPauseButtonClicked;
            m_pauseMenuScript.Disable();
        }

        private void OnPauseButtonClicked()
        {
            m_displayBool = !m_displayBool;

            if (m_displayBool)
            {
                m_pauseMenu.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_pauseMenu.style.display = DisplayStyle.None;
            }

            GameManager.Instance.PauseGame(m_displayBool);
        }
    }
}
