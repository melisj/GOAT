using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Pooling
{
    public class PoolManager : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();
        [SerializeField] private Dictionary<int, GameObject> parentDictionary = new Dictionary<int, GameObject>();
        private static PoolManager instance;

        public static PoolManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<PoolManager>();
                }
                return instance;
            }
        }

        /// <summary>
        /// Creates a new pool if the Prefab doesn't have one already
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="poolSize"></param>
        public void CreatePool(GameObject prefab, int poolSize, Transform parent = null)
        {
            int poolKey = prefab.GetInstanceID();

            if (!poolDictionary.ContainsKey(poolKey))
            {
                poolDictionary.Add(poolKey, new Queue<ObjectInstance>());
                GameObject poolHolder = new GameObject(prefab.name + " pool");
                poolHolder.transform.SetParent(transform);
                parentDictionary.Add(poolKey, poolHolder);

                AddToPool(prefab, poolSize, parent);
            }
        }

        /// <summary>
        /// Adds more objects to the pool of Prefab as it's not enough
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="amount"></param>
        private void AddToPool(GameObject prefab, int amount, Transform parent)
        {
            int poolKey = prefab.GetInstanceID();
            for (int i = 0; i < amount; i++)
            {
                ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject);
                poolDictionary[poolKey].Enqueue(newObject);
                newObject.SetParent(parent ? parent : parentDictionary[poolKey].transform);
            }
        }

        /// <summary>
        /// Gets the pooled object from the dictionary
        /// Makes a new pool if there wasn't one already
        /// Adds more to the existing pool if the Queue is empty
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns>The pooled object</returns>
        public GameObject GetFromPool(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            int poolKey = prefab.GetInstanceID();
            if (!poolDictionary.ContainsKey(poolKey))
            {
                CreatePool(prefab, 1, parent);
            }
            else
            {
                if (poolDictionary[poolKey].Count == 0)
                {
                    AddToPool(prefab, 1, parent);
                }
            }

            ObjectInstance objInstance = poolDictionary[poolKey].Dequeue();
            //poolDictionary[poolKey].Enqueue(objInstance);

            objInstance.SetParent(parent ? parent : parentDictionary[poolKey].transform);
            objInstance.GetObject(pos, rot, poolKey);

            return objInstance.GameObject;
        }

        /// <summary>
        /// Gets the pooled object from the dictionary
        /// Makes a new pool if there wasn't one already
        /// Adds more to the existing pool if the Queue is empty
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns>The pooled object</returns>
        public GameObject GetFromPool(GameObject prefab, Transform parent = null)
        {
            int poolKey = prefab.GetInstanceID();
            if (!poolDictionary.ContainsKey(poolKey))
            {
                CreatePool(prefab, 1, parent);
            }
            else
            {
                if (poolDictionary[poolKey].Count == 0)
                {
                    AddToPool(prefab, 1, parent);
                }
            }

            ObjectInstance objInstance = poolDictionary[poolKey].Dequeue();
            //poolDictionary[poolKey].Enqueue(objInstance);

            objInstance.SetParent(parent ? parent : parentDictionary[poolKey].transform);
            objInstance.GetObject(Vector3.zero, Quaternion.identity, poolKey);

            return objInstance.GameObject;
        }

        /// <summary>
        /// Adds objToReturn back to the Queue
        /// </summary>
        /// <param name="objToReturn"></param>
        public void ReturnToPool(GameObject objToReturn)
        {
            IPoolObject poolObject = objToReturn.GetComponent<IPoolObject>();

            if (poolObject == null)
            {
                Debug.LogErrorFormat("Object: {0} has no <b>IPoolObject</b>", objToReturn.name);
                return;
            }

            if (poolDictionary.ContainsKey(poolObject.PoolKey))
            {
                //Check if the object was already added before
                if (!poolDictionary[poolObject.PoolKey].Contains(poolObject.ObjInstance))
                {
                    poolDictionary[poolObject.PoolKey].Enqueue(poolObject.ObjInstance);
                }

                poolObject.OnReturnObject();
            }
            else
            {
                Debug.LogErrorFormat("Object: {0} with key {1} doesn't exist in dictionary", objToReturn.name, poolObject.PoolKey);
            }
        }

        public void SetParent(GameObject obj)
        {
            IPoolObject poolObject = obj.GetComponent<IPoolObject>();

            if (poolObject == null)
            {
                Debug.LogErrorFormat("Object: {0} has no <b>IPoolObject</b>", obj.name);
                return;
            }

            if (poolDictionary.ContainsKey(poolObject.PoolKey) && parentDictionary.ContainsKey(poolObject.PoolKey))
            {
                obj.transform.SetParent(parentDictionary[poolObject.PoolKey].transform);
                obj.transform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogErrorFormat("Object: {0} with key {1} doesn't exist in dictionary", obj.name, poolObject.PoolKey);
            }
        }

        public GameObject ReturnPoolParent(int poolKey)
        {
            parentDictionary.TryGetValue(poolKey, out GameObject value);
            return value;
        }
    }
}