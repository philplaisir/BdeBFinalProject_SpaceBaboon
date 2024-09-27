using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon.MenuSystem
{
    public class HowToPlayMenu
    {
        private MenuManager m_manager;

        private VisualElement m_root;

        private Button m_backButton;


        public void Create(VisualElement visualElement, MenuManager manager)
        {
            m_root = visualElement;
            m_manager = manager;

            m_backButton = m_root.Q<Button>("BackButton");

            Enable();
        }

        private void Enable()
        {
            m_backButton.clicked += BackToMainMenu;
        }

        public void Disable()
        {
            m_backButton.clicked -= BackToMainMenu;
        }


        private void BackToMainMenu()
        {
            m_manager.OpenMainMenu(m_root);
        }
    }
}
