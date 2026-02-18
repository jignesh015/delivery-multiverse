using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class DeliveryPointManager : MonoBehaviour
    {
        private List<DeliveryPoint> m_AllDeliveryPoints = new List<DeliveryPoint>();
        private Queue<DeliveryPoint> m_DeliveryPointsQueue = new Queue<DeliveryPoint>();
        private VehicleController m_VehicleController;
        
        private void Awake()
        {
            GameStatic.OnDeliveryPointRequested += OnDeliveryPointsRequested;
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
        }

        private void OnDestroy()
        {
            GameStatic.OnDeliveryPointRequested -= OnDeliveryPointsRequested;
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
        }

        private void Start()
        {
            m_VehicleController = FindFirstObjectByType<VehicleController>();
        }

        private void OnDeliveryPointsRequested()
        {
            if (!m_VehicleController)
            {
                m_VehicleController = FindFirstObjectByType<VehicleController>();
                if (!m_VehicleController)
                    return;
            }

            m_AllDeliveryPoints = new List<DeliveryPoint>(FindObjectsByType<DeliveryPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
            if (m_AllDeliveryPoints == null || m_AllDeliveryPoints.Count == 0)
                return;

            AssignNextDeliveryPoint();
        }

        private void GenerateDeliveryPointsQueue()
        {
            // Shuffle the delivery points to create a random order
            var rnd = new System.Random();
            var shuffledPoints = m_AllDeliveryPoints
                .OrderBy(x => rnd.Next())
                .ToList();

            m_DeliveryPointsQueue.Clear();
            foreach (var point in shuffledPoints)
            {
                m_DeliveryPointsQueue.Enqueue(point);
            }
        }
        
        private void AssignNextDeliveryPoint()
        {
            if (m_DeliveryPointsQueue.Count <= 0)
            {
                GenerateDeliveryPointsQueue();
            }
            
            var nextPoint = m_DeliveryPointsQueue.Dequeue();
            GameStatic.CurrentDeliveryPoint = nextPoint;
            GameStatic.OnDeliveryPointAssigned?.Invoke(nextPoint);
        }

        private void OnDeliveryCompleted(DeliveryPoint deliveryPoint, int tipAmount)
        {
            if (GameStatic.CurrentDeliveryPoint != deliveryPoint) return;
            AssignNextDeliveryPoint();
        }
    }
}