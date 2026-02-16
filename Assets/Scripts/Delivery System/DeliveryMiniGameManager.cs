using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class DeliveryMiniGameManager : MonoBehaviour
    {
        private void Awake()
        {
            GameStatic.OnPlayerInteractedWithDeliveryPoint += OnPlayerInteractedWithDeliveryPoint;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnPlayerInteractedWithDeliveryPoint -= OnPlayerInteractedWithDeliveryPoint;
        }

        private void OnPlayerInteractedWithDeliveryPoint(DeliveryPoint deliveryPoint)
        {
            // TODO: Implement mini-game logic here. For now, we'll just simulate a successful delivery.
            Debug.Log($"Player interacted with delivery point: {deliveryPoint.name}");
            
            GameStatic.OnDeliveryCompleted?.Invoke(deliveryPoint);
        }
    }
}