using System;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/CharacterStatEntry")]
	public class CharacterStatEntry : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_leftColumn;

		[SerializeField]
		private UILabel m_rightColumn;

		private String m_tt;

		public virtual void Init(String p_caption)
		{
			m_tt = String.Empty;
			if (m_leftColumn != null)
			{
				m_leftColumn.text = p_caption;
			}
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void UpdateLabel(String p_text, String p_tt)
		{
			m_rightColumn.text = p_text;
			m_tt = p_tt;
		}

		public void OnTooltip(Boolean isOver)
		{
			if (isOver && m_tt != String.Empty)
			{
				Vector3 position = m_rightColumn.transform.position;
				Vector3 p_offset = m_rightColumn.transform.localScale * 0.5f;
				TooltipManager.Instance.Show(this, m_tt, position, p_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
