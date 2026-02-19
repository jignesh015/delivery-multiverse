using System;
using UnityEngine;
using DG.Tweening;

namespace DeliveryMultiverse
{
    public class PropObstacle : MonoBehaviour
    {
        public BiomeType biomeType;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private float disappearDelay = 1f;
        [SerializeField] private float collisionForceMultiplier = 1f;
        [SerializeField] private float upwardForceMultiplier = 0.5f;
        [SerializeField] private float gravityScale = 1f;
        
        private Rigidbody m_Rigidbody;
        private Collider m_Collider;

        private Vector3 m_InitialVisualScale;
        private Quaternion m_InitialRotation;

        private void Awake()
        {
            TryGetComponent(out m_Rigidbody);
            TryGetComponent(out m_Collider);
            m_InitialVisualScale = visualTransform.localScale;
            m_InitialRotation = transform.rotation;
        }

        private void OnEnable()
        {
            if (m_Rigidbody)
            {
                m_Rigidbody.linearVelocity = Vector3.zero;
                m_Rigidbody.angularVelocity = Vector3.zero;
                m_Rigidbody.WakeUp();
                Invoke(nameof(ResetIsKinematic), 0.1f); // Delay to ensure physics system is ready
            }
            if (m_Collider)
            {
                m_Collider.enabled = true;
            }

            visualTransform.localScale = m_InitialVisualScale;
            transform.rotation = m_InitialRotation;
        }
        
        private void ResetIsKinematic()
        {
            if (m_Rigidbody)
            {
                m_Rigidbody.isKinematic = false;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.collider.CompareTag(GameStatic.VehicleTag)) return;

            if (m_Collider)
            {
                m_Collider.enabled = false;
            }

            if (m_Rigidbody && other.contactCount > 0)
            {
                var contact = other.GetContact(0);
                var collisionDirection = contact.normal;
                var collisionImpact = other.relativeVelocity.magnitude;
        
                // Failsafe: Clamp collision impact to prevent extreme values
                collisionImpact = Mathf.Clamp(collisionImpact, 0f, 100f);
        
                var upwardBoost = Vector3.up * upwardForceMultiplier;
                var finalDirection = (collisionDirection + upwardBoost).normalized;
        
                // Failsafe: Check if the direction is valid (not NaN or Infinity)
                if (float.IsNaN(finalDirection.x) || float.IsNaN(finalDirection.y) || float.IsNaN(finalDirection.z) ||
                    float.IsInfinity(finalDirection.x) || float.IsInfinity(finalDirection.y) || float.IsInfinity(finalDirection.z))
                {
                    // Fallback to a safe upward direction if calculation fails
                    finalDirection = (Vector3.up + Vector3.forward).normalized;
                }
        
                var additionalForce = finalDirection * collisionImpact * collisionForceMultiplier * gravityScale;
                
                // Failsafe: Final check on the force vector before applying
                if (!float.IsNaN(additionalForce.x) && !float.IsNaN(additionalForce.y) && !float.IsNaN(additionalForce.z) &&
                    !float.IsInfinity(additionalForce.x) && !float.IsInfinity(additionalForce.y) && !float.IsInfinity(additionalForce.z))
                {
                    m_Rigidbody.AddForce(additionalForce, ForceMode.Impulse);
                }
            }

            Invoke(nameof(DestroyObstacle), disappearDelay);
        }


        private void DestroyObstacle()
        {
            visualTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                // Reset Rigidbody velocity to prevent unintended movement when reused
                if (m_Rigidbody)
                {
                    m_Rigidbody.linearVelocity = Vector3.zero;
                    m_Rigidbody.angularVelocity = Vector3.zero;
                    m_Rigidbody.Sleep();
                    m_Rigidbody.isKinematic = true; // Make kinematic to prevent physics interactions while disappearing
                }
                
                // Reset rotation before returning to pool
                transform.rotation = m_InitialRotation;
            });
        }

        private void OnDisable()
        {
            if (m_Rigidbody)
            {
                m_Rigidbody.linearVelocity = Vector3.zero;
                m_Rigidbody.angularVelocity = Vector3.zero;
                m_Rigidbody.Sleep();
                m_Rigidbody.isKinematic = true;
            }
            
            // Reset rotation before returning to pool
            transform.rotation = m_InitialRotation;
        }
    }
}