using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int timePerRound = 180;
        
        private void Awake()
        {
            GameStatic.ResetGameState();
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
            GameStatic.OnNextDayButtonPressed += StartNewDay;
        }

        private void OnDestroy()
        {
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
            GameStatic.OnNextDayButtonPressed -= StartNewDay;
        }

        private void Start()
        {
            StartNewDay();
        }

        private void Update()
        {
            if (!GameStatic.IsDayActive)
                return;
            
            if(GameStatic.IsPlayingMinigame)
                return;
            
            GameStatic.TimeRemainingInDay -= Time.deltaTime;
            if (GameStatic.TimeRemainingInDay > 0) return;
            EndCurrentDay();
        }

        private void StartNewDay()
        {
            GameStatic.CurrentDayNumber++;
            GameStatic.TotalTipsEarnedToday = 0;
            GameStatic.DeliveriesCompletedToday = 0;
            GameStatic.TimeRemainingInDay = timePerRound;
            
            GameStatic.IsDayActive = true;
            GameStatic.OnNewDayStarted?.Invoke();
        }
        
        private void EndCurrentDay()
        {
            GameStatic.IsDayActive = false;
            GameStatic.TimeRemainingInDay = 0;
            GameStatic.OnDayEnded?.Invoke();
            GameStatic.SaveDeliveryScore();
        }

        #region Event Handlers

        private void OnDeliveryCompleted(DeliveryPoint deliveryPoint, int tipAmount)
        {
            GameStatic.DeliveriesCompletedToday++;
            GameStatic.TotalTipsEarnedToday += tipAmount;
        }
        

        #endregion
    }
}