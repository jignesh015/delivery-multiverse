using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeliveryMultiverse
{
    public static class GameStatic
    {
        #region REALTIME DATA
        public static DeliveryPoint CurrentDeliveryPoint;
        #endregion

        #region CONSTANTS
        public const string VehicleTag = "Player";
        

        #endregion

        #region EVENTS
        
        // Delivery Point Events
        public static UnityAction OnDeliveryPointsRequested;
        public static UnityAction<Queue<DeliveryPoint>> OnDeliveryPointsQueued;
        public static UnityAction<DeliveryPoint> OnDeliveryPointAssigned;
        public static UnityAction<DeliveryPoint> OnPlayerEnteredDeliveryPoint;
        public static UnityAction<DeliveryPoint> OnPlayerExitedDeliveryPoint;
        public static UnityAction<DeliveryPoint, bool> OnPlayerStoppedAtDeliveryPoint;
        public static UnityAction<DeliveryPoint> OnPlayerInteractedWithDeliveryPoint;
        public static UnityAction<DeliveryPoint> OnDeliveryCompleted;
        public static UnityAction OnAllDeliveriesCompleted;
        
        public static UnityAction<DeliveryPoint, bool> OnDeliveryPointVisibleOnScreen;
        
        // Player Interaction Events
        public static UnityAction OnPlayerPressedInteract;
        #endregion
    }
}