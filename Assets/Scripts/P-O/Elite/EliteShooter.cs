using SpaceBaboon.EnemySystem;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon
{
    public class EliteShooter : ShootingEnemy
    {
        [SerializeField] private int m_healthMultipler;
        [SerializeField] private List<ResourceData> m_possibleResources = new List<ResourceData>();
        [SerializeField] private float m_shardsSpawnStrenght;
        [SerializeField] private int m_resourceAmountMultiplier;
        protected override void Start()
        {
            base.Start();
            m_activeHealth = m_activeHealth * m_healthMultipler;
        }
        public override void OnDamageTaken(float damage)
        {
            //Debug.Log("Elite take damage");
            m_activeHealth -= damage;
            if (m_activeHealth <= 0)
            {
                if (m_enemyUniqueData.enemyType != EEnemyTypes.Boss)
                {
                    SpawnResources();
                    m_parentPool.UnSpawn(gameObject);
                    return;
                }
            }

            SpriteFlashRed();
            //DamagePopUp.Create(this.transform.position, damage);
            GameObject vfx = FXSystem.FXManager.Instance.PlayVFX(FXSystem.EVFXType.EnemyDamagePopUp, transform.position);
            var script = vfx.GetComponent<FXSystem.AnimateDamagePopUp>();
            if (script != null)
            {
                script.Activate(damage);
            }
        }
        private void SpawnResources()
        {
            //Debug.Log("Should spawn resources from elite");
            foreach (var resource in m_possibleResources)
            {
                Vector2 direction;
                float angleBetweenShards = 360 / resource.m_resourceAmount;
                float spawnAngle;
                GameObject spawnedShard;

                for (int i = 0; i < resource.m_resourceAmount * m_resourceAmountMultiplier; i++)
                {
                    spawnAngle = i * angleBetweenShards * Mathf.Deg2Rad;
                    direction = new Vector2(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle));
                    spawnedShard = GameManager.Instance.GetResourceManager().SpawnShard(resource.m_shardPrefab, transform.position);
                    spawnedShard.GetComponent<Crafting.ResourceShards>().Initialization(direction, m_shardsSpawnStrenght, GameManager.Instance.Player);
                }
            }
        }
    }
}
