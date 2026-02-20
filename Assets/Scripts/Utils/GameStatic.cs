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
        public static bool IsDayActive;
        public static VehicleConfig CurrentVehicleConfig;

        public static int CurrentDayNumber;
        public static int DeliveriesToCompleteToday;
        public static int DeliveriesCompletedToday;
        public static int TotalTipsEarnedToday;
        public static float TotalTimeTaken;
        
        public static float VehicleHealth = 1f; // 0 to 1, where 1 is full health and 0 is destroyed

        #endregion

        #region CONSTANTS

        public const string VehicleTag = "Player";
        public const string ObstacleTag = "Obstacle";
        public const string DeliveryScorePref = "DeliveryScoreInfo";

        #endregion

        #region EVENTS
        
        // Game Events
        public static UnityAction OnNewDayStarted;
        public static UnityAction OnDayEnded;
        
        // Vehicle Events
        public static UnityAction OnVehicleCollidedWithObstacle;
        public static UnityAction OnVehicleCollectedRepairKit;
        public static UnityAction OnVehicleDestroyed;

        // Biome Events
        public static UnityAction<BiomeType> OnBiomeChanged;

        // Delivery Point Events
        public static UnityAction<DeliveryPoint> OnDeliveryPointAssigned;
        public static UnityAction<DeliveryPoint> OnPlayerEnteredDeliveryPoint;
        public static UnityAction<DeliveryPoint> OnPlayerExitedDeliveryPoint;
        public static UnityAction<DeliveryPoint, bool> OnPlayerStoppedAtDeliveryPoint;
        public static UnityAction<DeliveryPoint> OnPlayerInteractedWithDeliveryPoint;
        public static UnityAction<DeliveryPoint, int> OnDeliveryCompleted; // float parameter is the tip amount earned
        public static UnityAction<DeliveryPoint, bool> OnDeliveryPointVisibleOnScreen;

        // Player Interaction Events
        public static UnityAction OnPlayerPressedInteract;
        
        // Player UI Events
        public static UnityAction OnNextDayButtonPressed;
        public static UnityAction OnRestartDayButtonPressed;
        public static UnityAction OnResignButtonPressed;

        #endregion

        #region STATIC METHODS

        public static void ResetGameState()
        {
            CurrentDeliveryPoint = null;
            CurrentVehicleConfig = null;
            CurrentBiome = BiomeType.Normal;
            IsPlayingMinigame = false;
            IsDayActive = false;
            CurrentDayNumber = LoadDeliveryScores().scores.Count;
            TotalTipsEarnedToday = 0;
            TotalTimeTaken = 0f;
            VehicleHealth = 1f;
        }
        
        public static void SaveDeliveryScore()
        {
            var scoreInfo = new DeliveryScoreInfo
            {
                totalTimeTaken = TotalTimeTaken,
                totalTipsEarned = TotalTipsEarnedToday
            };

            var scoreInfoList = LoadDeliveryScores();
            scoreInfoList.scores.Add(scoreInfo);
            var json = JsonUtility.ToJson(scoreInfoList);
            PlayerPrefs.SetString(DeliveryScorePref, json);
            Debug.Log($"Saved delivery score: {scoreInfo.totalTimeTaken} deliveries, {scoreInfo.totalTipsEarned} tips");
        }
        
        public static DeliveryScoreInfoList LoadDeliveryScores()
        {
            var json = PlayerPrefs.GetString(DeliveryScorePref, null);
            if (string.IsNullOrEmpty(json))
            {
                return new DeliveryScoreInfoList();
            }

            var scoreInfoList = JsonUtility.FromJson<DeliveryScoreInfoList>(json);
            return scoreInfoList;
        }

        #endregion
    }
}