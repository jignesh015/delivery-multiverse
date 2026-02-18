using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class VehicleHealthManager : MonoBehaviour
    {
        [SerializeField] private float minHealthDecreaseOnCollision = 0.2f;
        [SerializeField] private float maxHealthDecreaseOnCollision = 0.4f;
        [SerializeField] private float immunityDurationAfterCollision = 0.5f;
        [SerializeField] private float minValidHealth = 0.1f;
        
        private float m_LastCollisionTime = -Mathf.Infinity;
        
        private void Awake()
        {
            GameStatic.OnNewDayStarted += OnNewDayStarted;
        }

        private void OnDestroy()
        {
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
        }

        private void OnNewDayStarted()
        {
            GameStatic.VehicleHealth = 1;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.collider.CompareTag(GameStatic.ObstacleTag))
                return;
            
            if (Time.time - m_LastCollisionTime < immunityDurationAfterCollision)
                return; // Still in immunity period, ignore this collision
            
            m_LastCollisionTime = Time.time;
            
            var collisionImpact = other.relativeVelocity.magnitude;
            var healthDecrease = Mathf.Lerp(minHealthDecreaseOnCollision, maxHealthDecreaseOnCollision, collisionImpact / 10f);
            GameStatic.VehicleHealth = Mathf.Max(0, GameStatic.VehicleHealth - healthDecrease);
            GameStatic.OnVehicleCollidedWithObstacle?.Invoke();
            
            Debug.Log($"Vehicle collided with {other.collider.name}. Impact: {collisionImpact:F2}, Health Decrease: {healthDecrease:F2}, Remaining Health: {GameStatic.VehicleHealth:F2}");
            
            if (GameStatic.VehicleHealth < minValidHealth)
            {
                GameStatic.VehicleHealth = 0;
                GameStatic.OnVehicleDestroyed?.Invoke();
            }
        }
    }
}