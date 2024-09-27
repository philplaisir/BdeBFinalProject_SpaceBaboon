using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon.CollisionSystem
{
    public enum EAgentType
    {
        Player,
        Enemy,
        Count
    }

    public class Hitbox : MonoBehaviour
    {
        [field: SerializeField] public bool CanHit { get; set; }
        [field: SerializeField] public bool CanReceiveHit { get; set; }
        [field: SerializeField] public EAgentType AgentType { get; set; } = EAgentType.Count;
        [field: SerializeField] public List<EAgentType> AffectedAgents { get; set; } = new List<EAgentType>();


        private bool OtherHitboxCanReceiveHit(Hitbox otherHitbox)
        {
            return CanHit &&
                otherHitbox.CanReceiveHit &&
                AffectedAgents.Contains(otherHitbox.AgentType);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("Hitbox OnCollisionEnter  " + gameObject.name);

            var otherHb = collision.gameObject.GetComponent<Hitbox>();
            if (otherHb == null)
            {
                //Debug.Log(collision.gameObject.name + " has no Hitbox script");
                return;
            }

            if (OtherHitboxCanReceiveHit(otherHb))
            {
                Vector2 contactPoint = collision.GetContact(0).point;
                //call FXManager

                switch (AgentType)
                {
                    case EAgentType.Player:
                        break;
                    case EAgentType.Enemy:
                        var enemy = GetComponent<EnemySystem.Enemy>();
                        if (enemy == null)
                        {
                            //Debug.Log("enemy null");
                            return;
                        }

                        if (enemy.CanAttack())
                        {
                            enemy.ContactAttack(contactPoint);
                            //enemy.ContactAttack(contactPoint, this.transform);
                        }

                        break;
                    case EAgentType.Count:
                        break;
                    default:
                        break;
                }
            }

        }

        void OnTriggerEnter2D(Collider2D other) // Triggers are only used by projectiles
        {
            //Debug.Log("Entered ontrigger");
            var otherHb = other.gameObject.GetComponent<Hitbox>();
            if (otherHb == null)
            {
                //Debug.Log(other.gameObject.name + " has no Hitbox script");
                return;
            }

            //Debug.Log("There is a otherHB " + otherHb.name);

            if (OtherHitboxCanReceiveHit(otherHb))
            {
                var projectile = GetComponent<WeaponSystem.Projectile>();
                if (projectile == null)
                {
                    //Debug.Log("projectile null");
                    return;
                }

                switch (AgentType)
                {
                    case EAgentType.Player:
                        var enemy = other.gameObject.GetComponent<EnemySystem.Enemy>();
                        if (enemy == null)
                        {
                            //Debug.Log("enemy null");
                            return;
                        }

                        enemy.OnDamageTaken(projectile.OnHit(enemy));
                        break;

                    case EAgentType.Enemy:
                        var player = other.gameObject.GetComponent<Player>();
                        if (player == null)
                        {
                            //Debug.Log("player null");
                            return;
                        }

                        player.OnDamageTaken(projectile.OnHit(player));
                        break;
                    case EAgentType.Count:
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
