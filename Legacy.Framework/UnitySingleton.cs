using System;
using UnityEngine;
using Object = System.Object;

public class UnitySingleton<T> where T : MonoBehaviour
{
	private static T _instance;

	internal static Type _myType = typeof(T);

	internal static GameObject _autoCreatePrefab;

	private static Int32 _GlobalInstanceCount = 0;

	private static Boolean _awakeSingletonCalled = false;

	private UnitySingleton()
	{
	}

	public static T GetSingleton(Boolean throwErrorIfNotFound, Boolean autoCreate)
	{
		if (!_instance)
		{
			UnityEngine.Object @object = null;
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(_myType);
			foreach (UnityEngine.Object object2 in array)
			{
				ISingletonMonoBehaviour singletonMonoBehaviour = (ISingletonMonoBehaviour)object2;
				if (singletonMonoBehaviour.isSingletonObject)
				{
					@object = (UnityEngine.Object)singletonMonoBehaviour;
					break;
				}
			}
			if (!@object)
			{
				if (!autoCreate || !(_autoCreatePrefab != null))
				{
					if (throwErrorIfNotFound)
					{
						Debug.LogError("No singleton component " + _myType.Name + " found in the scene.");
					}
					return null;
				}
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(_autoCreatePrefab);
				gameObject.name = _autoCreatePrefab.name;
				UnityEngine.Object exists = UnityEngine.Object.FindObjectOfType(_myType);
				if (!exists)
				{
					Debug.LogError("Auto created object does not have component " + _myType.Name);
					return null;
				}
			}
			else
			{
				_AwakeSingleton(@object as T);
			}
			_instance = (T)@object;
		}
		return _instance;
	}

	internal static void _Awake(T instance)
	{
		_GlobalInstanceCount++;
		if (_GlobalInstanceCount > 1)
		{
			Debug.LogError("More than one instance of SingletonMonoBehaviour " + typeof(T).Name);
		}
		else
		{
			_instance = instance;
		}
		_AwakeSingleton(instance);
	}

	internal static void _Destroy()
	{
		if (_GlobalInstanceCount > 0)
		{
			_GlobalInstanceCount--;
			if (_GlobalInstanceCount == 0)
			{
				_awakeSingletonCalled = false;
				_instance = null;
			}
		}
	}

	private static void _AwakeSingleton(T instance)
	{
		if (!_awakeSingletonCalled)
		{
			_awakeSingletonCalled = true;
			instance.SendMessage("AwakeSingleton", SendMessageOptions.DontRequireReceiver);
		}
	}
}
