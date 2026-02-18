using System;
using UnityEngine;
using DG.Tweening;

namespace DeliveryMultiverse
{
    public class RepairKit : MonoBehaviour
    {
        [SerializeField] private float healAmount = 0.5f;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private float bobbingAmplitude = 0.25f;
        [SerializeField] private float bobbingFrequency = 1f;
        
        private Vector3 m_InitialVisualPosition;
        private Vector3 m_InitialVisualScale;

        private void Awake()
        {
            m_InitialVisualPosition = visualTransform.localPosition;
            m_InitialVisualScale = visualTransform.localScale;
        }

        private void OnEnable()
        {
            visualTransform.localScale = m_InitialVisualScale;
            visualTransform.localPosition = m_InitialVisualPosition;
        }

        private void Update()
        {
            var bobbingOffset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
            visualTransform.localPosition = m_InitialVisualPosition + Vector3.up * bobbingOffset;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameStatic.VehicleTag)) return;
            
            GameStatic.VehicleHealth = Mathf.Min(1, GameStatic.VehicleHealth + healAmount);
            GameStatic.OnVehicleCollectedRepairKit?.Invoke();

            visualTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                StaticPool.Return(this);
            });
        }
    }
}