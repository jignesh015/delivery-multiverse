using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace DeliveryMultiverse
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int day1And2DeliveryCount = 3;
        [SerializeField] private Vector2Int deliveriesPerDayRange = new Vector2Int(5, 7);
        [SerializeField] private MMF_Player goToMenuFeedback;

        private void Awake()
        {
            GameStatic.ResetGameState();
            GameStatic.OnBeginButtonPressed += StartNewDay;
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
            GameStatic.OnNextDayButtonPressed += OnNextDayButtonPressed;
            GameStatic.OnVehicleDestroyed += OnVehicleDestroyed;
            GameStatic.OnRestartDayButtonPressed += OnRestartDayButtonPressed;
            GameStatic.OnResignButtonPressed += OnResignButtonPressed;
        }

        private void OnDestroy()
        {
            GameStatic.ResetGameState();
            GameStatic.OnBeginButtonPressed -= StartNewDay;
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
            GameStatic.OnNextDayButtonPressed -= OnNextDayButtonPressed;
            GameStatic.OnVehicleDestroyed -= OnVehicleDestroyed;
            GameStatic.OnRestartDayButtonPressed -= OnRestartDayButtonPressed;
            GameStatic.OnResignButtonPressed -= OnResignButtonPressed;
        }

        private void Start()
        {
            if (GameStatic.CurrentDayNumber == 0)
                GameStatic.OnWelcomeScreenRequested?.Invoke();
            else
                StartNewDay();
        }

        private void Update()
        {
            if (!GameStatic.IsDayActive)
                return;

            if (GameStatic.IsPlayingMinigame)
                return;

            GameStatic.TotalTimeTaken += Time.deltaTime;
        }

        private void StartNewDay()
        {
            GameStatic.CurrentDayNumber++;
            GameStatic.DeliveriesToCompleteToday = GameStatic.CurrentDayNumber <= 2 ? day1And2DeliveryCount :
                UnityEngine.Random.Range(deliveriesPerDayRange.x, deliveriesPerDayRange.y + 1);

            GameStatic.TotalTimeTaken = 0;
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

        private void OnNextDayButtonPressed()
        {
            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnRestartDayButtonPressed()
        {
            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnResignButtonPressed()
        {
            goToMenuFeedback.PlayFeedbacks();
        }

        #endregion
    }
}