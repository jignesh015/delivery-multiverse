using UnityEngine;

namespace DeliveryMultiverse
{
    /// <summary>
    /// Component attached to pooled objects to track their original prefab reference
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        public GameObject prefab;
    }
}

