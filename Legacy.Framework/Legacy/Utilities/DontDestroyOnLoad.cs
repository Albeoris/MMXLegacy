using System;
using Legacy.Core.Api;
using Legacy.Core.MapLoading;
using UnityEngine;

namespace Legacy.Utilities
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private static DontDestroyOnLoad s_Active;

		public Boolean IsDestroyed_OnSpawnPlayer;

		private void Awake()
		{
			if (s_Active != null)
			{
				Destroy(gameObject);
				return;
			}
			s_Active = this;
			DontDestroyOnLoad(gameObject);
			if (!IsDestroyed_OnSpawnPlayer)
			{
				Destroy(this);
			}
		}

		private void Update()
		{
			if (LegacyLogic.Instance.MapLoader.State > EMapLoaderState.LOADING_DYNAMIC_OBJECTS)
			{
				DestroyImmediate(gameObject);
			}
		}
	}
}
