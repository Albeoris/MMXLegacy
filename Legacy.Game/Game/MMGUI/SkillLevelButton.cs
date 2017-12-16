using System;
using Legacy.Core.Entities.Skills;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.MMGUI;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SkillLevelButton")]
	public class SkillLevelButton : MonoBehaviour
	{
		[SerializeField]
		private Vector3 m_offset;

		[SerializeField]
		private Single m_tooltipDelay;

		[SerializeField]
		private GUIMultiSpriteButton m_button;

		private Skill m_skill;

		private Boolean m_isHovered;

		private Boolean m_tooltipVisible;

		private Single m_tooltipTime;

		private ButtonType m_buttonType;

		public void Init(Skill p_skill, ButtonType p_buttonType)
		{
			m_skill = p_skill;
			m_buttonType = p_buttonType;
		}

		public void SetEnabled(Boolean p_enabled)
		{
			m_button.IsEnabled = p_enabled;
		}

		public void UpdateTooltip()
		{
			if (m_tooltipVisible && m_button.IsEnabled)
			{
				ShowTooltip(true);
			}
		}

		private void OnHover(Boolean p_isHovered)
		{
			if (p_isHovered)
			{
				m_tooltipTime = Time.realtimeSinceStartup + m_tooltipDelay;
			}
			else if (m_tooltipVisible)
			{
				ShowTooltip(false);
			}
			m_isHovered = p_isHovered;
		}

		private void Update()
		{
			if (m_isHovered && !m_tooltipVisible && m_tooltipTime < Time.realtimeSinceStartup)
			{
				ShowTooltip(true);
			}
		}

		private void ShowTooltip(Boolean p_show)
		{
			if (p_show)
			{
				Vector3 position = gameObject.transform.position;
				SkillTooltip.TooltipType p_type = (m_buttonType != ButtonType.PLUS) ? SkillTooltip.TooltipType.CURRENT_EFFECT_PREV : SkillTooltip.TooltipType.CURRENT_EFFECT_NEXT;
				TooltipManager.Instance.Show(this, m_skill, p_type, false, position, m_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
			m_tooltipVisible = p_show;
		}

		private void OnDisable()
		{
			OnHover(false);
			TooltipManager.Instance.Hide(this);
		}

		public enum ButtonType
		{
			PLUS,
			MINUS
		}
	}
}
