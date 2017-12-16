using System;
using Legacy.Core.Entities.Skills;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SkillTierView")]
	public class SkillTierView : MonoBehaviour
	{
		[SerializeField]
		private String m_tierReachedSprite;

		[SerializeField]
		private String m_tierNotReachedSprite;

		[SerializeField]
		private UIFilledSprite m_barFill;

		[SerializeField]
		private UIFilledSprite m_barFillTemp;

		[SerializeField]
		private UIFilledSprite m_barFillAvailable;

		[SerializeField]
		private UISprite m_barDot;

		[SerializeField]
		private UISprite m_barDotAvailable;

		[SerializeField]
		private Vector3 m_tooltipOffset;

		private Skill m_skill;

		private SkillTooltip.TooltipType m_type;

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void SetVisible(Boolean p_visible)
		{
			NGUITools.SetActiveSelf(gameObject, p_visible);
		}

		public void SetTierReached(Boolean p_available, Boolean p_reached)
		{
			m_barDot.spriteName = ((!p_reached) ? m_tierNotReachedSprite : m_tierReachedSprite);
			m_barDot.color = ((!p_reached) ? Color.gray : Color.white);
			NGUITools.SetActiveSelf(m_barDotAvailable.gameObject, p_available && !p_reached);
		}

		public void SetFillAmount(Single p_amount, Single p_tempAmount)
		{
			m_barFillTemp.fillAmount = p_amount;
			m_barFill.fillAmount = p_amount - p_tempAmount;
		}

		public void SetAvailable(Boolean p_available)
		{
			NGUITools.SetActiveSelf(m_barFillAvailable.gameObject, p_available);
		}

		public void SetDataForTooltip(Skill p_skill, SkillTooltip.TooltipType p_type)
		{
			m_skill = p_skill;
			m_type = p_type;
		}

		private void OnTooltip(Boolean show)
		{
			if (show)
			{
				TooltipManager.Instance.Show(this, m_skill, m_type, false, gameObject.transform.position, m_tooltipOffset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
