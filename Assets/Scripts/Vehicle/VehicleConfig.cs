using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    [CreateAssetMenu(fileName = "VehicleConfig", menuName = "DeliveryMultiverse/Vehicle Config", order = 1)]
    public class VehicleConfig : ScriptableObject
    {
        public BiomeType biomeType;
        public float maxSpeed = 12f;
        public float reverseSpeedFactor = 0.75f;
        public float acceleration = 30f;
        public float drag = 0.5f;
        public float steeringSensitivity = 70f; 
        public float gravityMultiplier = 1f;
        public float jumpForce = 6f;
        public float extraLateralDrift;
    }
}
