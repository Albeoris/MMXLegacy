using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ButtonHighlight")]
	public class ButtonHighlight : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_highlightTarget;

		[SerializeField]
		private Color m_hovered = Color.white;

		[SerializeField]
		private Color m_selected = Color.white;

		[SerializeField]
		private Color m_hoveredSelected = Color.white;

		[SerializeField]
		private Color m_pressedColor = Color.white;

		[SerializeField]
		private Color m_normalBackgroundColor = Color.white;

		[SerializeField]
		private Single m_colorTweenSpeed = 0.1f;

		[SerializeField]
		private Boolean m_isSelectable;

		private Boolean m_isSelected;

		private Boolean m_isHovered;

		public void OnEnter(GameObject p_sender)
		{
			OnHover(true);
		}

		public void OnLeave(GameObject p_sender)
		{
			OnHover(false);
		}

		public void OnClicked(GameObject p_sender)
		{
			if (m_isSelectable)
			{
				SetSelected(true);
			}
		}

		public void OnPressed(GameObject p_sender)
		{
			TweenColor.Begin(m_highlightTarget.gameObject, m_colorTweenSpeed, m_pressedColor);
		}

		public void OnReleased(GameObject p_sender)
		{
			TweenColor.Begin(m_highlightTarget.gameObject, m_colorTweenSpeed, m_normalBackgroundColor);
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (enabled)
			{
				if (m_isSelected)
				{
					TweenColor.Begin(m_highlightTarget.gameObject, m_colorTweenSpeed, (!p_isOver) ? m_selected : m_hoveredSelected);
				}
				else
				{
					TweenColor.Begin(m_highlightTarget.gameObject, m_colorTweenSpeed, (!p_isOver) ? m_normalBackgroundColor : m_hovered);
				}
			}
		}

		private void OnDisable()
		{
			TweenColor.Begin(m_highlightTarget.gameObject, m_colorTweenSpeed, m_normalBackgroundColor);
		}

		private void OnEnable()
		{
			TweenColor.Begin(m_highlightTarget.gameObject, m_colorTweenSpeed, m_normalBackgroundColor);
		}

		public void SetSelected(Boolean p_selected)
		{
			m_isSelected = p_selected;
			if (m_isSelected)
			{
				TweenColor.Begin(m_highlightTarget.gameObject, m_colorTweenSpeed, (!m_isHovered) ? m_selected : m_hoveredSelected);
			}
			else
			{
				TweenColor.Begin(m_highlightTarget.gameObject, m_colorTweenSpeed, (!m_isHovered) ? m_normalBackgroundColor : m_hovered);
			}
		}
	}
}
