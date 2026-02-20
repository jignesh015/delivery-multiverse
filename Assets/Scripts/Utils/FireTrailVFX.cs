using UnityEngine;

namespace DeliveryMultiverse
{
    public class FireTrailVFX : MonoBehaviour
    {
        [SerializeField] private Rigidbody vehicleRigidbody;
        [SerializeField] private Vector2 scaleRange = new Vector2(0.05f, 0.8f);
        [SerializeField] private float scaleLerpSpeed = 5f;

        private Vector3 m_NewScale;

        private void Update()
        {
            if (!vehicleRigidbody || !GameStatic.CurrentVehicleConfig) return;

            var normalizedSpeed = Mathf.InverseLerp(0f,
                GameStatic.CurrentVehicleConfig.maxSpeed,
                vehicleRigidbody.linearVelocity.magnitude);
            m_NewScale = Vector3.Lerp(m_NewScale, Vector3.one * Mathf.Lerp(scaleRange.x, scaleRange.y, normalizedSpeed),
                scaleLerpSpeed);
            transform.localScale = m_NewScale;
        }
    }
}