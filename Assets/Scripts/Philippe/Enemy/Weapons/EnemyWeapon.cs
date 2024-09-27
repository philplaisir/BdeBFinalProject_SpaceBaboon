using SpaceBaboon.EnemySystem;
using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class EnemyWeapon : Weapon
    {
        [SerializeField] protected GameObject m_projectilePrefab;
        [SerializeField] protected WeaponData m_weaponData;

        protected EnemySpawner m_enemySpawner;

        protected Transform m_target;

        private void Start()
        {
            m_enemySpawner = GameManager.Instance.EnemySpawner;
        }

        public void GetTarget(Transform target)
        {
            m_target = target;
            Attack();
        }

        protected override void Attack()
        {           
            Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y);

            var projectile = m_enemySpawner.m_enemyProjectilesPool.Spawn(m_projectilePrefab, spawnPos);

            projectile.GetComponent<Projectile>()?.Shoot(m_target, 0, 0, m_weaponData.baseDamage);
        }
    }
}
