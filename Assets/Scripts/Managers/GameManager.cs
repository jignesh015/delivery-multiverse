using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeliveryMultiverse
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Vector2Int deliveriesPerDayRange = new Vector2Int(5, 7);
        [SerializeField] private MMF_Player goToMenuFeedback;
        
        private void Awake()
        {
            GameStatic.ResetGameState();
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
            GameStatic.OnNextDayButtonPressed += StartNewDay;
            GameStatic.OnVehicleDestroyed += OnVehicleDestroyed;
            GameStatic.OnRestartDayButtonPressed += StartNewDay;
            GameStatic.OnResignButtonPressed += OnResignButtonPressed;
        }

        private void OnDestroy()
        {
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
            GameStatic.OnNextDayButtonPressed -= StartNewDay;
            GameStatic.OnVehicleDestroyed -= OnVehicleDestroyed;
            GameStatic.OnRestartDayButtonPressed -= StartNewDay;
            GameStatic.OnResignButtonPressed -= OnResignButtonPressed;
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

        private void OnVehicleDestroyed()
        {
            GameStatic.IsDayActive = false;
            GameStatic.CurrentDayNumber--;
        }
        
        private void OnResignButtonPressed()
        {
            goToMenuFeedback.PlayFeedbacks();
        }
        #endregion
    }
}