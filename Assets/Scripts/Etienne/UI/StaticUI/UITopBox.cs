using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon.UISystem
{
    [System.Serializable]
    public class UITopBox
    {
        //Current Upgrade Image
        [SerializeField] private Sprite m_damageSprite;
        [SerializeField] private Sprite m_rangeSprite;
        [SerializeField] private Sprite m_speedSprite;
        [SerializeField] private Sprite m_zoneSprite;
        
        private UIManager m_manager;
        private VisualElement m_root;

        private Button m_pauseButton;
        private VisualElement m_currentUpgrade;
        private Label m_metalAmount;
        private Label m_crystalAmount;
        private Label m_technologyAmount;
        //private Label m_healthAmount;
        private Label m_timer;

        public void Create(UIManager manager, VisualElement root)
        {
            m_manager = manager;
            m_root = root;

            m_pauseButton = root.Q<Button>("PauseButton");
            m_currentUpgrade = root.Q<VisualElement>("UpgradeImage");
            m_metalAmount = root.Q<Label>("MetalAmount");
            m_crystalAmount = root.Q<Label>("CrystalAmount");
            m_technologyAmount = root.Q<Label>("TechnologyAmount");
            //m_healthAmount = root.Q<Label>("HealthAmount");
            m_timer = root.Q<Label>("Timer");

            Enable();
        }

        public void UpdateResource(Crafting.InteractableResource.EResourceType resourceType, int amount)
        {
            switch (resourceType)
            {
                case Crafting.InteractableResource.EResourceType.Metal:
                    m_metalAmount.text = amount.ToString();
                    break;
                case Crafting.InteractableResource.EResourceType.Crystal:
                    m_crystalAmount.text = amount.ToString();
                    break;
                case Crafting.InteractableResource.EResourceType.Technologie:
                    m_technologyAmount.text = amount.ToString();
                    break;
                case Crafting.InteractableResource.EResourceType.Heart:
                    //
                    break;

                case Crafting.InteractableResource.EResourceType.Count:
                default:
                    break;
            }
        }

        public void UpdateCurrentUpgrade(Crafting.CraftingStation.EWeaponUpgrades upgradeType)
        {
            switch (upgradeType)
            {
                case Crafting.CraftingStation.EWeaponUpgrades.AttackSpeed:
                    m_currentUpgrade.style.backgroundImage = new StyleBackground(m_speedSprite);
                    break;
                
                case Crafting.CraftingStation.EWeaponUpgrades.AttackZone:
                    m_currentUpgrade.style.backgroundImage = new StyleBackground(m_zoneSprite);
                    break;
                
                case Crafting.CraftingStation.EWeaponUpgrades.AttackRange:
                    m_currentUpgrade.style.backgroundImage = new StyleBackground(m_rangeSprite);
                    break;
                
                case Crafting.CraftingStation.EWeaponUpgrades.AttackDamage:
                    m_currentUpgrade.style.backgroundImage = new StyleBackground(m_damageSprite);
                    break;

                case Crafting.CraftingStation.EWeaponUpgrades.Count:
                default:
                    break;
            }
        }

        public void UpdateTimer(int timer)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            m_timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        private void Enable()
        {

        }

        public void Disable()
        {

        }
    }
}
