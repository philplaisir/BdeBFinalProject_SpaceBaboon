using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon.TutorialSystem
{
    public class TutorialPopUpZone : MonoBehaviour
    {
        [SerializeField] private ETutorialType m_tutorialType;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Player>() != null)
            {
                GameManager.Instance.DisplayTutorialWindow(m_tutorialType, transform.position);

                Destroy(gameObject);
            }
        }
    }
}
