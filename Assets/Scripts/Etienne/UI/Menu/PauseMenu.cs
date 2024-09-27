using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon.UISystem
{
    public class PauseMenu
    {
        private VisualElement m_root;

        private Toggle m_tutorialPopUpsToggle;
        private Slider m_volumeSlider;

        private Button m_endGameButton;

        public void Create(VisualElement visualElement)
        {
            m_root = visualElement;

            //m_tutorialPopUpsToggle = m_root.Q<Toggle>("TutorialPopUpsToggle");
            //Set value

            //m_volumeSlider = m_root.Q<Slider>("VolumeSlider");
            //m_volumeSlider.value = AudioListener.volume;

            m_endGameButton = m_root.Q<Button>("EndGameButton");

            Enable();
        }

        private void Enable()
        {
            //m_tutorialPopUpsToggle.RegisterValueChangedCallback(OnPopUpsToggled);
            //m_volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);

            m_endGameButton.clicked += OnEndGameButtonClicked;
        }

        public void Disable()
        {
            //m_tutorialPopUpsToggle.UnregisterValueChangedCallback(OnPopUpsToggled);
            //m_volumeSlider.UnregisterValueChangedCallback(OnVolumeChanged);

            m_endGameButton.clicked -= OnEndGameButtonClicked;
        }

        private void OnPopUpsToggled(ChangeEvent<bool> evt)
        {
            throw new NotImplementedException();
        }

        private void OnVolumeChanged(ChangeEvent<float> evt)
        {
            AudioListener.volume = evt.newValue;
        }

        private void OnEndGameButtonClicked()
        {
            GameManager.Instance.EndGame();
            m_root.style.display = DisplayStyle.None;
        }
    }
}
