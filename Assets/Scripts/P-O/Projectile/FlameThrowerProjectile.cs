using System.Collections;
using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class FlameThrowerProjectile : Projectile
    {
        #region Variables
        [SerializeField] private float m_maxFlameDuration;
        [SerializeField] private float m_flameRotationSpeed = 100;
        [SerializeField] private const float m_targetUpdateTiming = 0.2f;

        private Transform m_flamethrowerPosition;
        private ParticleSystem m_flames;
        private PolygonCollider2D m_flameCollider;

        private float m_tickMaxDuration;
        private float m_cooldownBetweenTick;
        private float m_currentFlameDuration;
        private float m_flameRange;
        private float m_flameWidth;
        private float m_currentTargetUpdateTime;

        private bool m_isFiring;
        #endregion
        #region ParentOverride
        protected override void Awake()
        {
            m_flames = GetComponentInChildren<ParticleSystem>();
            m_flameCollider = GetComponent<PolygonCollider2D>();
        }
        protected override void Update()
        {
            if (m_flamethrowerPosition != null)
            {
                FollowTargetPosition(m_flamethrowerPosition.position);
                RotateTowardTarget();
            }
            if (m_isFiring)
            {
                TargetUpdate();
                TickUpdate();
                OnProjectileEnd();
                TimersUpdate();
            }
        }
        public override void Shoot(Transform target, float maxRange, float attackZone, float damage, Transform weaponPosition = null)
        {
            base.Shoot(target, maxRange, attackZone, damage, weaponPosition);

            //The flame shoot from the weapon
            m_flamethrowerPosition = weaponPosition;
            m_target = target;

            //Values that scale from weapon
            m_flameRange = maxRange;
            m_flameWidth = attackZone;

            //Methods for size and time values calculation
            InitialRotationAngle();
            CalculateTickDuration(); // Must be after base.shoot for the damage to be assigned first
            SetFlameSize(m_flameRange, m_flameWidth);
            ParticleEffectInitialization();
        }
        #endregion
        #region FlameThrowerLogic
        protected void SetFlameSize(float flameLenght, float flameWidth)
        {
            transform.localScale = new Vector3(flameLenght, flameWidth);
        }
        protected void CalculateTickDuration()
        {
            //Protection from the scary division by zero
            if (m_damage < 1)
            {
                return;
            }

            //Distribute the ticks over the duration of the flame
            m_tickMaxDuration = m_maxFlameDuration / m_damage;
            //Every time the tick is calculated, so should be the damage
            CalculateDamagePerTick();
        }
        protected void CalculateDamagePerTick()
        {
            //Protection from the scary division by zero
            if (m_tickMaxDuration < 0)
            {
                return;
            }

            //Distribute damage over the duration
            m_damage = m_damage / (m_maxFlameDuration / m_tickMaxDuration);

            //Debug.Log("Damage per tick = " + m_damage);
        }
        private void TargetUpdate()
        {
            if (m_currentTargetUpdateTime < 0)
            {
                SearchNewTarget();
                m_currentTargetUpdateTime = m_targetUpdateTiming;
            }
        }
        private void SearchNewTarget()
        {
            for (int i = 0; i < (m_flameRange * 12); i++)
            {
                CheckIfCloserTarget(i);
            }
        }
        private void CheckIfCloserTarget(int index)
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, index);

            foreach (var collider in colliders)
            {
                if (collider.gameObject.tag == "Enemy")
                {
                    m_target = collider.gameObject.transform;
                }
            }
        }
        private void TimersUpdate()
        {
            m_cooldownBetweenTick -= Time.deltaTime;
            m_currentFlameDuration -= Time.deltaTime;
            m_currentTargetUpdateTime -= Time.deltaTime;
        }
        private void TickUpdate()
        {
            if (m_cooldownBetweenTick < 0)
            {
                StartCoroutine(ColliderTickCoroutine());
                m_cooldownBetweenTick = m_tickMaxDuration;
            }
        }
        private void OnProjectileEnd()
        {
            if (m_currentFlameDuration < 0)
            {
                m_parentPool.UnSpawn(gameObject);
            }
        }
        private void FollowTargetPosition(Vector2 targetposition)
        {
            transform.position = targetposition;
        }
        private void RotateTowardTarget()
        {
            //Get the direction toward target
            Vector2 directionToTarget = m_target.position - transform.position;
            float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.z;

            //Get the rotation axis
            float shortestAngle = (angleToTarget - currentAngle + 360) % 360;
            if (shortestAngle > 180)
            {
                shortestAngle -= 360;
            }

            //Now we get the angle needed to reach target
            angleToTarget = Mathf.MoveTowards(0, shortestAngle, Time.deltaTime * m_flameRotationSpeed);

            //Rotate the flames
            transform.rotation = Quaternion.Euler(0, 0, currentAngle + angleToTarget);
        }
        private void InitialRotationAngle()
        {
            Vector2 directionToTarget = m_target.position - transform.position;
            float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angleToTarget);
        }
        private IEnumerator ColliderTickCoroutine()
        {
            m_flameCollider.enabled = true;
            //Debug.Log("Flame collider is " + m_flameCollider.enabled);
            float timePassed = 0;

            while (timePassed < m_tickMaxDuration / 2)
            {
                timePassed += Time.deltaTime;
                yield return null;
            }
            m_flameCollider.enabled = false;
        }
        protected void ParticleEffectInitialization()
        {
            m_flames.transform.localScale = new Vector3(m_flames.transform.localScale.x, transform.localScale.y, transform.localScale.x);
        }
        #endregion
        #region IPoolable
        protected override void SetComponents(bool value)
        {
            //Debug.Log("SetComponents parent appeler");
            m_isActive = value;
            var emission = m_flames.emission;
            emission.enabled = value;
            if (!value)
            {
                m_flameCollider.enabled = value;
            }
            m_isFiring = value;
        }
        protected override void ResetValues(Vector2 pos)
        {
            m_lifetime = 0.0f;
            m_cooldownBetweenTick = -1;
            m_currentTargetUpdateTime = -1;
            m_currentFlameDuration = m_maxFlameDuration;
        }
        #endregion
    }
}
