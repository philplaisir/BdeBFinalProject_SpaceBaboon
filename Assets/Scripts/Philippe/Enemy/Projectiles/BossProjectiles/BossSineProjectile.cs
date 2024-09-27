using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class BossSineProjectile : ShootingEnemyProjectile
    {
        private BossSineProjectileData m_uniqueData;        

        protected override void Start()
        {
            base.Start();
            m_uniqueData = m_projectileData as BossSineProjectileData;
        }

        protected override void Move()
        {
            //TODO maybe change to an anim curve
            float horizontalMovement = Mathf.Sin(Time.time * m_uniqueData.sineWaveFrequency) * m_uniqueData.sineWaveAmplitude;                        
            Vector2 perpendicularDirection = new Vector2(-m_direction.y, m_direction.x);
            Vector2 combinedDirection = m_direction + perpendicularDirection * horizontalMovement;

            m_rb.AddForce(combinedDirection.normalized * m_projectileData.defaultAcceleration, ForceMode2D.Force);

            if (combinedDirection.magnitude > 0)
                RegulateVelocity();
        }
    }
}
