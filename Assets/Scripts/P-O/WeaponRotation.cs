using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceBaboon
{
    public class WeaponRotation : MonoBehaviour
    {
        //Serialized fields
        [SerializeField] private float m_defaultRotationSpeed;
        [SerializeField] private float m_rotationRange;
        [SerializeField] private float m_acceptableRangeOffset;
        [SerializeField] private float m_minDistanceBetweenObjects;

        //General variables
        private Dictionary<GameObject, GameObject> m_rotatingObjects = new Dictionary<GameObject, GameObject>();
        private float m_currentTimer;
        private float m_currentRotationSpeed;
        private bool m_isOnTimer = false;
        private bool m_isReajusting = false;

        //Resources variables
        private bool m_isResource = false;
        private float m_collectTimer;
        private float m_accelerationRatio;

        private void Awake()
        {
            DetectParentType();
            m_currentRotationSpeed = m_defaultRotationSpeed;
        }
        void Update()
        {
            foreach (KeyValuePair<GameObject, GameObject> item in m_rotatingObjects)
            {
                ObjectsUpdate(item);
            }
            if (m_isOnTimer)
            {
                TimerUpdate();
                TimerApplication();
            }
        }
        private void ObjectsUpdate(KeyValuePair<GameObject, GameObject> item)
        {
            float distanceInbetween = Vector2.Distance(transform.position, item.Key.transform.position);

            if (IsInRotationRange(item, distanceInbetween))
            {
                RotateAroundTarget();
            }
            else
            {
                GetInRotationRange(item, distanceInbetween);
            }
        }
        private bool IsInRotationRange(KeyValuePair<GameObject, GameObject> item, float distanceInbetween)
        {
            if (m_rotationRange - m_acceptableRangeOffset < distanceInbetween && distanceInbetween < m_rotationRange + m_acceptableRangeOffset)
            {
                return true;
            }
            return false;
        }
        private void DetectParentType()
        {
            if (GetComponent<Crafting.InteractableResource>() != null)
            {
                m_isResource = true;
                m_collectTimer = GetComponent<Crafting.InteractableResource>().GetCollectTimer();
                m_accelerationRatio = m_defaultRotationSpeed / m_collectTimer;
            }
        }
        private void RotateAroundTarget()
        {
            CheckForReajusting();
            for (int i = 0; i < m_rotatingObjects.Keys.Count; i++)
            {
                if (m_isReajusting)
                {
                    m_rotatingObjects.Keys.ElementAt(i).transform.RotateAround(transform.position, Vector3.forward, i * m_currentRotationSpeed * Time.deltaTime);
                }
                else
                {
                    m_rotatingObjects.Keys.ElementAt(i).transform.RotateAround(transform.position, Vector3.forward, m_currentRotationSpeed * Time.deltaTime);
                }
                m_rotatingObjects.Keys.ElementAt(i).transform.eulerAngles = new Vector3(m_rotatingObjects.Keys.ElementAt(i).transform.eulerAngles.x, m_rotatingObjects.Keys.ElementAt(i).transform.eulerAngles.y, 0f);
            }
            //foreach (KeyValuePair<GameObject, GameObject> item in m_rotatingObjects)
            //{
            //    item.Key.transform.RotateAround(transform.position, Vector3.forward, m_currentRotationSpeed * Time.deltaTime);
            //}
        }
        private void TimerUpdate()
        {
            m_currentTimer -= Time.deltaTime;
            if (m_currentTimer < 0)
            {
                m_isOnTimer = false;
            }
        }
        private void TimerApplication()
        {
            m_currentRotationSpeed = m_currentTimer * m_accelerationRatio;
            if (ResourcesShouldUnregister())
            {
                ResourceUnregistering();
            }
        }
        private bool ResourcesShouldUnregister()
        {
            return (m_currentTimer < 0 && m_isResource);
        }
        private void RegisteringToResource()
        {
            m_isOnTimer = true;
            m_currentTimer = m_collectTimer;
            m_currentRotationSpeed = m_defaultRotationSpeed;
        }
        private void ResourceUnregistering()
        {
            foreach (KeyValuePair<GameObject, GameObject> item in m_rotatingObjects)
            {
                RegisterToPreviousOwner(item);
            }
            m_rotatingObjects.Clear();
            m_currentRotationSpeed = m_defaultRotationSpeed;
        }
        private void RegisterToPreviousOwner(KeyValuePair<GameObject, GameObject> itemToRegister)
        {
            //itemToRegister.Key.GetComponent<WeaponRotation>().RegisterToRotationAxis(itemToRegister.Key, gameObject);
            // Check if the initial owner exists and has a WeaponRotation component
            if (itemToRegister.Value != null && itemToRegister.Value.GetComponent<WeaponRotation>() != null)
            {
                // Register the object back to its previous owner's rotation system
                itemToRegister.Value.GetComponent<WeaponRotation>().RegisterToRotationAxis(itemToRegister.Key);
            }
            else
            {
                // Handle the case where the previous owner is not available or does not have a WeaponRotation component
                Debug.Log("Previous owner is invalid or does not have a WeaponRotation component.");
            }
        }
        private void GetInRotationRange(KeyValuePair<GameObject, GameObject> item, float distanceInbetween)
        {
            Vector2 direction = Vector2.zero;
            Vector2 targetPosition;
            if (m_rotationRange < distanceInbetween - m_acceptableRangeOffset)
            {
                direction = (transform.position - item.Key.transform.position).normalized;
            }
            else if (m_rotationRange + m_acceptableRangeOffset > distanceInbetween)
            {
                direction = (item.Key.transform.position - transform.position).normalized;
            }
            targetPosition = (Vector2)transform.position + direction * m_rotationRange;
            item.Key.transform.position = Vector3.MoveTowards(item.Key.transform.position, targetPosition, m_defaultRotationSpeed * Time.deltaTime);
        }
        private void CheckForReajusting()
        {
            m_isReajusting = false;
            List<Vector2> previousPositions = new List<Vector2>();
            foreach (KeyValuePair<GameObject, GameObject> item in m_rotatingObjects)
            {
                foreach (Vector2 position in previousPositions)
                {
                    if (Vector2.Distance((Vector2)item.Key.transform.position, position) < m_minDistanceBetweenObjects)
                    {
                        m_isReajusting = true;
                        //Debug.Log("Reajusting rotation");
                        return;
                    }
                }
                previousPositions.Add(item.Key.transform.position);
            }
        }
        public void Unregister(GameObject objectToRemove)
        {
            if (objectToRemove && m_rotatingObjects.ContainsKey(objectToRemove))
            {
                m_rotatingObjects.Remove(objectToRemove);
            }
        }
        public void RegisterToRotationAxis(GameObject objectToRegister, GameObject initialOwner = null)
        {
            if (!m_rotatingObjects.ContainsKey(objectToRegister))
            {
                m_rotatingObjects.Add(objectToRegister, initialOwner);
            }

            // Check if initialOwner is not null and has a WeaponRotation component
            WeaponRotation initialOwnerRotation = initialOwner?.GetComponent<WeaponRotation>();
            if (initialOwnerRotation != null && initialOwnerRotation != this)
            {
                initialOwnerRotation.Unregister(objectToRegister);
            }

            objectToRegister.transform.parent = transform;

            if (m_isResource)
            {
                RegisteringToResource();
            }
        }
    }
}
