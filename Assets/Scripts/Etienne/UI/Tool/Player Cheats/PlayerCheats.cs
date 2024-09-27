using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon
{
    public class PlayerCheats : MonoBehaviour
    {
        [SerializeField] private Player m_player;

        private UIDocument m_uiDoc;

        private bool m_displayBool;
        private Button m_displayButton;
        private VisualElement m_elementsToHide;

        private Toggle m_invincibilityToggle;
        private Button m_maxHealthButton;
        private Button m_killPlayerButton;
        private Slider m_speedSlider;
        private Toggle m_meleeToggle;
        private Toggle m_flameThrowerToggle;
        private Toggle m_grenadeLauncherToggle;
        private Toggle m_shockwaveToggle;
        private Toggle m_laserBeamToggle;


        private void Awake()
        {
            m_uiDoc = GetComponent<UIDocument>();
            VisualElement visualElement = m_uiDoc.rootVisualElement;

            m_displayButton = visualElement.Q<Button>("DisplayButton");
            m_elementsToHide = visualElement.Q<VisualElement>("ElementsToHide");
            m_elementsToHide.style.visibility = Visibility.Hidden;
            //m_elementsToHide.style.display = DisplayStyle.None;



            m_invincibilityToggle = visualElement.Q<Toggle>("InvincibilityToggle");
            m_maxHealthButton = visualElement.Q<Button>("MaxHealthButton");
            m_killPlayerButton = visualElement.Q<Button>("KillPlayerButton");
            m_speedSlider = visualElement.Q<Slider>("SpeedSlider");

            m_meleeToggle = visualElement.Q<Toggle>("MeleeToggle");
            m_flameThrowerToggle = visualElement.Q<Toggle>("FlameThrowerToggle");
            m_grenadeLauncherToggle = visualElement.Q<Toggle>("GrenadeLauncherToggle");
            m_shockwaveToggle = visualElement.Q<Toggle>("ShockwaveToggle");
            m_laserBeamToggle = visualElement.Q<Toggle>("LaserBeamToggle");

        }

        private void OnEnable()
        {
            m_displayButton.clicked += OnDisplayButtonClicked;


            m_invincibilityToggle.RegisterValueChangedCallback(OnInvincibilityToggled);
            m_maxHealthButton.clicked += OnMaxHealthButtonClicked;
            m_killPlayerButton.clicked += OnKillPlayerButtonClicked;
            m_speedSlider.RegisterValueChangedCallback(OnSpeedChanged);

            m_meleeToggle.RegisterValueChangedCallback(OnMeleeToggled);
            m_flameThrowerToggle.RegisterValueChangedCallback(OnFlameThrowerToggled);
            m_grenadeLauncherToggle.RegisterValueChangedCallback(OnGrenadeLauncherToggled);
            m_shockwaveToggle.RegisterValueChangedCallback(OnShockwaveToggled);
            m_laserBeamToggle.RegisterValueChangedCallback(OnLaserBeamToggled);
        }


        private void OnDisplayButtonClicked()
        {
            m_displayBool = !m_displayBool;

            if (m_displayBool)
            {
                m_elementsToHide.style.visibility = Visibility.Visible;
            }
            else
            {
                m_elementsToHide.style.visibility = Visibility.Hidden;
            }
        }


        private void OnInvincibilityToggled(ChangeEvent<bool> evt)
        {
            m_player.SetIsInvincible(evt.newValue);
        }

        private void OnMaxHealthButtonClicked()
        {
            m_player.SetCurrentHealthToMax();
        }

        private void OnKillPlayerButtonClicked()
        {
            m_player.KillPlayer();
        }

        private void OnSpeedChanged(ChangeEvent<float> evt)
        {
            m_player.SetSpeedWithMultiplier(evt.newValue);
        }

        //----------------------------------
        private void OnMeleeToggled(ChangeEvent<bool> evt)
        {
            m_player.SetWeaponStatus(WeaponSystem.EPlayerWeaponType.Melee, !evt.newValue);
        }

        private void OnFlameThrowerToggled(ChangeEvent<bool> evt)
        {
            m_player.SetWeaponStatus(WeaponSystem.EPlayerWeaponType.FlameThrower, !evt.newValue);
        }

        private void OnGrenadeLauncherToggled(ChangeEvent<bool> evt)
        {
            m_player.SetWeaponStatus(WeaponSystem.EPlayerWeaponType.GrenadeLauncher, !evt.newValue);
        }

        private void OnShockwaveToggled(ChangeEvent<bool> evt)
        {
            m_player.SetWeaponStatus(WeaponSystem.EPlayerWeaponType.Shockwave, !evt.newValue);
        }

        private void OnLaserBeamToggled(ChangeEvent<bool> evt)
        {
            m_player.SetWeaponStatus(WeaponSystem.EPlayerWeaponType.LaserBeam, !evt.newValue);
        }

    }
}
