using SpaceBaboon.EnemySystem;
using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class BossSpecialProjectile : Projectile
    {
        protected BossSpecialProjectileData m_uniqueData; // TODO put variables in scriptableObject

        private EnemySpawner m_enemySpawner;
        protected Player m_player;

        private Vector2 m_targetSavedPos = Vector2.zero;
        private Vector3 m_originalScale;
        private float m_distanceToPosThreshold = 5.0f;
        private float m_scalingTimer = 0.0f;
        private bool m_isAtTargetPos = false;

        protected void Start()
        {
            m_enemySpawner = GameManager.Instance.EnemySpawner;
            m_originalScale = transform.localScale;
            m_uniqueData = m_projectileData as BossSpecialProjectileData;
            m_player = GameManager.Instance.Player;
        }

        protected override void Update()
        {
            //base.Update();

            if (!m_isActive)
                return;

            if (m_isActive && !m_isAtTargetPos)
            {
                m_collider.enabled = false;
            }

            if (m_lifetime > m_projectileData.maxLifetime)
            {
                // TODO clean this update
                m_isAtTargetPos = false;
                m_collider.enabled = false;
                m_targetSavedPos = new Vector2(10000, 10000);
                transform.localScale = m_originalScale;
                m_scalingTimer = 0.0f;                
                m_rb.constraints = RigidbodyConstraints2D.None;                
                m_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                m_parentPool.UnSpawn(gameObject);
                //Debug.Log("UnSpawning (lifetime)");
            }

            m_lifetime += Time.deltaTime;

            if (m_isAtTargetPos)
            {
                float scaleProgress = Mathf.Clamp01(m_scalingTimer / m_uniqueData.sizeScalingDuration);
                float curveValue = m_uniqueData.sizeScalingCurve.Evaluate(scaleProgress);

                float targetScaleFactor = curveValue * m_uniqueData.radiusSizeMultiplier;
                Vector3 targetScale = m_originalScale * targetScaleFactor;

                transform.localScale = targetScale;

                m_scalingTimer += Time.deltaTime;

                return;
            }

            Move();
        }

        protected override void Move()
        {
            m_direction.x = m_targetSavedPos.x - transform.position.x;
            m_direction.y = m_targetSavedPos.y - transform.position.y;            
            m_direction = m_direction.normalized;

            float distanceToPos = Vector2.Distance(transform.position, m_targetSavedPos);

            //Debug.Log("distance to pos " + distanceToPos);

            if (distanceToPos > m_distanceToPosThreshold)
            {
                m_rb.AddForce(m_direction * m_projectileData.defaultAcceleration, ForceMode2D.Force);
                RegulateVelocity();
            }
            else
            {
                m_isAtTargetPos = true;
                m_collider.enabled = true;
                //m_rb.bodyType = RigidbodyType2D.Kinematic;
                m_rb.constraints = RigidbodyConstraints2D.FreezeAll;
                m_rb.velocity = Vector2.zero;
            }
        }

        public override void Shoot(Transform direction, float maxRange, float attackZone, float damage, Transform playerPosition)
        {
            m_targetSavedPos = FindValidTargetPosition(direction);

            //Debug.Log("PlayerSavedPos " + m_targetSavedPos);

            Vector2 currentPosition = transform.position;

            //Debug.Log("currentBossPosition " + currentPosition);

            m_direction = (m_targetSavedPos - currentPosition).normalized;
            m_damage = damage;
        }

        private Vector2Int FindValidTargetPosition(Transform targetPosition)
        {
            //Debug.Log("target position " + targetPosition.position);
            Vector3Int currentTargetTilePos = m_enemySpawner.m_obstacleTilemapRef.WorldToCell(targetPosition.position);
            //Debug.Log("current target tile pos " + currentTargetTilePos);
            float closestDistance = Mathf.Infinity;
            Vector3Int closestObstaclePos = Vector3Int.zero;

            foreach (var obstacleTilePos in m_enemySpawner.m_obstacleTilemapRef.cellBounds.allPositionsWithin)
            {
                if (m_enemySpawner.m_obstacleTilemapRef.HasTile(obstacleTilePos))
                {
                    float distanceBetweenTargetAndObstacle = Vector3Int.Distance(obstacleTilePos, currentTargetTilePos);

                    if (distanceBetweenTargetAndObstacle <= m_uniqueData.radiusSizeMultiplier && distanceBetweenTargetAndObstacle < closestDistance)
                    {
                        closestDistance = distanceBetweenTargetAndObstacle;
                        closestObstaclePos = obstacleTilePos;
                    }
                }
            }

            if (closestObstaclePos == Vector3Int.zero)
            {
                return new Vector2Int((int)targetPosition.position.x, (int)targetPosition.position.y);
            }

            Vector3 closestObstacleVec3 = closestObstaclePos;
            Vector3 directionToObstacle = (closestObstacleVec3 - currentTargetTilePos).normalized;
            Vector3 adjustedPosition = targetPosition.position + directionToObstacle * -m_uniqueData.radiusSizeMultiplier;

            return new Vector2Int((int)adjustedPosition.x, (int)adjustedPosition.y);
        }





    }
}
