using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class PlayerSword : PlayerWeapon
    {
        private Vector3 m_previousPosition;
        private Player m_playerDirection;
        [SerializeField] private float m_detectionRange = 3.0f;
        [SerializeField] private Transform m_directionalHelper;

        protected override void Awake()
        {
            base.Awake();
            m_previousPosition = transform.position;
            m_playerDirection = transform.parent.GetComponent<Player>();
        }
        protected override void Start()
        {
            //Don't use base start
        }
        //To use if we want the sword to aim toward player direction again
        //protected override void Attack()
        //{
        //    Transform targetTransform = GetTarget();
        //    Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y);
        //    var projectile = m_pool.Spawn(m_weaponData.projectilePrefab, spawnPos);

        //    projectile.GetComponent<Projectile>()?.Shoot(targetTransform, m_weaponData.maxRange, m_weaponData.attackZone, currentDamage, transform);

        //    FXSystem.FXManager fxManager = FXSystem.FXManager.Instance;
        //    if (fxManager != null)
        //    {
        //        fxManager.PlayAudio(m_weaponData.shootAudioCue);
        //    }
        //}
        protected override void Update()
        {
            base.Update();
            if (m_playerDirection.GetPlayerDirection().magnitude > 0.0f)
            {
                m_directionalHelper.transform.position = transform.position + (Vector3)m_playerDirection.GetPlayerDirection();
            }
        }
        protected override Transform GetTarget()
        {
            //To use if the sword swing must aim toward player movement direction
            for (int i = 0; i < currentRange * m_detectionRange; i++)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, i);

                foreach (var collider in colliders)
                {
                    if (collider.gameObject.tag == "Enemy")
                    {
                        return collider.gameObject.transform;
                    }
                }
            }
            //Didn't find an enemy
            return m_directionalHelper;
        }
    }
}
