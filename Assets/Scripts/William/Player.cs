using Cinemachine;
using SpaceBaboon.WeaponSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SpaceBaboon
{

    public class Player : Character, SpaceBaboon.IDamageable, IStatsEditable, ISlowable, IGlidable
    {
        //BaseVraiables
        private bool m_alive;
        private bool m_isDashing;
        private bool m_dashInputReceiver;
        private bool m_screenShake;
        private bool m_collectibleInRange;

        private float m_activeDashCD;
        private float m_activeDashCoolDown;
        private float m_dashCurveStrength;
        private float m_activeDashDuration;
        private float m_timestampedDash;
        private float m_currentMaximumVelocity;
        private float m_slowTimer;
        private float m_glideTimer;
        private bool m_isGliding = false;
        private bool m_isSlowed = false;
        private bool m_asDied;

        //Weapons variables
        [SerializeField] private List<PlayerWeapon> m_weaponList = new List<PlayerWeapon>();
        private Dictionary<PlayerWeapon, SWeaponInventoryInfo> m_weaponInventory = new Dictionary<PlayerWeapon, SWeaponInventoryInfo>();

        class SWeaponInventoryInfo
        {
            public bool m_isReadyToCollect;
            public float m_collectTimer;

            public SWeaponInventoryInfo(bool collectStatus, float collectTimer = 0.0f)
            {
                m_isReadyToCollect = collectStatus;
                m_collectTimer = collectTimer;
            }
        }

        //private Vector2 m_movementDirection;
        private AnimationCurve m_dashCurve;
        private Color m_spriteRendererOriginColor;
        private Color m_spriteRendererOriginOutlineColor;
        private Color m_spriteRendererCurrentColor;
        private PlayerFlash m_playerFlash;


        private Dictionary<Crafting.InteractableResource.EResourceType, int> m_collectibleInventory;

        //BonusVariables

        //Collider
        protected BoxCollider2D m_collider;

        //SerializeVraiables
        [SerializeField] private bool m_DebugMode;
        [SerializeField] private PlayerData m_playerData;
        [SerializeField] private CinemachineVirtualCamera m_playerCam;
        [SerializeField] private GameObject m_dahsTrail;
        [SerializeField] private float m_screenShakeAmplitude = 12.0f;
        [SerializeField] private float m_screenShakeFrequency = 2.0f;
        [SerializeField] private float m_collectRange;
        [SerializeField] private GameObject m_healthBar;

        //Cheats related
        private bool m_isInvincible = false;


        //Unity Methods

        #region Unity
        protected override void Awake()
        {
            PlayerVariablesInitialization();
            FreezePlayerRotation();
            RegisterToGameManager();
        }

        private void Start()
        {
            DictionaryInistalisation();
            //So we don't forget our mistakes
            //PlayerVariablesInitialization();
            FreezePlayerRotation();
        }

        protected override void Update()
        {
            OnPlayerDeath();
            InventoryManagement();
            StatusUpdate();
        }

        private void StatusUpdate()
        {
            if (m_isSlowed)
            {
                m_slowTimer -= Time.deltaTime;
                if (m_slowTimer < 0)
                {
                    m_isSlowed = false;
                    EndSlow();
                }
            }
            if (m_isGliding)
            {
                m_glideTimer -= Time.deltaTime;
                if (m_glideTimer < 0)
                {
                    m_isGliding = false;
                    StopGlide();
                }
            }
        }

        private void FixedUpdate()
        {
            PlayerMovement();
            ActiveDashCdReduction();
            CheckForSpriteDirectionSwap(m_movementDirection);
            PlayerDamageTakenScreenShake();
        }

        private void OnDestroy()
        {
            UnsubscribeToInputEvent();
        }

        #endregion

        //Methods

        #region Initialiazation
        private void PlayerVariablesInitialization()
        {
            base.Awake();
            InputHandler.instance.m_Input.Enable();
            SubscribeToInputEvent();

            m_collectibleInventory = new Dictionary<Crafting.InteractableResource.EResourceType, int>();

            m_collider = GetComponent<BoxCollider2D>();
            m_playerCam = GameObject.Find("PlayerCam").GetComponent<CinemachineVirtualCamera>();
            m_spriteRendererOriginColor = m_renderer.color;
            m_spriteRendererCurrentColor = m_spriteRendererOriginColor;
            m_spriteRendererOriginOutlineColor = m_renderer.material.GetColor("_Color");


            m_activeHealth = m_playerData.defaultHealth + m_bonusHealth;
            m_activeDashCoolDown = m_playerData.defaultDashCd;
            m_activeDashDuration = m_playerData.defaultDashDuration;
            m_dashCurve = m_playerData.defaultDashCurve;


            m_alive = true;
            enabled = true;
            m_isDashing = false;
            m_activeDashCD = 0.0f;
            m_dahsTrail.SetActive(false);
            m_screenShake = false;
            m_dashCurveStrength = 0.0f;
            m_timestampedDash = 0.0f;
            m_playerFlash = GetComponent<PlayerFlash>();
            m_currentMaximumVelocity = m_characterData.defaultMaxVelocity;
            m_asDied = false;
        }

        private void RegisterToGameManager()
        {
            GameManager.Instance.SetPlayer(this);
        }
        #endregion Initialiazation

        #region Events
        private void SubscribeToInputEvent()
        {
            InputHandler.instance.m_MoveEvent += Move;
            InputHandler.instance.m_DashStartEvent += DashStart;
            InputHandler.instance.m_CollectResourceEvent += OnCollectResource;
        }

        private void UnsubscribeToInputEvent()
        {
            InputHandler.instance.m_MoveEvent -= Move;
            InputHandler.instance.m_DashStartEvent -= DashStart;
            InputHandler.instance.m_CollectResourceEvent -= OnCollectResource;
        }

        #endregion Events

        #region EventMethods

        public override void Move(Vector2 values)
        {
            m_movementDirection = new Vector2(values.x, values.y).normalized;
        }
        public Vector2 GetPlayerDirection()
        {
            return m_movementDirection;
        }
        private void DashStart()
        {
            if (m_activeDashCD <= 0.0f && m_movementDirection != Vector2.zero)
            {
                m_dashInputReceiver = true;
            }
        }
        private void OnCollectResource()
        {
            if (m_collectibleInRange)
            {
                Crafting.InteractableResource resourceToCollect = SearchClosestResource();
                if (resourceToCollect != null)
                {
                    if (PickWeaponForCollect(resourceToCollect))
                    {
                        resourceToCollect.Collect(this);
                    }
                }
            }
        }
        #endregion EventMethods

        #region Collider
        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject.CompareTag("Structure"))
            {
                if (m_DebugMode) { Debug.Log("CollisionDetected with structure"); }

                collision.gameObject.GetComponent<SpaceBaboon.Crafting.ResourceDropPoint>().CollectResource(this);
            }

        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Resource"))
            {
                m_collectibleInRange = true;
                collision.GetComponent<Crafting.InteractableResource>().CollectableSizing(true);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Resource"))
            {
                m_collectibleInRange = false;
                collision.GetComponent<Crafting.InteractableResource>().CollectableSizing(false);
            }
        }

        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    //if (collision.gameObject.CompareTag("Projectile"))
        //    //{
        //    //    OnDamageTaken(collision.gameObject.GetComponent<SpaceBaboon.Projectile>().GetDamage());
        //    //}
        //}

        #endregion Collider

        #region PlayerMethods
        private void FreezePlayerRotation()
        {
            m_rB.freezeRotation = enabled;
        }

        public void StartSlow(float slowAmount, float slowTimer)
        {
            m_slowTimer = slowTimer;
            m_accelerationMulti = slowAmount;
            m_isSlowed = true;
        }
        public void EndSlow()
        {
            m_accelerationMulti = 1;
        }
        public void StartGlide(float glideAmount, float glideTime)
        {
            m_glideTimer = glideTime;
            m_angularVelocityMulti = glideAmount;
            m_rB.drag = 2;
            m_accelerationMulti = 0.1f;
            RegulateVelocityGliding();
            m_isGliding = true;
        }

        private void RegulateVelocityGliding()
        {
            if (m_rB.velocity.magnitude > 40) //Max Velo Kinda
            {
                m_rB.velocity = m_rB.velocity.normalized;
                m_rB.velocity *= 40;
            }
        }

        public void StopGlide()
        {
            m_accelerationMulti = 1;
            m_angularVelocityMulti = 1;
            m_rB.drag = 20;
        }




        private void OnPlayerDeath()
        {
            if (m_activeHealth <= 0 || m_alive == false)
            {
                m_asDied = true;
                m_alive = false;
                InputHandler.instance.m_Input.Disable();
                DestroyObjectsOnDeath();
                GameManager.Instance.EndGame();
                //SceneManager.LoadScene("SB_MainMenu");
                if (m_asDied)
                {
                    return;
                }

            }
        }
        private void DestroyObjectsOnDeath()
        {
            foreach (var weapon in m_weaponInventory)
            {
                Destroy(weapon.Key.gameObject);
            }
            Destroy(m_healthBar);
        }
        private void ActiveDashCdReduction()
        {
            if (m_activeDashCD > 0.0f)
            {
                m_activeDashCD -= Time.deltaTime;
            }
        }

        private void PlayerMovement()
        {
            if (m_movementDirection != Vector2.zero)
            {
                m_animator.SetBool("Moving", true);
                m_rB.AddForce(m_movementDirection * AccelerationValue, ForceMode2D.Force);   //Etienne : change Acceleration from data.defaultAccel
                //RegulateVelocity();
            }
            if (m_movementDirection == Vector2.zero)
            {
                m_animator.SetBool("Moving", false);
            }
            if (m_dashInputReceiver && m_isDashing == false)
            {
                m_dashInputReceiver = false;
                m_currentMaximumVelocity = m_characterData.dashMaximumVelocity;
                m_rB.AddForce(m_movementDirection * (m_currentMaximumVelocity), ForceMode2D.Impulse);
                StartCoroutine(DashCoroutine());
            }

        }

        private void BeforeDashCoroutine()
        {
            m_isDashing = true;
            m_playerFlash.FlashStart(m_renderer, new Color(1, 1, 1, 0.5f));
            m_timestampedDash = 0.0f;
            m_rB.GameObject().layer = LayerMask.NameToLayer("ImmunityDash");
            m_dahsTrail.SetActive(true);
        }
        private void AfterDashCoroutine()
        {
            m_activeDashCD = m_activeDashCoolDown;
            m_playerFlash.FlashEnd(m_renderer, m_spriteRendererOriginColor);
            m_dahsTrail.SetActive(false);
            m_isDashing = false;
            m_dashInputReceiver = false;
            m_rB.GameObject().layer = LayerMask.NameToLayer("Player");
        }


        private void PlayerDamageTakenScreenShake()
        {
            if (m_screenShake)
            {
                StartCoroutine(PlayerDamageTakenScreenShakeCoroutine());
            }
        }

        public override void OnDamageTaken(float damage)
        {
            //Debug.Log("player takes damage");

            // TODO change name of OnDamageTaken to AttackReceived
            // We could change the IDammageable interface to IAttackable
            // Player could eventually react to an attack here (for example momentarilly impervious, etc.)
            m_screenShake = true;
            m_playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = m_screenShakeAmplitude;
            m_playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = m_screenShakeFrequency;
            m_renderer.material.SetColor("_Color", Color.red);
            if (m_alive && !m_isInvincible)
                m_activeHealth -= damage;
        }

        public void IceZoneEffectsStart(float accelMultiValue, float slowTime)
        {
            StartGlide(accelMultiValue, slowTime);
            StartSlow(accelMultiValue, slowTime);
        }

        public void IceZoneEffectsEnd()
        {
            EndSlow();
            StopGlide();
        }

        public void HealPlayer(int healingValue)
        {
            if (m_alive)
            {
                if ((m_activeHealth + healingValue) >= m_playerData.defaultHealth)
                {
                    m_activeHealth = m_playerData.defaultHealth;
                }
                else
                {
                    m_activeHealth += healingValue;
                }
            }
        }

        #endregion PlayerMethods

        #region PlayerCoroutine
        private IEnumerator DashCoroutine()
        {
            BeforeDashCoroutine();
            while (m_timestampedDash < m_activeDashDuration)
            {

                m_timestampedDash += Time.deltaTime;
                float dashCurvePosition = m_timestampedDash / m_activeDashDuration;
                m_dashCurveStrength = m_dashCurve.Evaluate(dashCurvePosition);

                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            AfterDashCoroutine();
        }

        private IEnumerator PlayerDamageTakenScreenShakeCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            m_screenShake = false;
            m_playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
            m_playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0f;
            m_renderer.material.SetColor("_Color", m_spriteRendererOriginOutlineColor);
            yield return new WaitForSeconds(0.5f);
        }

        #endregion PlayerCoroutine

        #region Crafting
        public void AddResource(Crafting.InteractableResource.EResourceType resourceType, int amount)
        {
            FXSystem.FXManager fxManager = FXSystem.FXManager.Instance;
            if (fxManager != null)
            {
                fxManager.PlayAudio(FXSystem.ESFXType.CoinCollected);
            }

            if (!m_collectibleInventory.ContainsKey(resourceType))
            {
                m_collectibleInventory.Add(resourceType, amount);
            }
            else
            {
                m_collectibleInventory[resourceType] += amount;
            }

            UISystem.UIManager uiManager = UISystem.UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.UpdateResource(resourceType, m_collectibleInventory[resourceType]);
            }

            if (m_DebugMode)
            {
                Debug.Log(resourceType + " amount is : " + m_collectibleInventory[resourceType]);
            }
        }
        private void InventoryManagement()
        {
            foreach (KeyValuePair<PlayerWeapon, SWeaponInventoryInfo> weapon in m_weaponInventory)
            {
                ReduceColletTimer(weapon.Value);
            }
        }
        private void ReduceColletTimer(SWeaponInventoryInfo collectTimerWeapon)
        {
            if (collectTimerWeapon.m_collectTimer > 0)
            { collectTimerWeapon.m_collectTimer -= Time.deltaTime; }

            if (collectTimerWeapon.m_collectTimer < 0)
            { collectTimerWeapon.m_isReadyToCollect = true; }
        }
        public bool DropResource(Crafting.InteractableResource.EResourceType resourceType, int amount)
        {
            if (m_collectibleInventory.ContainsKey(resourceType) && !(m_collectibleInventory[resourceType] < amount))
            {
                m_collectibleInventory[resourceType] -= amount;

                FXSystem.FXManager fxManager = FXSystem.FXManager.Instance;
                if (fxManager != null)
                {
                    fxManager.PlayAudio(FXSystem.ESFXType.DroppingCoins);
                }

                UISystem.UIManager uiManager = UISystem.UIManager.Instance;
                if (uiManager != null)
                {
                    uiManager.UpdateResource(resourceType, m_collectibleInventory[resourceType]);
                }

                if (m_DebugMode) { Debug.Log(resourceType + " amount is : " + m_collectibleInventory[resourceType]); }
                return true;
            }

            return false;
        }

        private Crafting.InteractableResource SearchClosestResource()
        {
            for (int i = 0; i < m_collectRange; i++)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, i);

                foreach (var collider in colliders)
                {
                    if (collider.gameObject.tag == "Resource")
                    {
                        if (!collider.gameObject.GetComponent<Crafting.InteractableResource>().IsBeingCollected())
                        {
                            return collider.gameObject.GetComponent<Crafting.InteractableResource>();
                        }
                    }
                }
            }
            return null;
        }

        private bool PickWeaponForCollect(Crafting.InteractableResource resourceToCollect)
        {
            //Since melee weapon is index 0 of the enum and is the only one that can't collect, we start at index 1
            List<PlayerWeapon> availableWeapons = new List<PlayerWeapon>();

            //Check if weapon is available for collecting
            foreach (KeyValuePair<PlayerWeapon, SWeaponInventoryInfo> possibleWeapon in m_weaponInventory)
            {
                if (CheckIfWeaponIsAvailableForCollect(possibleWeapon.Value.m_isReadyToCollect, possibleWeapon.Key))
                {
                    availableWeapons.Add(possibleWeapon.Key);
                }
            }

            //Pick a weapon at random
            if (availableWeapons.Count > 0)
            {
                //Choose weapon index
                int chosenWeaponindex = Random.Range(0, availableWeapons.Count);

                //Set chosen weapon to collecting and store the collect timer for later
                float newCollectTimer = availableWeapons[chosenWeaponindex].SetIsCollecting(true, resourceToCollect);

                List<PlayerWeapon> weaponsToUpdate = new List<PlayerWeapon>();

                foreach (KeyValuePair<PlayerWeapon, SWeaponInventoryInfo> weapon in m_weaponInventory)
                {
                    if (weapon.Key.GetWeaponData().weaponName == availableWeapons[chosenWeaponindex].GetWeaponData().weaponName)
                    {
                        weaponsToUpdate.Add(weapon.Key);
                    }
                }

                foreach (PlayerWeapon weapon in weaponsToUpdate)
                {
                    m_weaponInventory[weapon].m_isReadyToCollect = false;
                    m_weaponInventory[weapon].m_collectTimer = newCollectTimer;
                    //Debug.Log(m_weaponInventory[weapon]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckIfWeaponIsAvailableForCollect(bool collectingState, PlayerWeapon weaponTypeToCheck)
        {
            //Melee weapon is the only type that shouldn't ever collect
            return (collectingState && weaponTypeToCheck.GetWeaponData().weaponName != EPlayerWeaponType.Melee);
        }
        #endregion

        #region Gets

        public float GetCurrentHealth()
        {
            return m_activeHealth;
        }
        public int GetResources(int resourceType)
        {
            if (m_collectibleInventory.ContainsKey((Crafting.InteractableResource.EResourceType)resourceType))
            {
                return m_collectibleInventory[(Crafting.InteractableResource.EResourceType)resourceType];
            }
            else
            {
                return 0;
            }
        }
        private void DictionaryInistalisation()
        {
            //Initialize collectible inventory
            for (int i = 0; i != (int)Crafting.InteractableResource.EResourceType.Count; i++)
            {
                m_collectibleInventory.Add((Crafting.InteractableResource.EResourceType)i, 0);
            }

            //Initialize weapon inventory
            foreach (PlayerWeapon weapon in m_weaponList)
            {
                m_weaponInventory.Add(weapon, new SWeaponInventoryInfo(!weapon.CheckIfCollecting()));
            }
        }

        public List<WeaponSystem.PlayerWeapon> GetPlayerWeapons()
        {
            return m_weaponList;
        }
        public float GetPlayerCollectRange()
        {
            return m_collectRange;
        }
        #endregion Gets

        #region Cheats
        public void SetIsInvincible(bool value)
        {
            m_isInvincible = value;
            Debug.Log("Invincibility is : " + m_isInvincible);
        }

        public void SetCurrentHealthToMax()
        {
            m_activeHealth = m_playerData.defaultHealth;
        }

        public void SetSpeedWithMultiplier(float value)
        {
            m_speedMultiplierCheat = value;
            Debug.Log("Max Velocity Mult : " + m_speedMultiplierCheat);
        }

        public void SetWeaponStatus(WeaponSystem.EPlayerWeaponType type, bool value)
        {
            foreach (KeyValuePair<PlayerWeapon, SWeaponInventoryInfo> weapon in m_weaponInventory)
            {
                if (weapon.Key.GetWeaponData().weaponName == type) { weapon.Key.ToggleWeapon(value); }
            }
        }

        public void KillPlayer()
        {
            m_activeHealth = 0;
        }

        #endregion

        public override ScriptableObject GetData()
        {
            return m_playerData;
        }
    }
}
