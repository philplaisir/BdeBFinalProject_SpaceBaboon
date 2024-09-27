using UnityEngine;

namespace SpaceBaboon.PoolingSystem
{
    public interface IPoolableGeneric
    {
        public bool IsActive { get; }
        /* 
         must add:
            private bool m_isActive = false;

         at beginning of Update()
            if (!m_isActive) return;
         */

        public void Activate(Vector2 pos, GenericObjectPool pool);
        public void Deactivate();

        /*
        For cleaner code, add:

            // MUST BE ADDED IN ACTIVATE()
        private void ResetValues(Vector2 pos)
            in here, add all values of your poolableObject that need to be reinitialised when you spawn
                ex: position, lifetime, etc...


            //MUST BE ADDED IN ACTIVATE() -- value as true
            //MUST BE ADDED IN DEACTIVATE() -- value as false
        private void SetComponents(bool value)
            in here, add all components that need to be toggled on/off when spawning/unspawning
                ** must contain m_isActive = value; **
                other component examples: renderer, collider, etc...
            as such => someComponent.enabled = value;
         */
    }
}
