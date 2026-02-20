using UnityEngine;

namespace DeliveryMultiverse
{
    public class SpaceRollVFX : MonoBehaviour
    {
        [SerializeField] private Rigidbody vehicleRigidbody;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float maxRollAngle = 45f;
        [SerializeField] private float rollSpeed = 5f;
        [Tooltip("Degrees/sec of yaw that maps to full maxRollAngle. Lower = more sensitive.")]
        [SerializeField] private float yawSensitivity = 60f;

        private Vector3 m_InitialRotation;
        private float m_CurrentRoll;
        private float m_PrevYaw;

        private void Awake()
        {
            m_InitialRotation = targetTransform.localEulerAngles;
            m_PrevYaw = vehicleRigidbody ? vehicleRigidbody.transform.eulerAngles.y : 0f;
            
            GameStatic.OnBiomeChanged += OnBiomeChanged;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnBiomeChanged -= OnBiomeChanged;
        }

        private void OnBiomeChanged(BiomeType biomeType)
        {
            if (biomeType != BiomeType.Space)
            {
                targetTransform.localEulerAngles = m_InitialRotation;
            }
        }

        private void Update()
        {
            if (!vehicleRigidbody || GameStatic.CurrentBiome != BiomeType.Space) return;

            // Measure yaw change this frame (degrees), unwrap to avoid ±180 discontinuities
            float currentYaw = vehicleRigidbody.transform.eulerAngles.y;
            float yawDelta = Mathf.DeltaAngle(m_PrevYaw, currentYaw);
            m_PrevYaw = currentYaw;

            // Convert to degrees/sec and normalise against sensitivity
            float yawRate = (Time.deltaTime > 0f) ? yawDelta / Time.deltaTime : 0f;
            float normalised = Mathf.Clamp(yawRate / yawSensitivity, -1f, 1f);

            // Right turn (positive yaw) → roll left (negative Z), left turn → roll right
            float targetRoll = -normalised * maxRollAngle;

            // Smoothly interpolate current roll
            m_CurrentRoll = Mathf.Lerp(m_CurrentRoll, targetRoll, Time.deltaTime * rollSpeed);

            // Apply roll as an offset on top of the initial rotation
            var euler = m_InitialRotation;
            euler.z += m_CurrentRoll;
            targetTransform.localEulerAngles = euler;
        }
    }
}