using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int timePerRound = 180;
        
        private bool m_IsDayActive;
        
        private void Awake()
        {
            GameStatic.ResetGameState();
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
        }

        private void OnDestroy()
        {
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
        }

        private void Start()
        {
            StartNewDay();
        }

        private void Update()
        {
            if (!m_IsDayActive)
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
            GameStatic.TimeRemainingInDay += timePerRound;
            GameStatic.OnNewDayStarted?.Invoke();
            m_IsDayActive = true;
        }
        
        private void EndCurrentDay()
        {
            m_IsDayActive = false;
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