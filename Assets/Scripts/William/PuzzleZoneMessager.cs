using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SpaceBaboon
{
    public class PuzzleZoneMessager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void DisableCircleMsg()
        {
            SendMessageUpwards("ReactivateCraftingStation", this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collidedObject)
        {
            if (collidedObject.gameObject.CompareTag("Enemy"))
            {
                SendMessageUpwards("OnEnemyDetected", collidedObject.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collidedObject)
        {
            if (collidedObject.gameObject.CompareTag("Enemy"))
            {
                SendMessageUpwards("OnEnemyExit", collidedObject.gameObject);
            }
            
        }
        
    }
}
