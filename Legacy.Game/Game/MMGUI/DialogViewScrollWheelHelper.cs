using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/DialogViewScrollWheelHelper")]
	public class DialogViewScrollWheelHelper : MonoBehaviour
	{
		[SerializeField]
		private UIScrollBar m_scrollbar;

		public void OnScroll(Single p_delta)
		{
			m_scrollbar.scrollValue = m_scrollbar.scrollValue - p_delta;
		}
	}
}
