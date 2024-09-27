using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SpaceBaboon.MenuSystem
{
    public class MenuManager : MonoBehaviour
    {
        private UIDocument m_uiDoc;

        private MainMenu m_mainMenuScript = new MainMenu();
        private HowToPlayMenu m_howToPlayMenuScript = new HowToPlayMenu();
        private SettingsMenu m_settingsMenuScript = new SettingsMenu();

        private VisualElement m_mainMenu;
        private VisualElement m_howToPlayMenu;
        private VisualElement m_settingsMenu;




        private void Awake()
        {
            m_uiDoc = GetComponent<UIDocument>();
            VisualElement visualElement = m_uiDoc.rootVisualElement;

            m_mainMenu = visualElement.Q<VisualElement>("MainMenuContainer");
            //m_mainMenuScript = GetComponent<MainMenu>();
            m_mainMenuScript.Create(m_mainMenu, this);

            m_howToPlayMenu = visualElement.Q<VisualElement>("HowToPlayMenuContainer");
            //m_howToPlayMenuScript = GetComponent<HowToPlayMenu>();
            m_howToPlayMenuScript.Create(m_howToPlayMenu, this);

            m_settingsMenu = visualElement.Q<VisualElement>("SettingsMenuContainer");
            //m_settingsMenuScript = GetComponent<SettingsMenu>();
            m_settingsMenuScript.Create(m_settingsMenu, this);

        }

        private void OnDisable()
        {
            m_mainMenuScript.Disable();
            m_howToPlayMenuScript.Disable();
            m_settingsMenuScript.Disable();
        }

        private void ChangeDisplay(VisualElement containerToActivate, VisualElement containerToDeactivate)
        {
            containerToDeactivate.style.display = DisplayStyle.None;
            containerToActivate.style.display = DisplayStyle.Flex;
        }

        public void OpenSettings()
        {
            ChangeDisplay(m_settingsMenu, m_mainMenu);
        }

        public void OpenHowToPlay()
        {
            ChangeDisplay(m_howToPlayMenu, m_mainMenu);
        }

        public void OpenMainMenu(VisualElement current)
        {
            ChangeDisplay(m_mainMenu, current);
        }
    }
}
