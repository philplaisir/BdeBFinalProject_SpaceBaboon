using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class BossEnemyShotGunWeapon : EnemyWeapon
    {
        // TODO needs to be refined
        [SerializeField] private int m_pelletQty;

        protected override void Attack()
        {            
            Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y);

            var projectile = m_enemySpawner.m_enemyProjectilesPool.Spawn(m_projectilePrefab, spawnPos);

            projectile.GetComponent<Projectile>()?.Shoot(m_target, 0, 0, m_weaponData.baseDamage);
        }


    }
}
