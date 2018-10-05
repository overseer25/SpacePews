using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{

    // The object pooled.
    public GameObject pooledObject;
    // How many of the object to pool.
    public int amountPooled = 20;

    // The internal list of objects.
    private List<GameObject> objectPool;

    /// <summary>
    /// Setup the instance.
    /// </summary>
    void Awake()
    {
        objectPool = new List<GameObject>(amountPooled);
    }

    /// <summary>
    /// Creates a pool of the object. This method exists so that the pool is created only when it is needed.
    /// </summary>
    public void CreatePool()
    {
        for (int i = 0; i < amountPooled; i++)
        {
            GameObject obj = Instantiate(pooledObject) as GameObject;
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    /// <summary>
    /// Destroy the object pool when it is no longer necessary.
    /// </summary>
    public void DestroyPool()
    {
        for(int i = 0; i < objectPool.Count; i++)
        {
            Destroy(objectPool[i].gameObject);
        }
    }

    /// <summary>
    /// Sets the object pool to use a new object.
    /// </summary>
    /// <param name="obj"></param>
    public void SetGameObject(GameObject newObject)
    {
        // Clear the old list.
        objectPool.Clear();

        for (int i = 0; i < amountPooled; i++)
        {
            GameObject obj = Instantiate(newObject) as GameObject;
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    /// <summary>
    /// Gets an unused object in the object pool, if it exists.
    /// </summary>
    /// <returns> Unused GameObject in object pool, or null if none exists. </returns>
    public GameObject GetPooledObject()
    {
        foreach (var obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}
