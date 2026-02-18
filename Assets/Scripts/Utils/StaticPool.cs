using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DeliveryMultiverse
{
    public static class StaticPool
    {
        private static readonly Dictionary<GameObject, IObjectPool<GameObject>> pools = new Dictionary<GameObject, IObjectPool<GameObject>>();

        // Create a pool for a given GameObject prefab with initial size
        public static void CreatePool(GameObject prefab, int initialSize = 10, int maxSize = 100)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab), "Prefab cannot be null.");

            if (pools.ContainsKey(prefab))
                return; // Pool already exists

            var pool = new ObjectPool<GameObject>(
                createFunc: () => {
                    var instance = UnityEngine.Object.Instantiate(prefab);
                    instance.SetActive(false);
                    
                    // Attach PooledObject component to track the prefab reference
                    var pooledObject = instance.GetComponent<PooledObject>();
                    if (pooledObject == null)
                    {
                        pooledObject = instance.AddComponent<PooledObject>();
                    }
                    pooledObject.prefab = prefab;
                    
                    return instance;
                },
                actionOnGet: (obj) => {
                    obj.SetActive(true);
                },
                actionOnRelease: (obj) => {
                    obj.SetActive(false);
                },
                actionOnDestroy: (obj) => {
                    if (obj != null)
                        UnityEngine.Object.Destroy(obj);
                },
                collectionCheck: true,
                defaultCapacity: initialSize,
                maxSize: maxSize
            );

            pools[prefab] = pool;

            // Pre-warm the pool by creating initial objects
            var temp = new List<GameObject>(initialSize);
            for (int i = 0; i < initialSize; i++)
            {
                temp.Add(pool.Get());
            }
            foreach (var obj in temp)
            {
                pool.Release(obj);
            }
        }

        // Fetch a GameObject from the pool (creates pool if it doesn't exist)
        public static GameObject Fetch(GameObject prefab)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab), "Prefab cannot be null.");

            if (!pools.ContainsKey(prefab))
            {
                CreatePool(prefab);
            }

            return pools[prefab].Get();
        }

        // Fetch a component of type T from the pooled GameObject
        public static T Fetch<T>(GameObject prefab) where T : Component
        {
            var obj = Fetch(prefab);
            var component = obj.GetComponent<T>();
            if (component == null)
                throw new InvalidOperationException($"Prefab does not have component of type {typeof(T)}.");
            return component;
        }

        // Return a GameObject back to its pool using prefab reference
        public static void Return(GameObject prefab, GameObject instance)
        {
            if (prefab == null || instance == null)
                return;

            if (!pools.ContainsKey(prefab))
            {
                Debug.LogWarning($"No pool exists for prefab {prefab.name}. Destroying instance instead.");
                UnityEngine.Object.Destroy(instance);
                return;
            }

            pools[prefab].Release(instance);
        }

        // Return a GameObject back to its pool (automatically finds prefab from PooledObject component)
        public static void Return(GameObject instance)
        {
            if (instance == null)
                return;

            var pooledObject = instance.GetComponent<PooledObject>();
            if (pooledObject == null || pooledObject.prefab == null)
            {
                Debug.LogWarning($"GameObject {instance.name} does not have a PooledObject component or prefab reference. Destroying instead.");
                UnityEngine.Object.Destroy(instance);
                return;
            }

            Return(pooledObject.prefab, instance);
        }

        // Return a component's GameObject back to its pool using prefab reference
        public static void Return<T>(GameObject prefab, T component) where T : Component
        {
            if (component != null)
                Return(prefab, component.gameObject);
        }

        // Return a component's GameObject back to its pool (automatically finds prefab from PooledObject component)
        public static void Return<T>(T component) where T : Component
        {
            if (component != null)
                Return(component.gameObject);
        }

        // Clear a specific pool
        public static void ClearPool(GameObject prefab)
        {
            if (pools.ContainsKey(prefab))
            {
                pools[prefab].Clear();
                pools.Remove(prefab);
            }
        }

        // Clear all pools
        public static void ClearAllPools()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
            pools.Clear();
        }
    }
}