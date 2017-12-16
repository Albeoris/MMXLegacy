using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	public class LoadingAnim : MonoBehaviour
	{
		[SerializeField]
		private UISprite[] m_sprites;

		[SerializeField]
		private Single m_alphaMin;

		[SerializeField]
		private Single m_alphaMax = 0.75f;

		[SerializeField]
		private Single m_circleTime = 1f;

		public void SetVisible(Boolean p_visible)
		{
			NGUITools.SetActiveSelf(gameObject, p_visible);
		}

		private void Update()
		{
			if (!enabled)
			{
				return;
			}
			for (Int32 i = 0; i < m_sprites.Length; i++)
			{
				Single num = (Int32)((Time.time + m_circleTime * i / m_sprites.Length) % m_circleTime / m_circleTime * m_sprites.Length);
				m_sprites[i].alpha = m_alphaMin + (m_alphaMax - m_alphaMin) * (1f - num / m_sprites.Length);
			}
		}
	}
}
