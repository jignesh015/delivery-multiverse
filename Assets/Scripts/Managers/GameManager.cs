using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Vector2Int deliveriesPerDayRange = new Vector2Int(5, 7);
        
        
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
            
            GameStatic.TotalTimeTaken += Time.deltaTime;
        }

        private void StartNewDay()
        {
            GameStatic.CurrentDayNumber++;
            GameStatic.DeliveriesToCompleteToday = UnityEngine.Random.Range(deliveriesPerDayRange.x, deliveriesPerDayRange.y + 1);
            
            GameStatic.TotalTimeTaken = 0 ;
            GameStatic.TotalTipsEarnedToday = 0;
            GameStatic.DeliveriesCompletedToday = 0;
            
            GameStatic.IsDayActive = true;
            GameStatic.OnNewDayStarted?.Invoke();
        }
        
        private void EndCurrentDay()
        {
            GameStatic.IsDayActive = false;
            GameStatic.SaveDeliveryScore();
            GameStatic.OnDayEnded?.Invoke();
        }

        #region Event Handlers

        private void OnDeliveryCompleted(DeliveryPoint deliveryPoint, int tipAmount)
        {
            GameStatic.DeliveriesCompletedToday++;
            GameStatic.TotalTipsEarnedToday += tipAmount;
            
            if (GameStatic.DeliveriesCompletedToday >= GameStatic.DeliveriesToCompleteToday)
            {
                EndCurrentDay();
            }
        }
        

        #endregion
    }
}