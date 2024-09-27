using SpaceBaboon.WeaponSystem;
using System;
using UnityEngine;
using UnityEngine.UIElements;
//using static Cinemachine.DocumentationSortingAttribute;

namespace SpaceBaboon.UISystem
{
    [System.Serializable]
    public class UIWeaponBox
    {
        [SerializeField] private Sprite m_swordSprite;
        [SerializeField] private Sprite m_flameThrowerSprite;
        [SerializeField] private Sprite m_grenadeLauncherSprite;
        [SerializeField] private Sprite m_shockwaveSprite;
        [SerializeField] private Sprite m_laserBeamSprite;

        private UIManager m_manager;
        private UIInfoBox m_infoBoxScript;

        private VisualElement m_root;

        private VisualElement m_swordBox;
        private VisualElement m_flameThrowerBox;
        private VisualElement m_grenadeLauncherBox;
        private VisualElement m_shockwaveBox;
        private VisualElement m_laserBeamBox;


        private WeaponBoxData[] m_weaponsData = new WeaponBoxData[5];

        public void Create(UIManager manager, VisualElement root, UIInfoBox infoBoxScript)
        {
            m_manager = manager;
            m_infoBoxScript = infoBoxScript;

            m_root = root;

            m_swordBox = root.Q<VisualElement>("SwordBox");
            m_weaponsData[0] = new WeaponBoxData(m_swordBox, "BANANA\nSWORD", m_swordSprite);
            m_flameThrowerBox = root.Q<VisualElement>("FlameThrowerBox");
            m_weaponsData[1] = new WeaponBoxData(m_flameThrowerBox, "FLAME\nTHROWER", m_flameThrowerSprite);
            m_grenadeLauncherBox = root.Q<VisualElement>("GrenadeLauncherBox");
            m_weaponsData[2] = new WeaponBoxData(m_grenadeLauncherBox, "COCONUT\nLAUNCHER", m_grenadeLauncherSprite);
            m_shockwaveBox = root.Q<VisualElement>("ShockwaveBox");
            m_weaponsData[3] = new WeaponBoxData(m_shockwaveBox, "SHOCKWAVE\nBLASTER", m_shockwaveSprite);
            m_laserBeamBox = root.Q<VisualElement>("LaserBeamBox");
            m_weaponsData[4] = new WeaponBoxData(m_laserBeamBox, "LASER\nBEAM", m_laserBeamSprite);


            Enable();
        }

        public void UpdateWeapon(EPlayerWeaponType type, int totalLevel, int dLevel, int sLevel, int rLevel, int zLevel)
        {
            m_weaponsData[(int)type].SetLevels(totalLevel, dLevel, sLevel, rLevel, zLevel);
        }



        private void Enable()
        {
            m_swordBox.RegisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxSword);
            m_flameThrowerBox.RegisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxFlameThrower);
            m_grenadeLauncherBox.RegisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxGrenadeLauncher);
            m_shockwaveBox.RegisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxShockwave);
            m_laserBeamBox.RegisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxLaserBeam);

            m_swordBox.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
            m_flameThrowerBox.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
            m_grenadeLauncherBox.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
            m_shockwaveBox.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
            m_laserBeamBox.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
        }


        #region OnPointerEnter

        private void OnPointerEnterDisplayInfoBoxSword(PointerEnterEvent evt)
        {
            m_infoBoxScript.DisplayInfoBox(m_weaponsData[(int)EPlayerWeaponType.Melee]);
        }

        private void OnPointerEnterDisplayInfoBoxFlameThrower(PointerEnterEvent evt)
        {
            m_infoBoxScript.DisplayInfoBox(m_weaponsData[(int)EPlayerWeaponType.FlameThrower]);
        }

        private void OnPointerEnterDisplayInfoBoxGrenadeLauncher(PointerEnterEvent evt)
        {
            m_infoBoxScript.DisplayInfoBox(m_weaponsData[(int)EPlayerWeaponType.GrenadeLauncher]);
        }

        private void OnPointerEnterDisplayInfoBoxShockwave(PointerEnterEvent evt)
        {
            m_infoBoxScript.DisplayInfoBox(m_weaponsData[(int)EPlayerWeaponType.Shockwave]);
        }

        private void OnPointerEnterDisplayInfoBoxLaserBeam(PointerEnterEvent evt)
        {
            m_infoBoxScript.DisplayInfoBox(m_weaponsData[(int)EPlayerWeaponType.LaserBeam]);
        }

        #endregion


        private void OnPointerLeaveHideInfoBox(PointerLeaveEvent evt)
        {
            m_infoBoxScript.HideInfoBox();
        }



        public void Disable()
        {
            m_swordBox.UnregisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxSword);
            m_flameThrowerBox.UnregisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxFlameThrower);
            m_grenadeLauncherBox.UnregisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxGrenadeLauncher);
            m_shockwaveBox.UnregisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxShockwave);
            m_laserBeamBox.UnregisterCallback<PointerEnterEvent>(OnPointerEnterDisplayInfoBoxLaserBeam);

            m_swordBox.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
            m_flameThrowerBox.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
            m_grenadeLauncherBox.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
            m_shockwaveBox.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
            m_laserBeamBox.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveHideInfoBox);
        }


    }

    public struct WeaponBoxData
    {
        public WeaponBoxData(VisualElement vE, string n, Sprite s)
        {
            visualElement = vE;
            totallevelDisplay = visualElement.Q<Label>("WeaponLevel");
            totalLevel = 1;

            name = n;
            image = s;

            damageLevel = 0;
            speedLevel = 0;
            rangeLevel = 0;
            zoneLevel = 0;

        }

        public void SetLevels(int tLevel, int dLevel, int sLevel, int rLevel, int zLevel)
        {
            totalLevel = tLevel;
            totallevelDisplay.text = totalLevel.ToString();

            damageLevel = dLevel;
            speedLevel = sLevel;
            rangeLevel = rLevel;
            zoneLevel = zLevel;
        }

        public VisualElement visualElement;
        public Label totallevelDisplay;
        public int totalLevel;

        public Sprite image;
        public string name;

        public int damageLevel;
        public int speedLevel;
        public int rangeLevel;
        public int zoneLevel;
    }
}
