using SpaceBaboon.Crafting;
using UnityEngine;

namespace SpaceBaboon
{
    public class HealthCollectable : InteractableResource
    {
        private Player m_playerToHeal;
        private float m_collectTravelSpeed;
        private float m_maxVelocity;
        private Rigidbody2D m_rigidbody;
        protected override void Start()
        {
            m_collectRange = GameManager.Instance.Player.GetPlayerCollectRange();
            m_circleCollider.radius = m_collectRange;
            m_rendereInitialColor = m_renderer.color;
            m_rigidbody = GetComponent<Rigidbody2D>();
        }
        public override void Collect(Player collectingPlayer)
        {
            //Debug.Log("Healing collected");
            m_parentPool.UnSpawn(gameObject);
        }

        private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Healing consumed");
                collision.gameObject.GetComponent<Player>().HealPlayer(m_resourceData.m_resourceAmount);
                Collect(null);
            }
        }
        private void ChasePlayer()
        {
            Vector2 directionToPlayer = (m_playerToHeal.transform.position - transform.position).normalized;
            m_rigidbody.AddForce(directionToPlayer * m_collectTravelSpeed, ForceMode2D.Impulse);
            m_collectTravelSpeed += Time.fixedDeltaTime;
            m_rigidbody.velocity = Vector2.ClampMagnitude(m_rigidbody.velocity, m_maxVelocity);
        }
    }
}
