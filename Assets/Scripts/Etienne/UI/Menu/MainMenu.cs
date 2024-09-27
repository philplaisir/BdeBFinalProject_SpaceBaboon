using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SpaceBaboon.MenuSystem
{
    public class MainMenu
    {
        private MenuManager m_manager;

        private VisualElement m_root;

        private Button m_startButton;
        private Button m_howToPlayButton;
        private Button m_settingsButton;
        private Button m_quitButton;


        public void Create(VisualElement visualElement, MenuManager manager)
        {
            m_root = visualElement;
            m_manager = manager;

            m_startButton = m_root.Q<Button>("StartButton");
            m_howToPlayButton = m_root.Q<Button>("HowToPlayButton");
            m_settingsButton = m_root.Q<Button>("SettingsButton");
            m_quitButton = m_root.Q<Button>("QuitButton");

            Enable();
        }

        private void Enable()
        {
            m_startButton.clicked += StartGame;
            m_howToPlayButton.clicked += OpenHowToPlay;
            m_settingsButton.clicked += OpenSettings;
            m_quitButton.clicked += QuitGame;

        }

        public void Disable()
        {
            m_startButton.clicked -= StartGame;
            m_howToPlayButton.clicked -= OpenHowToPlay;
            m_settingsButton.clicked -= OpenSettings;
            m_quitButton.clicked -= QuitGame;

        }

        private void StartGame()
        {
            //Debug.Log("Start Game");

            //SceneManager.LoadScene("SB_Build3");
            GameManager.Instance.StartGame();
        }

        private void OpenHowToPlay()
        {
            m_manager.OpenHowToPlay();
        }

        private void OpenSettings()
        {
            m_manager.OpenSettings();
        }

        private void QuitGame()
        {
            Application.Quit();
            //Debug.Log("Quit Game");
        }

    }
}
