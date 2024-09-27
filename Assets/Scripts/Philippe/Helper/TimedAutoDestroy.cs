using UnityEngine;

namespace SpaceBaboon
{
    public class TimedAutoDestroy : MonoBehaviour
    {
        [SerializeField] private float m_timer = 0.0f;

        void Start()
        {
            Destroy(gameObject, m_timer);
        }        
    }
}

//public class TimedAutoDestroy
//{
//    [SerializeField] private float m_timer = 0.0f;
//
//    TimedAutoDestroy(float timer, GameObject gameObject)
//    {
//        GameObject.Destroy(gameObject, timer);
//    }
//
//
//    //void Start()
//    //{
//    //    GameObject.Destroy(this, m_timer);
//    //}
//}