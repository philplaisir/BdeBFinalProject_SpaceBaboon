using SpaceBaboon.WeaponSystem;
using UnityEngine;
namespace SpaceBaboon
{
    public class DefaultProjectile : Projectile
    {
        protected override void Update()
        {
            base.Update();
            MovingDirection();
        }
        protected void MovingDirection()
        {
            //Debug.Log("Called Default weapon MovingDirection with thos data : m_direction = " + m_direction + " m_projectileDataSpeed = " + m_projectileData.speed);
            transform.Translate(m_direction * m_projectileData.speed * Time.deltaTime, Space.World);
        }

        public override float OnHit(Character characterHit)
        {
            m_parentPool.UnSpawn(gameObject);
            return base.OnHit(characterHit);
        }

        public override void Shoot(Transform target, float maxRange, float attackZone, float damage, Transform playerPosition)
        {
            Vector2 newDirection = target.position;
            Vector2 currentPosition = transform.position;
            m_direction = (newDirection - currentPosition).normalized;
            //Debug.Log("Called Default weapon shoot with those data : m_direction = " + m_direction);
        }
    }
}
