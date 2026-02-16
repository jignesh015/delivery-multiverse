using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            GameStatic.OnDeliveryPointsRequested?.Invoke();
        }
    }
}