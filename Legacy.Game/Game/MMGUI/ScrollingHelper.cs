using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ScrollingHelper")]
	public class ScrollingHelper : MonoBehaviour
	{
		private IScrollingListener m_listener;

		public static void InitScrollListeners(IScrollingListener p_listener, GameObject p_go)
		{
			ScrollingHelper[] componentsInChildren = p_go.GetComponentsInChildren<ScrollingHelper>();
			for (Int32 i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Init(p_listener);
			}
			ScrollingHelper component = p_go.GetComponent<ScrollingHelper>();
			if (component != null)
			{
				component.Init(p_listener);
			}
		}

		public void Init(IScrollingListener p_listener)
		{
			m_listener = p_listener;
		}

		private void OnScroll(Single p_delta)
		{
			if (m_listener != null)
			{
				m_listener.OnScroll(p_delta);
			}
		}
	}
}
