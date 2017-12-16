using System;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/CharacterHudStatIcon")]
	public class CharacterHudStatIcon : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private Color m_defaultColor = new Color(1f, 0f, 0f, 1f);

		private String m_tooltipText;

		private Boolean m_isEnabled;

		public Boolean IsEnabled => m_isEnabled;

	    public void Init()
		{
			m_tooltipText = String.Empty;
			m_icon.color = m_defaultColor;
			SetEnabled(false);
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void SetTooltip(String p_tt)
		{
			m_tooltipText = p_tt;
		}

		public void AddToTooltip(String p_extension)
		{
			m_tooltipText = m_tooltipText + "\n" + p_extension;
		}

		public void SetColor(Color p_color)
		{
			m_icon.color = p_color;
		}

		public void SetEnabled(Boolean p_enabled)
		{
			NGUITools.SetActive(m_icon.gameObject, p_enabled);
			m_isEnabled = p_enabled;
			if (!p_enabled)
			{
				m_tooltipText = String.Empty;
			}
		}

		public void OnTooltip(Boolean isOver)
		{
			if (isOver && m_tooltipText != String.Empty && m_isEnabled)
			{
				Vector3 position = m_icon.transform.position;
				Vector3 p_offset = m_icon.transform.localScale * 0.5f;
				TooltipManager.Instance.Show(this, m_tooltipText, position, p_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
