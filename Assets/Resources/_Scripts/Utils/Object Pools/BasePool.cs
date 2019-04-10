using UnityEngine;

/// <summary>
/// Abstract class for all object pools. Contains required methods and fields for object pools.
/// </summary>
public abstract class BasePool : MonoBehaviour
{
	/// <summary>
	/// Size of the object pool.
	/// </summary>
	protected int poolSize;

	/// <summary>
	/// The oldest index in the object pool.
	/// </summary>
	protected int oldest;

	/// <summary>
	/// Sets the size of the object pool.
	/// </summary>
	/// <param name="size"></param>
	public abstract void SetPoolSize(int size);

	/// <summary>
	/// Get an unused or LRU object in the object pool to use.
	/// </summary>
	/// <returns></returns>
	public abstract object GetPooledObject();
}
