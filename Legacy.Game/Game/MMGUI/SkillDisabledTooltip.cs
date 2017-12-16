using System;
using Legacy.Core.Entities.Skills;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SkillDisabledTooltip")]
	public class SkillDisabledTooltip : MonoBehaviour
	{
		[SerializeField]
		private Vector3 m_offset;

		[SerializeField]
		private Single m_tooltipDelay;

		private Skill m_skill;

		private Boolean m_isHovered;

		private Boolean m_tooltipVisible;

		private Single m_tooltipTime;

		public void Init(Skill p_skill)
		{
			m_skill = p_skill;
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
				String text = LocaManager.GetText(m_skill.Name);
				String id = String.Empty;
				if (m_skill.Tier == ETier.NOVICE)
				{
					id = "SPELLBOOK_SKILL_NEED_EXPERT_TIER";
				}
				else if (m_skill.Tier == ETier.EXPERT)
				{
					id = "SPELLBOOK_SKILL_NEED_MASTER_TIER";
				}
				else if (m_skill.Tier == ETier.MASTER)
				{
					id = "SPELLBOOK_SKILL_NEED_GRAND_MASTER_TIER";
				}
				TooltipManager.Instance.Show(this, LocaManager.GetText(id, text), position, m_offset);
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
	}
}
