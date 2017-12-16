using System;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T s_Instance;

		public static T Instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = (FindObjectOfType(typeof(T)) as T);
					if (s_Instance == null)
					{
						GameObject gameObject = new GameObject("_Singleton " + typeof(T).FullName);
						DontDestroyOnLoad(gameObject);
						s_Instance = gameObject.AddComponent<T>();
					}
				}
				return s_Instance;
			}
		}

		protected virtual void Awake()
		{
			if (s_Instance != null && s_Instance != this)
			{
				Destroy(this);
				throw new Exception(typeof(UnitySingleton<T>) + "\nInstance already set! by -> " + s_Instance);
			}
			DontDestroyOnLoad(gameObject);
			s_Instance = (this as T);
		}

		protected virtual void OnDestroy()
		{
			s_Instance = null;
		}
	}
}
