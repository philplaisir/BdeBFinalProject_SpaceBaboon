using SpaceBaboon.WeaponSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceBaboon.UISystem
{
    public class UIInfoBox
    {
        private UIManager m_manager;
        private VisualElement m_root;

        private VisualElement m_image;
        private Label m_name;

        private Label m_damageLevelDisplay;
        private Label m_speedLevelDisplay;
        private Label m_rangeLevelDisplay;
        private Label m_zoneLevelDisplay;

        public void Create(UIManager manager, VisualElement root)
        {
            m_manager = manager;
            m_root = root;

            m_image = root.Q<VisualElement>("InfoWeaponImage");
            m_name = root.Q<Label>("InfoWeaponName");

            m_damageLevelDisplay = root.Q<Label>("DamageSkillLevel");
            m_speedLevelDisplay = root.Q<Label>("SpeedSkillLevel");
            m_rangeLevelDisplay = root.Q<Label>("RangeSkillLevel");
            m_zoneLevelDisplay = root.Q<Label>("ZoneSkillLevel");

            Enable();
        }

        public void DisplayInfoBox(WeaponBoxData data)
        {
            StyleBackground bg = new StyleBackground(data.image);
            m_image.style.backgroundImage = bg;

            m_name.text = data.name;

            m_damageLevelDisplay.text = data.damageLevel.ToString();
            m_speedLevelDisplay.text = data.speedLevel.ToString();
            m_rangeLevelDisplay.text = data.rangeLevel.ToString();
            m_zoneLevelDisplay.text = data.zoneLevel.ToString();

            //m_root.style.display = DisplayStyle.Flex;
            m_root.style.visibility = Visibility.Visible;
        }

        public void HideInfoBox()
        {
            //m_root.style.display = DisplayStyle.None;
            m_root.style.visibility = Visibility.Hidden;
        }


        private void Enable()
        {

        }

        public void Disable()
        {

        }
    }
}
