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

        public static int CurrentDayNumber;
        public static int DeliveriesCompletedToday;
        public static int TotalTipsEarnedToday;
        public static float TimeRemainingInDay;

        #endregion

        #region CONSTANTS

        public const string VehicleTag = "Player";
        public const string DeliveryScorePref = "DeliveryScoreInfo";

        #endregion

        #region EVENTS
        
        // Game Events
        public static UnityAction OnNewDayStarted;
        public static UnityAction OnDayEnded;

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

        #endregion

        #region STATIC METHODS

        public static void ResetGameState()
        {
            CurrentDeliveryPoint = null;
            CurrentBiome = BiomeType.Normal;
            IsPlayingMinigame = false;
            IsDayActive = false;
            CurrentDayNumber = LoadDeliveryScores().scores.Count;
            DeliveriesCompletedToday = 0;
            TotalTipsEarnedToday = 0;
            TimeRemainingInDay = 0f;
        }
        
        public static void SaveDeliveryScore()
        {
            var scoreInfo = new DeliveryScoreInfo
            {
                deliveriesCompleted = DeliveriesCompletedToday,
                totalTipsEarned = TotalTipsEarnedToday
            };

            var scoreInfoList = LoadDeliveryScores();
            scoreInfoList.scores.Add(scoreInfo);
            var json = JsonUtility.ToJson(scoreInfoList);
            PlayerPrefs.SetString(DeliveryScorePref, json);
            Debug.Log($"Saved delivery score: {scoreInfo.deliveriesCompleted} deliveries, {scoreInfo.totalTipsEarned} tips");
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