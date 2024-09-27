using TMPro;
using UnityEngine;

namespace SpaceBaboon.Crafting
{
    public class ResourceDropPoint : MonoBehaviour
    {
        //Serialized variables
        [SerializeField]
        private bool m_DebugMode = false;
        [SerializeField]
        private CraftingStation m_craftingStation;
        [SerializeField]
        private GameObject m_circleRef;
        [SerializeField]
        private GameObject m_circlePercentRef;
        [SerializeField]
        private GameObject m_circleMask;

        //Private variables
        private int m_resourceAmountNeeded;
        private TextMeshPro m_resourceAmountDisplay;
        private Crafting.InteractableResource.EResourceType m_resourceTypeNeeded;
        private SpriteMask m_dropPointMask;
        private SpriteRenderer m_circleDropPointref;


        // Start is called before the first frame update
        void Awake()
        {
            m_resourceAmountDisplay = GetComponentInChildren<TextMeshPro>();
            m_dropPointMask = GetComponentInChildren<SpriteMask>();
            m_circleDropPointref = m_circleRef.GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_circlePercentRef.GetComponent<SpriteRenderer>().color != Color.clear)
            {
                UpdateMaskSize();
            }
            
        }

        private void UpdateMaskSize()
        {
            m_circlePercentRef.transform.localScale = new Vector3(NewSize(), NewSize(), NewSize());
            m_circlePercentRef.GetComponent<SpriteRenderer>().color = new Color(m_circlePercentRef.GetComponent<SpriteRenderer>().color.r,
                m_circlePercentRef.GetComponent<SpriteRenderer>().color.g,
                m_circlePercentRef.GetComponent<SpriteRenderer>().color.b,
                0.5f);
        }
        private float NewSize()
        {
            float playerResources = GameManager.Instance.Player.GetResources((int)m_resourceTypeNeeded);
            float sizeRatio;
            // Ensure we don't divide by zero
            //if (playerResources == 0)
            //    return 0;  // Return full size when no resources are available

            if (!m_craftingStation.ResourceIsNeeded(m_resourceTypeNeeded) || playerResources == 0)
                return 0;

            if (playerResources == 1)
            {
                sizeRatio = playerResources / m_resourceAmountNeeded * m_circleDropPointref.transform.localScale.x;
            }
            else
            {
                sizeRatio = playerResources / m_resourceAmountNeeded * m_circleDropPointref.transform.localScale.x;
            }
            //Debug.Log("The player have " + playerResources + " of " + m_resourceTypeNeeded);
            //Debug.Log("The player need " + m_resourceAmountNeeded + " of " + m_resourceTypeNeeded);
            // Calculate the inverse ratio to make the mask smaller as resources increase
            float newSize = Mathf.Clamp(sizeRatio, 0f, 1f);
            newSize *= m_circleMask.transform.localScale.x;

            return newSize;
        }
        public void CollectResource(Player playerRef)
        {
            if (m_DebugMode) { Debug.Log("Player activated CollectResource on station"); }

            if (playerRef.DropResource(m_resourceTypeNeeded, m_resourceAmountNeeded))
            {
                if (m_DebugMode) { Debug.Log("Calling AddResource on " + m_craftingStation.gameObject.name); }

                //TODO Clean this up
                if (m_craftingStation.AddResource(m_resourceTypeNeeded))
                {
                    if (m_DebugMode) { Debug.Log(gameObject.name + " collected " + m_resourceTypeNeeded); }
                    GetComponent<SpriteRenderer>().color = Color.clear;
                    GetComponent<CircleCollider2D>().enabled = false;
                    m_resourceAmountDisplay.enabled = false;
                    m_circleRef.GetComponent<SpriteRenderer>().color = Color.clear;
                }
            }

        }
        private void CheatUpgrade()
        {
            m_craftingStation.AddResource(m_resourceTypeNeeded);
            GetComponent<SpriteRenderer>().color = Color.clear;
            GetComponent<CircleCollider2D>().enabled = false;
            m_resourceAmountDisplay.enabled = false;
        }
        public void AllocateResource(Crafting.InteractableResource.EResourceType resourceType, int resourceAmount)
        {
            if (m_DebugMode) { Debug.Log("To " + gameObject.name + " was allocated " + resourceAmount + " " + resourceType); }
            m_resourceTypeNeeded = resourceType;
            m_resourceAmountNeeded = resourceAmount;

            Color newColor = Color.white;
            if (resourceAmount > 0)
            {
                if (resourceType == InteractableResource.EResourceType.Metal)
                {
                    //Yellow
                    newColor = Color.yellow;
                }
                else if (resourceType == InteractableResource.EResourceType.Crystal)
                {
                    //Pink
                    newColor = Color.magenta;
                }
                else if (resourceType == InteractableResource.EResourceType.Technologie)
                {
                    //Light blue
                    newColor = Color.cyan;
                }
                
                m_resourceAmountDisplay.text = m_resourceAmountNeeded.ToString();
                m_circleDropPointref.color = newColor;
                GetComponent<CircleCollider2D>().enabled = true;
                m_resourceAmountDisplay.enabled = true;
                m_circlePercentRef.GetComponent<SpriteRenderer>().color = newColor;
                
            }
            else
            {
                SetDisableDropPoint();
            }
        }
        
        public void SetDisableDropPoint()
        {
            //Invisible
            m_circleDropPointref.color = Color.clear;
            GetComponent<CircleCollider2D>().enabled = false;
            m_resourceAmountDisplay.enabled = false;
            m_circlePercentRef.GetComponent<SpriteRenderer>().color = Color.clear;
            m_circlePercentRef.GetComponent<SpriteRenderer>().color = Color.clear;
            m_circleRef.GetComponent<SpriteRenderer>().color = Color.clear;
        }
        public void SetRef()
        {
            m_circleDropPointref = m_circleRef.GetComponent<SpriteRenderer>();
        }
    }
}
