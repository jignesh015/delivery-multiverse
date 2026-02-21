using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class PropPlacementManager : MonoBehaviour
    {
        [SerializeField] private List<BoxCollider> propPlacementZones = new List<BoxCollider>();
        [SerializeField] private RepairKit repairKitPrefab;
        [SerializeField] private List<PropObstacle> propObstaclePrefabs = new List<PropObstacle>();
        [SerializeField] private int repairKitsPerDay = 3;
        [SerializeField] private int obstalcePoolSize = 10;
        [SerializeField] private Vector2Int propObstacleSpawnRange = new Vector2Int(3, 6);
        [SerializeField] private float spawnPositionYOffset = 0f;

        private List<RepairKit> m_SpawnedRepairKits = new List<RepairKit>();
        private List<PropObstacle> m_SpawnedObstacles = new List<PropObstacle>();
        private HashSet<BoxCollider> m_OccupiedZones = new HashSet<BoxCollider>();

        private void Awake()
        {
            GameStatic.OnBiomeChanged += OnBiomeChanged;
            GameStatic.OnNewDayStarted += OnNewDayStarted;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnBiomeChanged -= OnBiomeChanged;
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
            StaticPool.ClearAllPools();
        }

        private void Start()
        {
            propPlacementZones = transform.GetComponentsInChildren<BoxCollider>(false).ToList();
            
            // Create a pool of repair kits
            StaticPool.CreatePool(repairKitPrefab.gameObject, repairKitsPerDay);

            // Create a pool for each prop obstacle prefab
            foreach (var prefab in propObstaclePrefabs)
            {
                StaticPool.CreatePool(prefab.gameObject, obstalcePoolSize);
            }
        }
        
        private void SpawnRepairKits()
        {
            // Return previously spawned repair kits to pool
            foreach (var repairKit in m_SpawnedRepairKits)
            {
                if (repairKit && repairKit.gameObject.activeSelf)
                {
                    StaticPool.Return(repairKit);
                }
            }
            m_SpawnedRepairKits.Clear();
            
            // Clear occupied zones from repair kits
            m_OccupiedZones.Clear();
            
            // Create a randomized list of placement zones
            List<BoxCollider> availableZones = new List<BoxCollider>(propPlacementZones);
            ShuffleList(availableZones);
            
            // Spawn repair kits up to repairKitsPerDay
            int spawnedCount = 0;
            foreach (var zone in availableZones)
            {
                if (spawnedCount >= repairKitsPerDay)
                    break;
                    
                if (!m_OccupiedZones.Contains(zone))
                {
                    Vector3 spawnPosition = GetRandomPositionInBounds(zone.bounds);
                    RepairKit repairKit = StaticPool.Fetch<RepairKit>(repairKitPrefab.gameObject);
                    repairKit.transform.position = spawnPosition;
                    
                    m_SpawnedRepairKits.Add(repairKit);
                    m_OccupiedZones.Add(zone);
                    spawnedCount++;
                }
            }
        }
        
        private void SpawnPropObstacles(BiomeType biomeType)
        {
            // Return all previously spawned obstacles to pool
            foreach (var obstacle in m_SpawnedObstacles)
            {
                if (obstacle && obstacle.gameObject.activeSelf)
                {
                    StaticPool.Return(obstacle);
                }
            }
            m_SpawnedObstacles.Clear();
            
            // Clear occupied zones (obstacles can coexist with repair kits but not with each other)
            m_OccupiedZones.Clear();
            
            // Get obstacles matching current biome
            var biomePrefabs = propObstaclePrefabs.FindAll(p => p.biomeType == biomeType);
            if (biomePrefabs.Count == 0)
                return;
            
            // Determine random spawn count
            var spawnCount = UnityEngine.Random.Range(propObstacleSpawnRange.x, propObstacleSpawnRange.y + 1);
            
            // Create a randomized list of placement zones
            var availableZones = new List<BoxCollider>(propPlacementZones);
            ShuffleList(availableZones);
            
            // Spawn obstacles
            int spawnedCount = 0;
            foreach (var zone in availableZones)
            {
                if (spawnedCount >= spawnCount)
                    break;
                
                if (!m_OccupiedZones.Contains(zone))
                {
                    // Pick a random obstacle prefab from the biome-specific list
                    PropObstacle randomPrefab = biomePrefabs[UnityEngine.Random.Range(0, biomePrefabs.Count)];
                    
                    Vector3 spawnPosition = GetRandomPositionInBounds(zone.bounds);
                    PropObstacle obstacle = StaticPool.Fetch<PropObstacle>(randomPrefab.gameObject);
                    obstacle.transform.position = spawnPosition;
                    
                    m_SpawnedObstacles.Add(obstacle);
                    m_OccupiedZones.Add(zone);
                    spawnedCount++;
                }
            }
        }
        
        private Vector3 GetRandomPositionInBounds(Bounds bounds)
        {
            var randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            var randomZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
            return new Vector3(randomX, spawnPositionYOffset, randomZ);
        }
        
        private void ShuffleList<T>(List<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var randomIndex = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        private void OnNewDayStarted()
        {
            SpawnRepairKits();
        }


        private void OnBiomeChanged(BiomeType biomeType)
        {
            SpawnPropObstacles(biomeType);
        }
    }
}