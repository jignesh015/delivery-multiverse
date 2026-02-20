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
        
        private BiomeFeedback m_CurrentBiomeFeedback;

        private void Awake()
        {
            GameStatic.OnBiomeChanged += OnBiomeChanged;
            GameStatic.OnVehicleCollidedWithObstacle += OnVehicleCollidedWithObstacle;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnBiomeChanged -= OnBiomeChanged;
            GameStatic.OnVehicleCollidedWithObstacle -= OnVehicleCollidedWithObstacle;
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
