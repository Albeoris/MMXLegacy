using System;
using System.Threading;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/TradingCompareCharacter")]
	public class TradingCompareCharacter : MonoBehaviour
	{
		private const String BACKGROUND_SELECTED_CHARACTER = "BTN_square_152_highlight";

		private const String BACKGROUND_NOT_SELECTED_CHARACTER = "BTN_square_152";

		[SerializeField]
		private UISprite m_portrait;

		[SerializeField]
		private UISprite m_portraitBG;

		[SerializeField]
		private UISprite m_body;

		[SerializeField]
		private Color m_hoverColor;

		[SerializeField]
		private Color m_selectionColor;

		[SerializeField]
		private Color m_selectionHoverColor;

		private Character m_char;

		private Color m_portraitBackgroundNormalColor;

		private Boolean m_selected = true;

		private Boolean m_isHovered;

		public event EventHandler OnPortraitClicked;

		public Character Character => m_char;

	    public Boolean Selected => m_selected;

	    public void Init(Character p_char)
		{
			m_char = p_char;
			m_portraitBackgroundNormalColor = m_portraitBG.color;
			m_portrait.spriteName = p_char.Portrait;
			m_body.spriteName = p_char.Body;
		}

		public void SetSelected(Character p_chara)
		{
			if (p_chara == m_char)
			{
				SetSelected(true);
			}
			else if (m_selected)
			{
				SetSelected(false);
			}
		}

		public void SetSelected(Boolean p_selected)
		{
			m_selected = p_selected;
			if (m_selected)
			{
				m_portraitBG.spriteName = "BTN_square_152_highlight";
				TweenColor.Begin(m_portraitBG.gameObject, 0.1f, (!m_isHovered) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				m_portraitBG.spriteName = "BTN_square_152";
				TweenColor.Begin(m_portraitBG.gameObject, 0.1f, (!m_isHovered) ? m_portraitBackgroundNormalColor : m_hoverColor);
			}
		}

		public void OnClick()
		{
			if (OnPortraitClicked != null)
			{
				OnPortraitClicked(this, EventArgs.Empty);
			}
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (enabled)
			{
				if (m_selected)
				{
					TweenColor.Begin(m_portraitBG.gameObject, 0.1f, (!p_isOver) ? m_selectionColor : m_selectionHoverColor);
				}
				else
				{
					TweenColor.Begin(m_portraitBG.gameObject, 0.1f, (!p_isOver) ? m_portraitBackgroundNormalColor : m_hoverColor);
				}
			}
		}
	}
}
