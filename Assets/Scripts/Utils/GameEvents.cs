using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeliveryMultiverse
{
    public static class GameStatic
    {
        #region REALTIME DATA
        public static DeliveryPoint CurrentDeliveryPoint;
        public static BiomeType CurrentBiome;
        public static bool IsPlayingMinigame;
        #endregion

        #region CONSTANTS
        public const string VehicleTag = "Player";
        

        #endregion

        #region EVENTS
        // Biome Events
        public static UnityAction<BiomeType> OnBiomeChanged;
        
        
        // Delivery Point Events
        public static UnityAction OnDeliveryPointRequested;
        public static UnityAction<DeliveryPoint> OnDeliveryPointAssigned;
        public static UnityAction<DeliveryPoint> OnPlayerEnteredDeliveryPoint;
        public static UnityAction<DeliveryPoint> OnPlayerExitedDeliveryPoint;
        public static UnityAction<DeliveryPoint, bool> OnPlayerStoppedAtDeliveryPoint;
        public static UnityAction<DeliveryPoint> OnPlayerInteractedWithDeliveryPoint;
        public static UnityAction<DeliveryPoint, int> OnDeliveryCompleted; // float parameter is the tip amount earned
        
        public static UnityAction<DeliveryPoint, bool> OnDeliveryPointVisibleOnScreen;
        
        // Player Interaction Events
        public static UnityAction OnPlayerPressedInteract;
        #endregion
    }
}