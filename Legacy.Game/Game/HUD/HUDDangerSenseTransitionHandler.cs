using System;
using UnityEngine;

namespace Legacy.Game.HUD
{
	public class HUDDangerSenseTransitionHandler : MonoBehaviour
	{
		private static GameObject s_runningFX;

		[SerializeField]
		private String m_EnableFXPrefabPathFromClosed = String.Empty;

		[SerializeField]
		private String m_EnableFXPrefabPathFromOpened = String.Empty;

		public void OnEnableFXFromClosed()
		{
			SpawnPrefab(m_EnableFXPrefabPathFromClosed);
		}

		public void OnEnableFXFromOpened()
		{
			SpawnPrefab(m_EnableFXPrefabPathFromOpened);
		}

		private void SpawnPrefab(String p_FXPrefabPath)
		{
			if (!String.IsNullOrEmpty(p_FXPrefabPath))
			{
				if (s_runningFX != null)
				{
					Destroy(s_runningFX);
				}
				GameObject gameObject = Helper.ResourcesLoad<GameObject>(p_FXPrefabPath);
				s_runningFX = Helper.Instantiate<GameObject>(gameObject, transform.parent.TransformPoint(gameObject.transform.localPosition), Quaternion.identity);
				s_runningFX.transform.localScale = transform.lossyScale;
			}
		}
	}
}
