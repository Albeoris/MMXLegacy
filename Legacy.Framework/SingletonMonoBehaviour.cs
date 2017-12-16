using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour, ISingletonMonoBehaviour where T : MonoBehaviour
{
	public static T Instance => UnitySingleton<T>.GetSingleton(true, true);

    public static T DoesInstanceExist()
	{
		return UnitySingleton<T>.GetSingleton(false, false);
	}

	public static void ActivateSingletonInstance()
	{
		UnitySingleton<T>.GetSingleton(true, true);
	}

	public static void SetSingletonAutoCreate(GameObject autoCreatePrefab)
	{
		UnitySingleton<T>._autoCreatePrefab = autoCreatePrefab;
	}

	public static void SetSingletonType(Type type)
	{
		UnitySingleton<T>._myType = type;
	}

	protected virtual void Awake()
	{
		if (isSingletonObject)
		{
			UnitySingleton<T>._Awake(this as T);
		}
	}

	protected virtual void OnDestroy()
	{
		if (isSingletonObject)
		{
			UnitySingleton<T>._Destroy();
		}
	}

	public virtual Boolean isSingletonObject => true;
}
