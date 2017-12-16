using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SelectionListItem")]
	public class SelectionListItem : MonoBehaviour
	{
		private Boolean m_selected;

		[SerializeField]
		private Color m_normalColor;

		[SerializeField]
		private Color m_hoverColor;

		[SerializeField]
		private Color m_selectedColor;

		[SerializeField]
		private UILabel m_label;

		public Boolean Selected
		{
			get => m_selected;
		    set
			{
				m_selected = value;
				UpdateSelection();
			}
		}

		public String Value => m_label.text;

	    private void Awake()
		{
			m_label.color = m_normalColor;
		}

		public void OnHover(Boolean p_isOver)
		{
			if (p_isOver)
			{
				if (!m_selected)
				{
					m_label.color = m_hoverColor;
				}
			}
			else
			{
				OnLeave();
			}
		}

		public void OnLeave()
		{
			m_label.color = ((!m_selected) ? m_normalColor : m_selectedColor);
		}

		private void UpdateSelection()
		{
			m_label.color = ((!m_selected) ? m_normalColor : m_selectedColor);
		}
	}
}
