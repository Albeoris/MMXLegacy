using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	internal class CheckResolution : MonoBehaviour
	{
		private Int32 m_ScreenWidth;

		private Int32 m_ScreenHeight;

		private void Update()
		{
			if (m_ScreenWidth != Screen.width || m_ScreenHeight != Screen.height)
			{
				m_ScreenWidth = Screen.width;
				m_ScreenHeight = Screen.height;
				UIRoot.Broadcast("OnResolutionChange");
			}
		}
	}
}
