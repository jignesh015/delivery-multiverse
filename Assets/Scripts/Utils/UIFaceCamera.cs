using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIFaceCamera : MonoBehaviour
    {
        private CanvasGroup m_CanvasGroup;
        private Camera m_MainCamera;
        
        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
        }

        private void Start()
        {
            m_MainCamera = Camera.main;
        }

        private void Update()
        {
            if(m_CanvasGroup && m_CanvasGroup.alpha <= 0f) return; // Skip if invisible
            
            if (!m_MainCamera)
            {
                m_MainCamera = Camera.main; 
                if (!m_MainCamera) return; 
            }
            
            transform.LookAt(transform.position + m_MainCamera.transform.rotation * Vector3.forward, 
                m_MainCamera.transform.rotation * Vector3.up);
        }
    }
}