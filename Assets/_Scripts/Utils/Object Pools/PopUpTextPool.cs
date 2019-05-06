using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpTextPool : BasePool
{

    // The instance.
    public static PopUpTextPool current;
    // The object pooled.
    public PopUpText defaultPopUp;

    // The internal list of objects.
    private List<PopUpText> popUpPool;

    /// <summary>
    /// Setup the instance.
    /// </summary>
    void Awake()
    {
        popUpPool = new List<PopUpText>();
		if(current == null)
			current = this;
    }

	public override void SetPoolSize(int size)
	{
		if (popUpPool == null)
			popUpPool = new List<PopUpText>();

		if (size == poolSize)
			return;

		int difference = poolSize - size;

		if (size > poolSize)
		{
			// Add on to the list.
			for (int i = poolSize; i < size; i++)
			{
				var proj = Instantiate(defaultPopUp, gameObject.transform);
				proj.gameObject.SetActive(false);
				popUpPool.Add(proj);
			}
		}
		else
		{
			// Subtract from the list.
			for (int i = poolSize - 1; i >= poolSize - difference; i--)
			{
				Destroy(popUpPool[popUpPool.Count - 1].gameObject);
				popUpPool.RemoveAt(popUpPool.Count - 1);
			}
		}

		poolSize = popUpPool.Count;
		if (oldest > poolSize)
			oldest = 0;
	}

	/// <summary>
	/// Gets an unused object in the object pool, if it exists.
	/// </summary>
	/// <returns> Unused GameObject in object pool, or null if none exists. </returns>
	public override object GetPooledObject()
    {
        foreach (var obj in popUpPool)
        {
            if (obj != null && !obj.gameObject.activeInHierarchy)
            {
                return obj;
            }
        }

		// Otherwise, get the least recently used object in the pool.
		var result = popUpPool[oldest++];
		oldest %= poolSize;
		return result;
	}
}

