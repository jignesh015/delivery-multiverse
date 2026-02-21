using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine.InputSystem;

namespace DeliveryMultiverse
{
    public class BiomeSwitchManager : MonoBehaviour
    {
        [SerializeField] private List<BiomeConfig> biomeConfigs = new List<BiomeConfig>();
        [SerializeField] private Vector2 biomeSwitchIntervalRange = new Vector2(15f, 45f);
        
        [Space(10)]
        [SerializeField] private MMF_Player biomeSwitchFeedback;
        
        [Header("Input Actions for Testing (Editor Only)")]
        [SerializeField] private InputActionReference inputActionBiome1;
        [SerializeField] private InputActionReference inputActionBiome2;
        [SerializeField] private InputActionReference inputActionBiome3;

        private float m_TimeLeftToSwitchBiome = -1f;
        
        private void Awake()
        {
            GameStatic.OnNewDayStarted += OnNewDayStarted;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
        }

        private void OnNewDayStarted()
        {
            SwitchBiome(BiomeType.Normal);
        }

        private void Update()
        {
            #if UNITY_EDITOR
            // For testing purposes, allow switching biomes with number keys in the editor
            if (inputActionBiome1 && inputActionBiome1.action.triggered)
            {
                SwitchBiome(BiomeType.Normal);
            }
            else if (inputActionBiome2 && inputActionBiome2.action.triggered)
            {
                SwitchBiome(BiomeType.Water);
            }
            else if (inputActionBiome3 && inputActionBiome3.action.triggered)
            {
                SwitchBiome(BiomeType.Space);
            }
            #endif
            
            if(!GameStatic.CanSwitchBiome || Mathf.Approximately(m_TimeLeftToSwitchBiome, -1f) || !GameStatic.IsDayActive 
                || GameStatic.IsPlayingMinigame || Time.timeScale == 0) 
                return;

            m_TimeLeftToSwitchBiome -= Time.deltaTime;
            if (m_TimeLeftToSwitchBiome > 0f) return;
            m_TimeLeftToSwitchBiome = -1f;
            SwitchBiomeRandomly();
        }
        
        private void SwitchBiomeRandomly()
        {
            var biomeTypes = Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>().ToList();
            biomeTypes.Remove(GameStatic.CurrentBiome);
            var nextBiome = biomeTypes[UnityEngine.Random.Range(0, biomeTypes.Count)];
            biomeSwitchFeedback?.PlayFeedbacks();
            SwitchBiome(nextBiome);
        }
        
        private void SwitchBiome(BiomeType biomeType)
        {
            GameStatic.CurrentBiome = biomeType;
            
            foreach (var config in biomeConfigs)
            {
                config.biomeObj.SetActive(config.biomeType == biomeType);
            }
            
            GameStatic.OnBiomeChanged?.Invoke(biomeType);
            m_TimeLeftToSwitchBiome = UnityEngine.Random.Range(biomeSwitchIntervalRange.x, biomeSwitchIntervalRange.y);
        }
    }
    

    [Serializable]
    public class BiomeConfig
    {
        public BiomeType biomeType;
        public GameObject biomeObj;
    }

    public enum BiomeType
    {
        Normal,
        Water,
        Space
    }
}