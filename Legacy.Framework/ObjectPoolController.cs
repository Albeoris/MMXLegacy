using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPoolController
{
	internal static Int32 _globalSerialNumber = 0;

	internal static Boolean _isDuringInstantiate = false;

	private static Dictionary<GameObject, ObjectPool> _pools = new Dictionary<GameObject, ObjectPool>();

	public static Boolean isDuringPreload { get; private set; }

	public static GameObject Instantiate(GameObject prefab)
	{
		PoolableObject component = prefab.GetComponent<PoolableObject>();
		if (component == null)
		{
			return (GameObject)UnityEngine.Object.Instantiate(prefab);
		}
		GameObject pooledInstance = _GetPool(component).GetPooledInstance(null, null);
		if (pooledInstance)
		{
			return pooledInstance;
		}
		return InstantiateWithoutPool(prefab);
	}

	public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion quaternion)
	{
		PoolableObject component = prefab.GetComponent<PoolableObject>();
		if (component == null)
		{
			return (GameObject)UnityEngine.Object.Instantiate(prefab, position, quaternion);
		}
		GameObject pooledInstance = _GetPool(component).GetPooledInstance(new Vector3?(position), new Quaternion?(quaternion));
		if (pooledInstance)
		{
			return pooledInstance;
		}
		return InstantiateWithoutPool(prefab, position, quaternion);
	}

	public static GameObject InstantiateWithoutPool(GameObject prefab)
	{
		return InstantiateWithoutPool(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
	}

	public static GameObject InstantiateWithoutPool(GameObject prefab, Vector3 position, Quaternion quaternion)
	{
		_isDuringInstantiate = true;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(prefab, position, quaternion);
		_isDuringInstantiate = false;
		PoolableObject component = gameObject.GetComponent<PoolableObject>();
		if (component)
		{
			if (component.doNotDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			component._createdWithPoolController = true;
			UnityEngine.Object.Destroy(component);
		}
		return gameObject;
	}

	public static void Destroy(GameObject obj)
	{
		PoolableObject component = obj.GetComponent<PoolableObject>();
		if (component == null)
		{
			_DetachChildrenAndDestroy(obj.transform);
			UnityEngine.Object.Destroy(obj);
			return;
		}
		if (component._myPool != null)
		{
			component._myPool._SetAvailable(component, true);
		}
		else
		{
			if (!component._createdWithPoolController)
			{
				Debug.LogWarning("Poolable object " + obj.name + " not created with ObjectPoolController");
			}
			UnityEngine.Object.Destroy(obj);
		}
	}

	public static void Preload(GameObject prefab)
	{
		PoolableObject component = prefab.GetComponent<PoolableObject>();
		if (component == null)
		{
			Debug.LogWarning("Can not preload because prefab '" + prefab.name + "' is not poolable");
			return;
		}
		ObjectPool objectPool = _GetPool(component);
		Int32 num = component.preloadCount - objectPool.GetObjectCount();
		if (num <= 0)
		{
			return;
		}
		isDuringPreload = true;
		try
		{
			for (Int32 i = 0; i < num; i++)
			{
				objectPool.PreloadInstance();
			}
		}
		finally
		{
			isDuringPreload = false;
		}
	}

	internal static ObjectPool _GetPool(PoolableObject prefabPoolComponent)
	{
		GameObject gameObject = prefabPoolComponent.gameObject;
		ObjectPool objectPool;
		if (!_pools.TryGetValue(gameObject, out objectPool))
		{
			objectPool = new ObjectPool(gameObject);
			_pools.Add(gameObject, objectPool);
		}
		return objectPool;
	}

	private static void _DetachChildrenAndDestroy(Transform transform)
	{
		Int32 childCount = transform.childCount;
		Transform[] array = new Transform[childCount];
		for (Int32 i = 0; i < childCount; i++)
		{
			array[i] = transform.GetChild(i);
		}
		transform.DetachChildren();
		for (Int32 i = 0; i < childCount; i++)
		{
			GameObject gameObject = array[i].gameObject;
			if (gameObject)
			{
				Destroy(gameObject);
			}
		}
	}

	internal class ObjectPool
	{
		private HashSet_Flash<PoolableObject> _pool;

		private PoolableObject _prefabPoolObj;

		private Transform _poolParentDummy;

		public ObjectPool(GameObject prefab)
		{
			_prefabPoolObj = prefab.GetComponent<PoolableObject>();
		}

		internal Transform poolParentDummy
		{
			get
			{
				_ValidatePoolParentDummy();
				return _poolParentDummy;
			}
		}

		private void _ValidatePoolParentDummy()
		{
			if (!_poolParentDummy)
			{
				GameObject gameObject = new GameObject("POOL:" + _prefabPoolObj.name);
				_poolParentDummy = gameObject.transform;
				_SetActive(gameObject, false);
				if (_prefabPoolObj.doNotDestroyOnLoad)
				{
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
			}
		}

		private void _ValidatePooledObjectDataContainer()
		{
			if (_pool == null)
			{
				_pool = new HashSet_Flash<PoolableObject>();
				_ValidatePoolParentDummy();
			}
		}

		internal void Remove(PoolableObject poolObj)
		{
			_pool.Remove(poolObj);
		}

		internal Int32 GetObjectCount()
		{
			if (_pool == null)
			{
				return 0;
			}
			return _pool.Count;
		}

		internal GameObject GetPooledInstance(Vector3? position, Quaternion? rotation)
		{
			_ValidatePooledObjectDataContainer();
			Transform transform = _prefabPoolObj.transform;
			foreach (PoolableObject poolableObject in _pool)
			{
				if (poolableObject != null && poolableObject._isAvailableForPooling)
				{
					Transform transform2 = poolableObject.transform;
					transform2.position = ((position == null) ? transform.position : position.Value);
					transform2.rotation = ((rotation == null) ? transform.rotation : rotation.Value);
					transform2.localScale = transform.localScale;
					poolableObject._usageCount++;
					_SetAvailable(poolableObject, false);
					return poolableObject.gameObject;
				}
			}
			if (_pool.Count < _prefabPoolObj.maxPoolSize)
			{
				return _NewPooledInstance(position, rotation).gameObject;
			}
			return null;
		}

		internal PoolableObject PreloadInstance()
		{
			_ValidatePooledObjectDataContainer();
			PoolableObject poolableObject = _NewPooledInstance(null, null);
			poolableObject._wasPreloaded = true;
			_SetAvailable(poolableObject, true);
			return poolableObject;
		}

		private PoolableObject _NewPooledInstance(Vector3? position, Quaternion? rotation)
		{
			_isDuringInstantiate = true;
			GameObject gameObject;
			if (position != null && rotation != null)
			{
				gameObject = (GameObject)UnityEngine.Object.Instantiate(_prefabPoolObj.gameObject, position.Value, rotation.Value);
			}
			else
			{
				gameObject = (GameObject)UnityEngine.Object.Instantiate(_prefabPoolObj.gameObject);
			}
			_isDuringInstantiate = false;
			PoolableObject component = gameObject.GetComponent<PoolableObject>();
			component._createdWithPoolController = true;
			component._myPool = this;
			component._isAvailableForPooling = false;
			component._serialNumber = ++_globalSerialNumber;
			component._usageCount++;
			if (component.doNotDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			_pool.Add(component);
			gameObject.BroadcastMessage("OnPoolableInstanceAwake", SendMessageOptions.DontRequireReceiver);
			return component;
		}

		internal Int32 _SetAllAvailable()
		{
			Int32 num = 0;
			foreach (PoolableObject poolableObject in _pool)
			{
				if (poolableObject != null && !poolableObject._isAvailableForPooling)
				{
					_SetAvailable(poolableObject, true);
					num++;
				}
			}
			return num;
		}

		internal PoolableObject[] _GetAllObjects(Boolean includeInactiveObjects)
		{
			List<PoolableObject> list = new List<PoolableObject>();
			foreach (PoolableObject poolableObject in _pool)
			{
				if (poolableObject != null && (includeInactiveObjects || !poolableObject._isAvailableForPooling))
				{
					list.Add(poolableObject);
				}
			}
			return list.ToArray();
		}

		internal void _SetAvailable(PoolableObject poolObj, Boolean b)
		{
			poolObj._isAvailableForPooling = b;
			Transform transform = poolObj.transform;
			if (b)
			{
				if (poolObj.sendAwakeStartOnDestroyMessage)
				{
					poolObj._destroyMessageFromPoolController = true;
				}
				transform.parent = null;
				_RecursivelySetInactiveAndSendMessages(poolObj.gameObject, poolObj, false);
				transform.parent = poolObj._myPool.poolParentDummy;
			}
			else
			{
				transform.parent = null;
				_SetActiveAndSendMessages(poolObj.gameObject, poolObj);
			}
		}

		private void _SetActive(GameObject obj, Boolean active)
		{
			obj.SetActive(active);
		}

		private Boolean _GetActive(GameObject obj)
		{
			return obj.activeInHierarchy;
		}

		private void _SetActiveAndSendMessages(GameObject obj, PoolableObject parentPoolObj)
		{
			_SetActive(obj, true);
			if (parentPoolObj.sendAwakeStartOnDestroyMessage)
			{
				obj.BroadcastMessage("Awake", null, SendMessageOptions.DontRequireReceiver);
				if (_GetActive(obj) && parentPoolObj._wasStartCalledByUnity)
				{
					obj.BroadcastMessage("Start", null, SendMessageOptions.DontRequireReceiver);
				}
			}
			if (parentPoolObj.sendPoolableActivateDeactivateMessages)
			{
				obj.BroadcastMessage("OnPoolableObjectActivated", null, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void _RecursivelySetInactiveAndSendMessages(GameObject obj, PoolableObject parentPoolObj, Boolean recursiveCall)
		{
			Transform transform = obj.transform;
			Transform[] array = new Transform[transform.childCount];
			for (Int32 i = 0; i < transform.childCount; i++)
			{
				array[i] = transform.GetChild(i);
			}
			foreach (Transform transform2 in array)
			{
				PoolableObject component = transform2.gameObject.GetComponent<PoolableObject>();
				if (component && component._myPool != null)
				{
					_SetAvailable(component, true);
				}
				else
				{
					_RecursivelySetInactiveAndSendMessages(transform2.gameObject, parentPoolObj, true);
				}
			}
			if (parentPoolObj.sendAwakeStartOnDestroyMessage)
			{
				obj.SendMessage("OnDestroy", null, SendMessageOptions.DontRequireReceiver);
			}
			if (parentPoolObj.sendPoolableActivateDeactivateMessages)
			{
				obj.SendMessage("OnPoolableObjectDeactivated", null, SendMessageOptions.DontRequireReceiver);
			}
			if (!recursiveCall)
			{
				_SetActive(obj, false);
			}
		}
	}
}
