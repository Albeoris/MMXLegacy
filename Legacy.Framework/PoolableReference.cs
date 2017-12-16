using System;
using UnityEngine;
using Object = System.Object;

public class PoolableReference<T> where T : Component
{
	private PoolableObject _pooledObj;

	private Int32 _initialUsageCount;

	private T _objComponent;

	public PoolableReference()
	{
		Reset();
	}

	public PoolableReference(T componentOfPoolableObject)
	{
		Set(componentOfPoolableObject, false);
	}

	public PoolableReference(PoolableReference<T> poolableReference)
	{
		_objComponent = poolableReference._objComponent;
		_pooledObj = poolableReference._pooledObj;
		_initialUsageCount = poolableReference._initialUsageCount;
	}

	public void Reset()
	{
		_pooledObj = null;
		_objComponent = null;
		_initialUsageCount = 0;
	}

	public T Get()
	{
		if (!_objComponent)
		{
			return null;
		}
		if (_pooledObj && (_pooledObj._usageCount != _initialUsageCount || _pooledObj._isAvailableForPooling))
		{
			_objComponent = null;
			_pooledObj = null;
			return null;
		}
		return _objComponent;
	}

	public void Set(T componentOfPoolableObject)
	{
		Set(componentOfPoolableObject, false);
	}

	public void Set(T componentOfPoolableObject, Boolean allowNonePoolable)
	{
		if (!componentOfPoolableObject)
		{
			Reset();
			return;
		}
		_objComponent = componentOfPoolableObject;
		_pooledObj = _objComponent.GetComponent<PoolableObject>();
		if (!_pooledObj)
		{
			if (!allowNonePoolable)
			{
				Debug.LogError("Object for PoolableReference must be poolable");
				return;
			}
			_initialUsageCount = 0;
		}
		else
		{
			_initialUsageCount = _pooledObj._usageCount;
		}
	}
}
