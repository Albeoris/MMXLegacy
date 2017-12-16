using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Legacy.HUD
{
	[AddComponentMenu("MM Legacy/MMGUI/HUDTextProvider")]
	public class HUDTextProvider : MonoBehaviour
	{
		private static HUDTextProvider s_Instance;

		private static Dictionary<Object, GameObject> s_HUDTextDemageMap = new Dictionary<Object, GameObject>();

		public static HUDDamageText CreateHUDDamageText(Object owner, Transform anchor)
		{
			if (s_Instance == null || owner == null || anchor == null || s_HUDTextDemageMap.ContainsKey(owner))
			{
				return null;
			}
			GameObject gameObject = Helper.ResourcesLoad<GameObject>("GuiPrefabs/HUDDamageText");
			gameObject = NGUITools.AddChild(s_Instance.gameObject, gameObject);
			s_HUDTextDemageMap.Add(owner, gameObject);
			HUDDamageText component = gameObject.GetComponent<HUDDamageText>();
			component.TargetAnchor = anchor;
			return component;
		}

		public static void DestroyHUDDamageText(Object owner)
		{
			if (s_Instance == null || owner == null || !s_HUDTextDemageMap.ContainsKey(owner))
			{
				return;
			}
			GameObject obj;
			if (s_HUDTextDemageMap.TryGetValue(owner, out obj))
			{
				Destroy(obj);
				s_HUDTextDemageMap.Remove(owner);
			}
		}

		private void Awake()
		{
			if (s_Instance != null)
			{
				throw new Exception("HUDTextProvider\nInstance already set! by -> " + s_Instance);
			}
			s_Instance = this;
		}
	}
}
