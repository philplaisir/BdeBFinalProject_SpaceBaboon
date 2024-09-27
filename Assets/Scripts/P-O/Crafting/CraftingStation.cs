using SpaceBaboon.WeaponSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SpaceBaboon.Crafting
{
    public class CraftingStation : MonoBehaviour
    {
        //Serializable variables
        [SerializeField] private PlayerWeapon m_linkedWeapon;
        [SerializeField] public int m_maxHealth;
        [SerializeField] private List<ResourceDropPoint> m_resourceDropPoints = new List<ResourceDropPoint>();
        [SerializeField] private float m_levelScaling;
        [SerializeField] private bool m_debugMode;
        [SerializeField] private float m_maxUpgradeCooldown;
        [SerializeField] private SpriteRenderer m_weaponIcon;
        [SerializeField] private Sprite m_enabledStationSprite;
        [SerializeField] private Sprite m_disabledStationSprite;
        [SerializeField] private Light2D m_light2D;
        [SerializeField] private SpriteRenderer m_craftingStationRenderer;
        //Private variables
        private Transform m_position;
        public int m_currentHealth;
        private int m_currentStationLevel;
        [SerializeField] private bool m_isEnabled = false;
        private CraftingPuzzle m_puzzleScript;
        private SpriteRenderer m_stationRenderer;

        //Serialized for test purpose
        [SerializeField] private List<Crafting.InteractableResource.EResourceType> m_resourceNeeded = new List<Crafting.InteractableResource.EResourceType>();
        [SerializeField] private List<Crafting.InteractableResource.EResourceType> m_currentResources = new List<Crafting.InteractableResource.EResourceType>();
        [SerializeField] private List<Crafting.InteractableResource.EResourceType> m_possibleResources = new List<Crafting.InteractableResource.EResourceType>();
        [SerializeField] private float m_currentUpgradeCD = 0.0f;
        [SerializeField] private bool m_isUpgrading = false;

        //Static variables
        //static Upgrade currentUpgrade;
        static List<CraftingStation> m_craftingStationsList = new List<CraftingStation>();
        static List<EWeaponUpgrades> m_lastsUpgrades = new List<EWeaponUpgrades>();
        static EWeaponUpgrades m_currentUpgrade = EWeaponUpgrades.Count;

        private static bool s_unlockPopUpHasBeenCalled = false;

        public static EWeaponUpgrades CurrentUpgrade { get { return m_currentUpgrade; } }

        private void Awake()
        {
            m_craftingStationsList.Add(this);
        }
        private void OnDestroy()
        {
            m_craftingStationsList.Remove(this);
        }
        void Start()
        {
            Initialization();
        }

        void Update()
        {
            // TODO FOR TESTING TO DELETE
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SetCraftingStation(false);
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                SetCraftingStation(true);
            }

            if (!m_isEnabled)
            {
                //Debug.Log("Station disabled");
                return;
            }

            if (m_isUpgrading)
            {
                m_currentUpgradeCD -= Time.deltaTime;

                if (m_currentUpgradeCD < 0.0f)
                {
                    ResetDropStation();
                }
            }
        }

        private void SetCraftingStation(bool value) // true is enabled
        {
            m_isEnabled = value;

            if (value) // Enabled
            {
                ResourceNeededAllocation();
                m_currentHealth = m_maxHealth;
                m_stationRenderer.sprite = m_enabledStationSprite;
                m_craftingStationRenderer.sprite = m_enabledStationSprite;
                m_light2D.color = Color.green;

                m_puzzleScript.SetPuzzle(false);


                if (!s_unlockPopUpHasBeenCalled)
                {
                    GameManager.Instance.DisplayTutorialWindow(TutorialSystem.ETutorialType.CraftingStationUnlocked, transform.position);
                    s_unlockPopUpHasBeenCalled = true;
                }
                return;
            }

            foreach (var dropPoint in m_resourceDropPoints)
            {
                dropPoint.SetDisableDropPoint();
            }
            m_stationRenderer.sprite = m_disabledStationSprite;
            m_craftingStationRenderer.sprite = m_disabledStationSprite;
            m_light2D.color = Color.red;
            m_puzzleScript.SetPuzzle(true);
        }

        public void ReceivePuzzleCompleted()
        {
            SetCraftingStation(true);
        }

        public static List<CraftingStation> GetCraftingStations()
        {
            if (m_craftingStationsList.Count == 0)
            {
                Debug.Log("m_craftingStationsList is empty");
            }
            return m_craftingStationsList;
        }

        public List<ResourceDropPoint> GetDropPopint()
        {
            return m_resourceDropPoints;
        }

        public bool ResourceIsNeeded(Crafting.InteractableResource.EResourceType resourceToCheck)
        {
            return m_resourceNeeded.Contains(resourceToCheck);
        }

        public void ReceiveDamage(float damage)
        {
            m_currentHealth -= (int)damage;
            if (m_currentHealth <= 0)
            {
                SetCraftingStation(false);
            }
        }

        public bool GetIsEnabled() { return m_isEnabled; }

        #region StationManagement
        private void Initialization()
        {
            m_stationRenderer = GetComponent<SpriteRenderer>();
            m_currentStationLevel = 1;
            ResourceNeededAllocation();
            if (m_currentUpgrade == EWeaponUpgrades.Count)
            {
                ResetUpgrade();
            }
            m_puzzleScript = GetComponent<CraftingPuzzle>();
            m_puzzleScript.Initialisation();
            SetCraftingStation(false);
        }

        private void ResetDropStation()
        {
            m_isUpgrading = false;
            //Set Green Color;
            m_light2D.color = Color.green;
            ResetPossibleResourceList();
            ResourceNeededAllocation();
        }

        public void StationSetup(WeaponSystem.PlayerWeapon weapon)
        {
            m_linkedWeapon = weapon;
            //Debug.Log("linkedweapon to show is " + m_linkedWeapon);
            m_weaponIcon.sprite = weapon.GetComponent<SpriteRenderer>().sprite;
        }

        #endregion

        #region UpgradeManagement
        private void CheckIfUpgradable()
        {
            if (m_resourceNeeded.Count == 0)
            {
                if (m_debugMode) { Debug.Log("CrafingStation " + gameObject.name + " is upgrading weapon"); }

                LocalStationUpgrading();
                ResetUpgrade();

                FXSystem.FXManager fxManager = FXSystem.FXManager.Instance;
                if (fxManager != null)
                {
                    fxManager.PlayAudio(FXSystem.ESFXType.WeaponUpgrading);
                }
            }
        }

        private void LocalStationUpgrading()
        {
            m_linkedWeapon.Upgrade(m_currentUpgrade);
            m_currentResources.Clear();
            m_isUpgrading = true;
            // change to upgrade color
            m_light2D.color = Color.yellow;
            m_currentUpgradeCD = m_maxUpgradeCooldown;
            m_currentStationLevel++;
            m_lastsUpgrades.Add(m_currentUpgrade);
        }

        private void LastUpgradesCheck()
        {
            bool isChoosingUpgrade = true;
            EWeaponUpgrades newUpgrade = ResetUpgrade();

            while (isChoosingUpgrade)
            {
                if (CheckLastTwoUpgrades(newUpgrade))
                {
                    newUpgrade = ResetUpgrade();
                }
                else
                {
                    isChoosingUpgrade = false;
                }
            }
        }

        private bool CheckLastTwoUpgrades(EWeaponUpgrades newUpgrade)
        {
            if (m_lastsUpgrades.Count < 2)
            {
                return false;
            }

            bool isUpgradeValid = (m_lastsUpgrades.Count >= 2 &&
                                   m_lastsUpgrades[0] != newUpgrade &&
                                   m_lastsUpgrades[1] != newUpgrade);

            //Check if the list countain more than two upgrades
            if (m_lastsUpgrades.Count >= 2)
            {
                m_lastsUpgrades.RemoveAt(0);
            }
            m_lastsUpgrades.Add(newUpgrade);

            return isUpgradeValid;
        }

        static EWeaponUpgrades ResetUpgrade()
        {
            //Debug.Log("Chosen upgrade is " + m_currentUpgrade);
            m_currentUpgrade = (EWeaponUpgrades)Random.Range(0, (int)EWeaponUpgrades.Count);

            UISystem.UIManager uiManager = UISystem.UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.UpdateCurrentUpgrade(m_currentUpgrade);
            }

            return m_currentUpgrade;
        }
        #endregion

        #region ResourceManagement
        public bool AddResource(Crafting.InteractableResource.EResourceType resourceType)
        {
            //Check if the resource is needed
            if (m_resourceNeeded.Contains(resourceType))
            {
                m_currentResources.Add(resourceType);
                m_currentResources.Sort();
                m_resourceNeeded.Remove(resourceType);
                CheckIfUpgradable();

                if (m_debugMode)
                {
                    Debug.Log("AddResource called on " + gameObject.name);
                    foreach (Crafting.InteractableResource.EResourceType resource in m_currentResources)
                    {
                        Debug.Log("For crafting station " + gameObject.name + " there is a " + resource);
                    }
                }

                return true;
            }
            return false;
        }

        private void ResetPossibleResourceList()
        {
            m_possibleResources.Clear();
            for (int i = 0; i != (int)Crafting.InteractableResource.EResourceType.Count; i++)
            {
                if (m_debugMode) { Debug.Log("Added to m_possibleResource : " + (Crafting.InteractableResource.EResourceType)i); }
                m_possibleResources.Add((Crafting.InteractableResource.EResourceType)i);
            }
        }

        private void ResourceNeededAllocation()
        {
            //Clear resource needed before allocation
            m_resourceNeeded.Clear();

            //Reset possible resources list
            ResetPossibleResourceList();

            //Variables needed for while loop
            int amountOfResourceNeeded = m_currentStationLevel * (int)m_levelScaling;
            int currentResourceAllocation;
            int initialResourceIndex;
            //int dropPointIndex = 0;
            //int whileIterations = 0;

            //Choose first resource to add
            initialResourceIndex = Random.Range(0, m_possibleResources.Count);

            for (int i = 0; i < m_resourceDropPoints.Count; i++)
            {
                //Use modulo to have the correct resource index no matter the iteration order
                int resourceIndex = (initialResourceIndex + i) % ((int)InteractableResource.EResourceType.Count - 1);

                //Randomly select an amount to give to the drop point
                currentResourceAllocation = Random.Range(0, amountOfResourceNeeded);
                amountOfResourceNeeded -= currentResourceAllocation;

                //Add resource to the point
                m_resourceDropPoints[i].AllocateResource((Crafting.InteractableResource.EResourceType)resourceIndex, currentResourceAllocation);

                //Check if the amount is 0 before adding it to neededResources
                if (currentResourceAllocation != 0) { m_resourceNeeded.Add((InteractableResource.EResourceType)resourceIndex); }
            }
        }
        #endregion

        #region Enums
        public enum EWeaponUpgrades
        {
            AttackSpeed,
            AttackZone,
            AttackRange,
            AttackDamage,
            Count
        }
        #endregion
    }
}
