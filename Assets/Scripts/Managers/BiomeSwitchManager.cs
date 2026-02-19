using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

namespace DeliveryMultiverse
{
    public class BiomeSwitchManager : MonoBehaviour
    {
        [SerializeField] private List<BiomeConfig> biomeConfigs = new List<BiomeConfig>();
        
        [Header("Input Actions for Testing (Editor Only)")]
        [SerializeField] private InputActionReference inputActionBiome1;
        [SerializeField] private InputActionReference inputActionBiome2;
        [SerializeField] private InputActionReference inputActionBiome3;

        private void Awake()
        {
            GameStatic.OnNewDayStarted += OnNewDayStarted;
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
        }

        private void OnDeliveryCompleted(DeliveryPoint arg0, int arg1)
        {
            var biomeTypes = Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>().ToList();
            biomeTypes.Remove(GameStatic.CurrentBiome);
            var nextBiome = biomeTypes[UnityEngine.Random.Range(0, biomeTypes.Count)];
            SwitchBiome(nextBiome);
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
        }
        
        private void SwitchBiome(BiomeType biomeType)
        {
            GameStatic.CurrentBiome = biomeType;
            
            foreach (var config in biomeConfigs)
            {
                config.biomeObj.SetActive(config.biomeType == biomeType);
            }
            
            GameStatic.OnBiomeChanged?.Invoke(biomeType);
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