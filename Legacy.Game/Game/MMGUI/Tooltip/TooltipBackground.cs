using System;
using Legacy.Core.Configuration;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/TooltipBackground")]
	public class TooltipBackground : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private UISprite m_border;

		private Vector3 m_backgroundStartScale;

		private Vector3 m_borderStartScale;

		private void Awake()
		{
			m_backgroundStartScale = m_background.transform.localScale;
			m_borderStartScale = m_border.transform.localScale;
			m_background.alpha = ConfigManager.Instance.Options.TooltipOpacity;
		}

		public void Scale(Single p_x, Single p_y)
		{
			Vector3 backgroundStartScale = m_backgroundStartScale;
			Vector3 borderStartScale = m_borderStartScale;
			backgroundStartScale.x = p_x;
			borderStartScale.x = p_x;
			backgroundStartScale.y = p_y;
			borderStartScale.y = p_y;
			m_background.transform.localScale = backgroundStartScale;
			m_border.transform.localScale = borderStartScale;
		}

		public Vector3 GetScale()
		{
			return m_background.transform.localScale;
		}
	}
}
