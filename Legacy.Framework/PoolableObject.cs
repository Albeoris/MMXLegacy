using System;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("ClockStone/PoolableObject")]
public class PoolableObject : MonoBehaviour
{
	public Int32 maxPoolSize = 10;

	public Int32 preloadCount;

	public Boolean doNotDestroyOnLoad;

	public Boolean sendAwakeStartOnDestroyMessage = true;

	public Boolean sendPoolableActivateDeactivateMessages;

	internal Boolean _isAvailableForPooling;

	internal Boolean _createdWithPoolController;

	internal Boolean _destroyMessageFromPoolController;

	internal Boolean _wasPreloaded;

	internal Boolean _wasStartCalledByUnity;

	internal ObjectPoolController.ObjectPool _myPool;

	internal Int32 _serialNumber;

	internal Int32 _usageCount;

	protected void Start()
	{
		_wasStartCalledByUnity = true;
	}

	private static void _InvokeMethodByName(MonoBehaviour behaviour, String methodName)
	{
		Type type = behaviour.GetType();
		MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
		if (method != null)
		{
			method.Invoke(behaviour, null);
		}
	}

	private static void _BroadcastMessageToGameObject(GameObject go, String message)
	{
		Component[] components = go.GetComponents(typeof(MonoBehaviour));
		foreach (Component component in components)
		{
			_InvokeMethodByName((MonoBehaviour)component, message);
		}
		if (go.transform.childCount > 0)
		{
			_BroadcastMessageToAllChildren(go, message);
		}
	}

	private static void _BroadcastMessageToAllChildren(GameObject go, String message)
	{
		Transform[] array = new Transform[go.transform.childCount];
		for (Int32 i = 0; i < go.transform.childCount; i++)
		{
			array[i] = go.transform.GetChild(i);
		}
		for (Int32 j = 0; j < array.Length; j++)
		{
			if (array[j].GetComponent<PoolableObject>() == null)
			{
				_BroadcastMessageToGameObject(array[j].gameObject, message);
			}
		}
	}

	protected void OnDestroy()
	{
		if (!_destroyMessageFromPoolController && _myPool != null)
		{
			_myPool.Remove(this);
		}
		if (!_destroyMessageFromPoolController)
		{
			_BroadcastMessageToGameObject(gameObject, "OnPoolableInstanceDestroy");
		}
		_destroyMessageFromPoolController = false;
	}

	public Int32 GetSerialNumber()
	{
		return _serialNumber;
	}

	public Int32 GetUsageCount()
	{
		return _usageCount;
	}

	public Int32 DeactivateAllPoolableObjectsOfMyKind()
	{
		if (_myPool != null)
		{
			return _myPool._SetAllAvailable();
		}
		return 0;
	}

	public Boolean IsDeactivated()
	{
		return _isAvailableForPooling;
	}

	public PoolableObject[] GetAllPoolableObjectsOfMyKind(Boolean includeInactiveObjects)
	{
		if (_myPool != null)
		{
			return _myPool._GetAllObjects(includeInactiveObjects);
		}
		return null;
	}
}
