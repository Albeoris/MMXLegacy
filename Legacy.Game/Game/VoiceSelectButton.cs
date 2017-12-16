using System;
using System.Threading;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/VoiceSelectButton")]
	public class VoiceSelectButton : MonoBehaviour
	{
		private const String BACKGROUND_SELECTED_VOICE = "BTN_rect_192_48_highlight";

		private const String BACKGROUND_NOT_SELECTED_VOICE = "BTN_rect_192_48";

		[SerializeField]
		private UISprite m_BG;

		[SerializeField]
		private Color m_hoverColor;

		[SerializeField]
		private Color m_selectionColor;

		[SerializeField]
		private Color m_selectionHoverColor;

		private Boolean m_selected = true;

		private Boolean m_isHovered;

		private EVoice m_voice;

		private Color m_pBackgroundNormalColor;

		public event EventHandler OnRaceSelected;

		public EVoice Voice => m_voice;

	    public void Init(EVoice p_voice)
		{
			m_pBackgroundNormalColor = m_BG.color;
			m_voice = p_voice;
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (m_selected)
			{
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!p_isOver) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!p_isOver) ? m_pBackgroundNormalColor : m_hoverColor);
			}
		}

		private void OnClick()
		{
			if (OnRaceSelected != null)
			{
				OnRaceSelected(this, EventArgs.Empty);
			}
		}

		public void SetSelected(EVoice p_voice)
		{
			m_selected = (m_voice == p_voice);
			if (m_selected)
			{
				m_BG.spriteName = "BTN_rect_192_48_highlight";
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!m_isHovered) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				m_BG.spriteName = "BTN_rect_192_48";
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!m_isHovered) ? m_pBackgroundNormalColor : m_hoverColor);
			}
		}
	}
}
