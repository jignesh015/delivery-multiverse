using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class NavigationalPointerUI : MonoBehaviour
    {
        private Transform m_PointerPivot;
        private CanvasGroup m_CanvasGroup;

        private void Awake()
        {
            m_PointerPivot = transform;
            TryGetComponent(out m_CanvasGroup);
            TogglePointerVisibility(false);
            GameStatic.OnDeliveryPointAssigned += OnDeliveryPointAssigned;
        }

        private void OnDestroy()
        {
            GameStatic.OnDeliveryPointAssigned -= OnDeliveryPointAssigned;
        }

        private void Update()
        {
            if (!GameStatic.CurrentDeliveryPoint) return;
            var parent = m_PointerPivot.parent;
            var directionToTarget = GameStatic.CurrentDeliveryPoint.transform.position - m_PointerPivot.position;
            // Transform direction to local space of the parent
            var localDirection = parent ? parent.InverseTransformDirection(directionToTarget) : directionToTarget;
            // Project onto XY plane in local space (ignore local Z)
            localDirection.z = 0f;
            if (!(localDirection.sqrMagnitude > 0.001f)) return;
            var targetLocalRotation = Quaternion.LookRotation(Vector3.forward, localDirection);
            // Slerp only the local Z rotation
            var currentLocalEuler = m_PointerPivot.localRotation.eulerAngles;
            var targetLocalEuler = targetLocalRotation.eulerAngles;
            var newZ = Mathf.LerpAngle(currentLocalEuler.z, targetLocalEuler.z, Time.deltaTime * 5f);
            m_PointerPivot.localRotation = Quaternion.Euler(0f, 0f, newZ);
        }

        private void TogglePointerVisibility(bool isVisible)
        {
            if (m_CanvasGroup)
                m_CanvasGroup.alpha = isVisible ? 1f : 0f;
        }

        private void OnDeliveryPointAssigned(DeliveryPoint arg0)
        {
            TogglePointerVisibility(true);
        }
    }
}