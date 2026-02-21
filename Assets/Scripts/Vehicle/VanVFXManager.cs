using System;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains;
using MoreMountains.Feedbacks;

namespace DeliveryMultiverse
{
    public class VanVFXManager : MonoBehaviour
    {
        [Serializable]
        public class BiomeFeedback
        {
            public BiomeType biomeType;
            public MMF_Player feedback;
            public MMF_Player collisionFeedback;
        }
        
        [SerializeField] private List<BiomeFeedback> biomeFeedbacks;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private AudioSource deliveryPointInteractSfx;
        [SerializeField] private MMF_Player exclamationFeedback;
        
        private BiomeFeedback m_CurrentBiomeFeedback;
        private Vector3 m_InitialVisualPosition;

        private void Awake()
        {
            m_InitialVisualPosition = visualTransform.localPosition;
            
            GameStatic.OnBiomeChanged += OnBiomeChanged;
            GameStatic.OnVehicleCollidedWithObstacle += OnVehicleCollidedWithObstacle;
            GameStatic.OnPlayerInteractedWithDeliveryPoint += OnPlayerInteractedWithDeliveryPoint;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnBiomeChanged -= OnBiomeChanged;
            GameStatic.OnVehicleCollidedWithObstacle -= OnVehicleCollidedWithObstacle;
            GameStatic.OnPlayerInteractedWithDeliveryPoint -= OnPlayerInteractedWithDeliveryPoint;
        }

        private void OnPlayerInteractedWithDeliveryPoint(DeliveryPoint arg0)
        {
            deliveryPointInteractSfx?.Play();
        }

        private void OnVehicleCollidedWithObstacle(bool isPropObstacle)
        {
            if (isPropObstacle)
                return;
            
            m_CurrentBiomeFeedback?.collisionFeedback?.PlayFeedbacks();
        }

        private void OnBiomeChanged(BiomeType biomeType)
        {
            ResetAllFeedbacks();
            m_CurrentBiomeFeedback = biomeFeedbacks.Find(b => b.biomeType == biomeType);
            if (m_CurrentBiomeFeedback?.feedback)
            {
                m_CurrentBiomeFeedback?.feedback.PlayFeedbacks();
            }
            visualTransform.localPosition = m_InitialVisualPosition;
            
            if(biomeType == BiomeType.Normal)return;
            
            var prefKey = $"{GameStatic.HasSeenBiomePref}_{biomeType.ToString()}";
            if (PlayerPrefs.HasKey(prefKey)) return;
            exclamationFeedback?.PlayFeedbacks();
            PlayerPrefs.SetInt(prefKey, 1);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (var biomeFeedback in biomeFeedbacks)
            {
                biomeFeedback.feedback.Initialization();
                biomeFeedback.collisionFeedback.Initialization();
            }
            
            ResetAllFeedbacks();
        }

        private void ResetAllFeedbacks()
        {
            foreach (var biomeFeedback in biomeFeedbacks)
            {
                biomeFeedback.feedback.ResetFeedbacks();
            }
        }
    }
}
