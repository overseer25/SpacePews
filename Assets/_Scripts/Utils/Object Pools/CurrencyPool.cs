using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPool : BasePool
{
	[SerializeField]
	private Currency defaultCurrency;

	// The internal list of objects.
	private List<Currency> currencyPool;

	public static CurrencyPool current;

	private void Awake()
	{
		if (current == null)
			current = this;
	}

	public override void SetPoolSize(int size)
	{
		if (currencyPool == null)
			currencyPool = new List<Currency>();

		if (size == poolSize)
			return;

		int difference = poolSize - size;

		if (size > poolSize)
		{
			// Add on to the list.
			for (int i = poolSize; i < size; i++)
			{
				var currency = Instantiate(defaultCurrency, gameObject.transform);
				currency.gameObject.SetActive(false);
				currencyPool.Add(currency);
			}
		}
		else
		{
			// Subtract from the list.
			for (int i = poolSize - 1; i >= poolSize - difference; i--)
			{
				Destroy(currencyPool[currencyPool.Count - 1].gameObject);
				currencyPool.RemoveAt(currencyPool.Count - 1);
			}
		}

		poolSize = currencyPool.Count;
		if (oldest > poolSize)
			oldest = 0;
	}

	public override object GetPooledObject()
	{
		foreach (var obj in currencyPool)
		{
			if (!obj.gameObject.activeInHierarchy)
			{
				return obj;
			}
		}

		// Otherwise, get the least recently used object in the pool.
		var result = currencyPool[oldest++];
		oldest %= poolSize;
		return result;
	}
}
