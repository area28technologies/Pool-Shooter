using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
    public class ObjectPooling : SingletonMono<ObjectPooling>
    {
        public List<PoolObjectInfor> ObjectPools = new();

        public GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            if (objectToSpawn == null)
            {
                Debug.Log("Object To Spawn is null!");
                return null;
            }

            PoolObjectInfor pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

            //If the pool isn't exist, create it
            if (pool == null)
            {
                pool = new PoolObjectInfor() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }

            //Check if there are any inactive object in the pool
            GameObject spawnableObject = null;
            if (pool.InactiveObjects.Count > 0)
            {
                spawnableObject = pool.InactiveObjects.FirstOrDefault();
            }

            if (spawnableObject == null)
            {
                spawnableObject = Instantiate(objectToSpawn, parent);
                spawnableObject.transform.SetLocalPositionAndRotation(spawnPosition, spawnRotation);
            }
            else
            {
                spawnableObject.transform.SetParent(parent);
                spawnableObject.transform.SetLocalPositionAndRotation(spawnPosition, spawnRotation);
                pool.InactiveObjects.Remove(spawnableObject);
                spawnableObject.SetActive(true);

            }

            spawnableObject.name = spawnableObject.name.Replace("(Clone)", "");
            return spawnableObject;
        }

        public void ReturnObjectToPool(GameObject objectToPool)
        {
            if (objectToPool == null)
            {
                Debug.Log("Object To Return is null!");
                return;
            }
            //Debug.Log("ReturnObjectToPool " + objectToPool.name);
            string objectToPoolName = objectToPool.name.Replace("(Clone)", "").Trim();
            PoolObjectInfor pool = ObjectPools.Find(p => p.LookupString.Equals(objectToPoolName));

            //If the pool isn't exist, create it
            if (pool == null)
            {
                Debug.LogWarning("Release an object that is not pooled!");
                pool = new PoolObjectInfor() { LookupString = objectToPool.name };
                ObjectPools.Add(pool);
            }
            else
            {
                objectToPool.SetActive(false);
                pool.InactiveObjects.Add(objectToPool);
            }
            objectToPool.transform.parent = Instance.transform;
        }

        public void ClearPool()
        {
            int count = 0;
            foreach (var pool in ObjectPools)
            {
                foreach (var obj in pool.InactiveObjects)
                {
                    Destroy(obj);
                    count++;
                }
            }
            ObjectPools = new();
            Debug.Log($"Remove {count} game objects from pool!");
        }
    }

    [Serializable]
    public class PoolObjectInfor
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new();

    }
}