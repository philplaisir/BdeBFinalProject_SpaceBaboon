using UnityEngine;
using UnityEngine.UIElements;
using SpaceBaboon.EnemySystem;
using System;

namespace SpaceBaboon
{
    public class EnemyCheats : MonoBehaviour
    {
        [SerializeField] private EnemySpawner m_spawner;

        private UIDocument m_uiDoc;

        private bool m_displayBool;
        private Button m_displayButton;
        private VisualElement m_elementsToHide;

        private Toggle m_spawningToggle;
        private Button m_spawnBossButton;
        private Slider m_delaySlider;

        private Toggle m_meleeToggle;
        private IntegerField m_meleeAmount;
        private Button m_meleeSpawn;

        private Toggle m_shootingToggle;
        private IntegerField m_shootingAmount;
        private Button m_shootingSpawn;

        private Toggle m_kamikazeToggle;
        private IntegerField m_kamikazeAmount;
        private Button m_kamikazeSpawn;




        private void Awake()
        {
            m_uiDoc = GetComponent<UIDocument>();
            VisualElement visualElement = m_uiDoc.rootVisualElement;

            m_displayButton = visualElement.Q<Button>("DisplayButton");
            m_elementsToHide = visualElement.Q<VisualElement>("ElementsToHide");
            m_elementsToHide.style.visibility = Visibility.Hidden;

            m_spawningToggle = visualElement.Q<Toggle>("SpawningToggle");
            m_spawnBossButton = visualElement.Q<Button>("SpawnBossButton");
            m_delaySlider = visualElement.Q<Slider>("SpawningDelay");
            InitDelaySliderValue();

            m_meleeToggle = visualElement.Q<Toggle>("MeleeToggle");
            m_meleeAmount = visualElement.Q<IntegerField>("MeleeSpawnAmount");
            m_meleeSpawn = visualElement.Q<Button>("MeleeSpawnButton");

            m_shootingToggle = visualElement.Q<Toggle>("ShootingToggle");
            m_shootingAmount = visualElement.Q<IntegerField>("ShootingSpawnAmount");
            m_shootingSpawn = visualElement.Q<Button>("ShootingSpawnButton");

            m_kamikazeToggle = visualElement.Q<Toggle>("KamikazeToggle");
            m_kamikazeAmount = visualElement.Q<IntegerField>("KamikazeSpawnAmount");
            m_kamikazeSpawn = visualElement.Q<Button>("KamikazeSpawnButton");


        }

        private void OnEnable()
        {
            m_displayButton.clicked += OnDisplayButtonClicked;

            m_spawningToggle.RegisterValueChangedCallback(OnSpawningToggled);
            m_spawnBossButton.clicked += OnSpawnBossButtonClicked;
            m_delaySlider.RegisterValueChangedCallback(OnDelayChanged);

            m_meleeToggle.RegisterValueChangedCallback(OnMeleeToggled);
            m_meleeSpawn.clicked += OnSpawnMeleeButtonClicked;

            m_shootingToggle.RegisterValueChangedCallback(OnShootingToggled);
            m_shootingSpawn.clicked += OnSpawnShootingButtonClicked;

            m_kamikazeToggle.RegisterValueChangedCallback(OnKamikazeToggled);
            m_kamikazeSpawn.clicked += OnSpawnKamikazeButtonClicked;

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

        private void OnSpawningToggled(ChangeEvent<bool> evt)
        {
            m_spawner.SetIsSpawning(evt.newValue);
        }

        private void OnSpawnBossButtonClicked()
        {
            throw new NotImplementedException();
        }

        //---------------------------------
        private void InitDelaySliderValue()
        {
            float startingValue = m_spawner.GetDelay();
            if (startingValue < 0)
            {
                Debug.LogError("Spawning Delay cannot be negative");
                return;
            }
            if (startingValue > m_delaySlider.highValue)
            {
                m_delaySlider.highValue = startingValue;
            }

            m_delaySlider.value = startingValue;
        }

        private void OnDelayChanged(ChangeEvent<float> evt)
        {
            m_spawner.SetDelay(evt.newValue);
        }

        //---------------------------------
        private void OnMeleeToggled(ChangeEvent<bool> evt)
        {
            m_spawner.ToggleSpawnByEnemyType(EEnemyTypes.Melee, evt.newValue);
        }
        private void OnSpawnMeleeButtonClicked()
        {
            int amount = m_meleeAmount.value;
            m_spawner.CheatSpawnGroup(EEnemyTypes.Melee, amount);
        }

        //---------------------------------
        private void OnShootingToggled(ChangeEvent<bool> evt)
        {
            m_spawner.ToggleSpawnByEnemyType(EEnemyTypes.Shooting, evt.newValue);
        }

        private void OnSpawnShootingButtonClicked()
        {
            int amount = m_shootingAmount.value;
            m_spawner.CheatSpawnGroup(EEnemyTypes.Shooting, amount);
        }

        //---------------------------------
        private void OnKamikazeToggled(ChangeEvent<bool> evt)
        {
            m_spawner.ToggleSpawnByEnemyType(EEnemyTypes.Kamikaze, evt.newValue);
        }
        private void OnSpawnKamikazeButtonClicked()
        {
            int amount = m_kamikazeAmount.value;
            m_spawner.CheatSpawnGroup(EEnemyTypes.Kamikaze, amount);
        }
    }
}
