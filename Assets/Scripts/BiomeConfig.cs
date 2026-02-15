using System;

namespace DeliveryMultiverse
{
    [Serializable]
    public class BiomeConfig
    {
        public float maxSpeed = 12f;
        public float acceleration = 30f;
        public float drag = 0.5f;
        public float steeringSensitivity = 70f; 
        public float gravityMultiplier = 1f;
        public float jumpForce = 6f;
        public float extraLateralDrift;
    }
}
