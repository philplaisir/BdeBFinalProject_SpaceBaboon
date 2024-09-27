using System.Collections;
using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class PlayerSwordSwing : Projectile, IPiercing
    {
        [SerializeField] private float m_attackZoneScaling;
        [SerializeField] private SpriteRenderer m_bladeRenderer;
        private Transform m_playerPos;
        private float m_swingArc;
        private CapsuleCollider2D m_swordHitbox;

        //IPiercing
        private int m_currentAmountOfPierceLeft;
        [SerializeField] private PiercingData m_piercingData;

        protected override void Awake()
        {
            m_renderer = GetComponentInChildren<SpriteRenderer>();
            m_swordHitbox = GetComponentInChildren<CapsuleCollider2D>();
        }
        public override void Shoot(Transform target, float maxRange, float attackZone, float damage, Transform swordPosition)
        {
            base.Shoot(target, maxRange, attackZone, damage, swordPosition);
            m_playerPos = swordPosition;
            StartSwing(target, maxRange, attackZone);
        }
        protected void StartSwing(Transform direction, float maxRange, float attackZone)
        {
            m_swingArc = m_attackZoneScaling * attackZone;
            StartCoroutine(SwingCoroutine(direction, maxRange, attackZone));
            gameObject.transform.localScale = new Vector3(1, maxRange, 1);
        }
        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                OnPiercing();
                LastPierce();
            }
        }
        protected override void Update()
        {
            //base.Update();
        }
        private IEnumerator SwingCoroutine(Transform direction, float maxRange, float attackZone)
        {
            //Vector2 targetDirection = (direction.position - transform.position).normalized;

            Vector2 directionVector = direction.position - transform.position;
            float directionAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
            float startAngle = directionAngle - m_swingArc / 2 - 90;
            float endAngle = directionAngle + m_swingArc / 2 - 90;

            float currentTime = 0.0f;
            float t;
            float currentAngle;

            while (currentTime < m_projectileData.speed)
            {
                currentTime += Time.deltaTime;
                t = currentTime / m_projectileData.speed;

                currentAngle = Mathf.Lerp(startAngle, endAngle, t);

                transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                transform.position = m_playerPos.position;

                yield return null;
            }
            m_currentAmountOfPierceLeft = 0;
            LastPierce();
        }
        public void IPiercingSetUp()
        {
            m_currentAmountOfPierceLeft = m_piercingData.m_piercingMultiplier;
        }

        public void LastPierce()
        {
            if (m_currentAmountOfPierceLeft <= 0)
            {
                m_parentPool.UnSpawn(gameObject);
            }
        }

        public void OnPiercing()
        {
            m_currentAmountOfPierceLeft--;
        }
        protected override void ResetValues(Vector2 pos)
        {
            base.ResetValues(pos);
            IPiercingSetUp();
        }

        protected override void SetComponents(bool value)
        {
            //Debug.Log("SetComponents parent appeler");
            m_isActive = value;
            m_renderer.enabled = value;
            m_bladeRenderer.enabled = value;
            m_swordHitbox.enabled = value;
        }
    }
}