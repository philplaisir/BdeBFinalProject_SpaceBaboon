using System.Collections.Generic;
using UnityEngine;
using SpaceBaboon.PoolingSystem;

namespace SpaceBaboon.FXSystem
{
    public enum ESFXType
    {
        PlayerHit,
        PlayerDash,
        GrenadeLaunch,
        GrenadeExplode,
        Shockwave,
        LaserBeam,
        BananaSword,
        FlameThrower,
        EnemyMelee,
        EnemyShoot,
        EnemyExplode,
        EnemySlashAttack,
        Mining,
        MiningCompleted,
        CoinCollected,
        DroppingCoins,
        WeaponUpgrading,
        Count
    }

    public enum EVFXType
    {
        EnemySlashAttack,
        PlayerSlashAttack,
        EnemyDamagePopUp,
        Count
    }

    public class FXManager : MonoBehaviour
    {
        private static FXManager instance;
        public static FXManager Instance
        {
            get
            {
                if (instance != null) { return instance; }

                Debug.LogError("FXManager instance is null");
                return null;
            }
        }

        //SFX
        [SerializeField] private List<SFXEvent> m_sfxEvents = new List<SFXEvent>();
        private Dictionary<ESFXType, AudioClip> m_sfxDictionary = new Dictionary<ESFXType, AudioClip>();
        [SerializeField] private GameObject m_audioSourcePrefab;
        private ObjectPool m_sfxPool = new ObjectPool();

        //VFX
        [SerializeField] private List<VFXEvent> m_vfxEvents = new List<VFXEvent>();
        private Dictionary<EVFXType, VFXEvent> m_vfxDictionary = new Dictionary<EVFXType, VFXEvent>();
        private GenericObjectPool m_vfxPool = new GenericObjectPool();


        private CameraShake m_cameraShakeController;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        private void Start()
        {
            SetupSFXPool();
            SetupVFXPool();
        }

        private void SetupSFXPool()
        {
            m_sfxPool.SetPoolSize(10);
            m_sfxPool.CreatePool(m_audioSourcePrefab);

            foreach (var item in m_sfxEvents)
            {
                m_sfxDictionary.Add(item.type, item.clip);
            }
        }

        private void SetupVFXPool()
        {

            List<GameObject> prefabList = new List<GameObject>();
            foreach (var item in m_vfxEvents)
            {
                m_vfxDictionary.Add(item.type, item);
                prefabList.Add(item.gameObject);
            }

            m_vfxPool.SetPoolStartingSize(5);
            m_vfxPool.CreatePool(prefabList, "VFX");
        }

        public void PlayAudio(ESFXType type)
        {
            //Debug.Log("PlayAudio called :  " + type);
            GameObject obj = m_sfxPool.Spawn(transform.position);

            AudioSource audioSource = obj.GetComponent<AudioInstance>().AudioSource;
            //Debug.Log("audioSource " + audioSource);

            if (type == ESFXType.GrenadeExplode || type == ESFXType.WeaponUpgrading)
            {
                audioSource.PlayOneShot(m_sfxDictionary[type], 0.2f);
                return;
            }
            audioSource.PlayOneShot(m_sfxDictionary[type]);
            //Debug.Log("dictionary value: " + m_dictionary[type].name);
        }

        public GameObject PlayVFX(EVFXType type, Vector3 pos)
        {
            GameObject obj = m_vfxPool.Spawn(m_vfxDictionary[type].gameObject, pos);

            if (m_vfxDictionary[type].hasAudio)
            {
                PlayAudio(m_vfxDictionary[type].audioType);
            }

            return obj;
        }

        #region CameraShake
        public void RegisterCameraShakeController(CameraShake controller)
        {
            m_cameraShakeController = controller;
        }

        public void ShakeCamera(float intensity, float frequency, float duration = 0.5f)
        {
            m_cameraShakeController.ShakeCamera(intensity, frequency, duration);
        }
        #endregion

    }

    [System.Serializable]
    public struct SFXEvent
    {
        public ESFXType type;
        public AudioClip clip;
    }

    [System.Serializable]
    public struct VFXEvent
    {
        public EVFXType type;
        public GameObject gameObject;
        public bool hasAudio;
        public ESFXType audioType;
    }
}
