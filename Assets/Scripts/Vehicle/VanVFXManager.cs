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
        }
        
        [SerializeField] private List<BiomeFeedback> biomeFeedbacks;

        private void Awake()
        {
            GameStatic.OnBiomeChanged += OnBiomeChanged;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnBiomeChanged -= OnBiomeChanged;
        }

        private void OnBiomeChanged(BiomeType biomeType)
        {
            ResetAllFeedbacks();
            var feedback = biomeFeedbacks.Find(b => b.biomeType == biomeType)?.feedback;
            if (feedback)
            {
                feedback.PlayFeedbacks();
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
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
