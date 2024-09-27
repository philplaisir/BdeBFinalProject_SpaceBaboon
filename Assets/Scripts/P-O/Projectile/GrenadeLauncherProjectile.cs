using SpaceBaboon.WeaponSystem;
using System.Collections;
using UnityEngine;

namespace SpaceBaboon
{
    public class GrenadeLauncherProjectile : Projectile, IExplodable
    {
        //Private variables
        [SerializeField] private AnimationCurve m_grenadeCurve;
        [SerializeField] private float m_curveMaxHeight;
        [SerializeField] private float m_curveDuration;
        [SerializeField] private float m_baitTimer;
        private Vector2 m_lastTargetPosition;
        private Vector2 m_initialShootingPosition;
        private float m_initialDistanceToTarget;

        //IExplodable data
        [SerializeField] private ExplodableData m_explodableData;
        private float m_currentExplosionTime = 0.0f;
        private bool m_isExploding = false;
        private Vector3 m_initialScaleOfProjectile;

        protected void Start()
        {
            m_target = null;
            IExplodableSetUp();
        }
        protected override void Update()
        {
            //base.Update();
            if (!m_isActive)
            {
                return;
            }

            IExplodableUpdate();
        }
        public override void Shoot(Transform target, float maxRange, float attackZone, float damage, Transform playerPosition)
        {
            base.Shoot(target, maxRange, attackZone, damage, playerPosition);
            m_target = target;
            m_initialShootingPosition = transform.position;
            m_explodableData.m_explosionRadius = attackZone;
            Vector2 initialPosition = transform.position;
            if (m_target != null)
            {
                //Debug.Log(m_target.position);
                m_initialDistanceToTarget = Vector2.Distance(m_target.position, initialPosition);
                m_lastTargetPosition = m_target.position;
            }
            StartCoroutine(Curve(transform.position, m_target.position));
        }

        public IEnumerator Curve(Vector2 start, Vector2 end)
        {
            float timePassed = 0.0f;

            if (m_target != null)
            {
                while (timePassed < m_curveDuration)
                {
                    timePassed += Time.deltaTime;

                    float timeSpeedScaling = timePassed / m_curveDuration;
                    float heightScaling = m_grenadeCurve.Evaluate(timeSpeedScaling);

                    float directionHeightModifier = Mathf.Lerp(0.0f, m_curveMaxHeight, heightScaling);

                    transform.position = Vector2.Lerp(start, end, timeSpeedScaling) + new Vector2(0.0f, directionHeightModifier);

                    yield return null;
                }
            }
            StartExplosion();
        }
        public void Explode()
        {
            //Debug.Log("Explode");
            gameObject.transform.localScale = new Vector2(m_explodableData.m_explosionRadius, m_explodableData.m_explosionRadius);
            m_collider.enabled = true;

            FXSystem.FXManager fxManager = FXSystem.FXManager.Instance;
            if (fxManager != null)
            {
                fxManager.PlayAudio(FXSystem.ESFXType.GrenadeExplode);
            }

        }

        public void IExplodableUpdate()
        {
            if (m_currentExplosionTime < 0)
            {
                m_parentPool.UnSpawn(gameObject);
            }

            if (m_isExploding)
            {
                m_currentExplosionTime -= Time.deltaTime;
            }
        }

        public void StartExplosion()
        {
            //Debug.Log("Start explosion");
            //m_collider.radius = m_explodableData.m_explosionRadius;
            m_isExploding = true;
            m_currentExplosionTime = m_explodableData.m_maxExplosionTime;
            Explode();
        }

        protected override void ResetValues(Vector2 pos)
        {
            transform.position = pos;
            gameObject.transform.localScale = m_initialScaleOfProjectile;
            m_currentExplosionTime = 0;
        }
        //public override void Activate(Vector2 pos, ObjectPool pool)
        //{
        //    ResetValues(pos);
        //    m_parentPool = pool;
        //    m_collider.enabled = false;
        //    //Debug.Log("Activate enfant grenade appeler,Explosion time :" + m_currentExplosionTime + " et isExploding : " + m_isExploding);
        //}
        protected override void SetComponents(bool value)
        {
            //Debug.Log("SetComponents parent appeler");
            m_renderer.enabled = value;

            m_isActive = value;

            if (!value)
            {
                m_collider.enabled = value;
                m_isExploding = false;
            }
        }
        public void IExplodableSetUp()
        {
            //m_collider.enabled = false;
            m_initialScaleOfProjectile = transform.localScale;
        }
        public override float OnHit(Character characterHit)
        {
            if (characterHit != null)
            {
                if (characterHit.GetComponent<IBaitable>() != null)
                {
                    characterHit.GetComponent<IBaitable>().StartBait(transform, m_baitTimer);
                    //Debug.Log(characterHit + " was pulled");
                }
            }
            return base.OnHit(characterHit);
        }
    }
}
