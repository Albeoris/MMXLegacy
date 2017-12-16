using System;
using Legacy.Game.MMGUI;
using UnityEngine;

namespace Legacy.Game.Cheats
{
	[AddComponentMenu("MM Legacy/Cheats/CheatController")]
	public class CheatController : MonoBehaviour
	{
		private Boolean m_active;

		public void ToggleOpenClose()
		{
			m_active = !m_active;
			NGUITools.SetActiveSelf(gameObject, m_active);
			if (m_active)
			{
				DragDropManager.Instance.CancelDragAction();
			}
		}
	}
}
