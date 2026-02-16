using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class DeliveryPointManager : MonoBehaviour
    {
        [SerializeField] private int maxDeliveryPoints = 5;
        
        private Queue<DeliveryPoint> m_DeliveryPointsQueue = new Queue<DeliveryPoint>();
        private VehicleController m_VehicleController;
        
        private void Awake()
        {
            GameStatic.OnDeliveryPointsRequested += OnDeliveryPointsRequested;
        }

        private void Start()
        {
            m_VehicleController = FindFirstObjectByType<VehicleController>();
        }

        private void OnDestroy()
        {
            GameStatic.OnDeliveryPointsRequested -= OnDeliveryPointsRequested;
        }

        private void OnDeliveryPointsRequested()
        {
            if (!m_VehicleController)
            {
                m_VehicleController = FindFirstObjectByType<VehicleController>();
                if (!m_VehicleController)
                    return;
            }

            var allPoints = FindObjectsByType<DeliveryPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (allPoints == null || allPoints.Length == 0)
                return;

            // Shuffle and select maxDeliveryPoints using LINQ
            var rnd = new System.Random();
            var selectedPoints = allPoints
                .OrderBy(x => rnd.Next())
                .Take(maxDeliveryPoints)
                .OrderBy(x => Vector3.Distance(m_VehicleController.transform.position, x.transform.position))
                .ToList();

            m_DeliveryPointsQueue.Clear();
            foreach (var point in selectedPoints)
            {
                m_DeliveryPointsQueue.Enqueue(point);
            }
            
            var cloneQueue = new Queue<DeliveryPoint>(m_DeliveryPointsQueue);
            GameStatic.OnDeliveryPointsQueued?.Invoke(cloneQueue);
            
            // Pop the first point to assign it immediately
            if (m_DeliveryPointsQueue.Count <= 0) return;
            var firstPoint = m_DeliveryPointsQueue.Dequeue();
            GameStatic.CurrentDeliveryPoint = firstPoint;
            GameStatic.OnDeliveryPointAssigned?.Invoke(firstPoint);
        }
    }
}