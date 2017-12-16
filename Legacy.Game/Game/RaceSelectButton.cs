using System;
using System.Threading;
using Legacy.Core;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/RaceSelectButton")]
	public class RaceSelectButton : MonoBehaviour
	{
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

		private ERace m_race;

		private Color m_pBackgroundNormalColor;

		public event EventHandler OnRaceSelected;

		private void Awake()
		{
			m_pBackgroundNormalColor = m_BG.color;
		}

		public void Init(ERace p_race)
		{
			m_race = p_race;
		}

		public ERace Race => m_race;

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
			if (p_isOver)
			{
			}
		}

		private void OnClick()
		{
			if (OnRaceSelected != null)
			{
				OnRaceSelected(this, EventArgs.Empty);
			}
		}

		public void SetSelected(ERace p_race)
		{
			m_selected = (m_race == p_race);
			if (m_selected)
			{
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!m_isHovered) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!m_isHovered) ? m_pBackgroundNormalColor : m_hoverColor);
			}
		}
	}
}
