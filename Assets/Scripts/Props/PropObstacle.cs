using UnityEngine;

namespace DeliveryMultiverse
{
    public class PropObstacle : MonoBehaviour
    {
        public BiomeType biomeType;
        
        public void DestroyObstacle()
        {
            StaticPool.Return(this);
        }
    }
}