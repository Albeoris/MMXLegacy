using System;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/BestiaryProgressBar")]
	public class BestiaryProgressBar : MonoBehaviour
	{
		[SerializeField]
		private BoxCollider m_collider;

		[SerializeField]
		private UIFilledSprite m_fillSprite;

		private Int32 m_currentAmount;

		private Int32 m_maxAmount;

		public UIFilledSprite FillSprite => m_fillSprite;

	    private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void SetCurrentAmount(Int32 p_amount, Int32 p_maxAmount)
		{
			m_currentAmount = p_amount;
			m_maxAmount = p_maxAmount;
			Single num = 1f / (p_maxAmount / (Single)p_amount);
			m_fillSprite.fillAmount = ((num <= 1f) ? num : 1f);
		}

		public void Hide()
		{
			NGUITools.SetActiveSelf(gameObject, false);
		}

		public void Show()
		{
			NGUITools.SetActiveSelf(gameObject, true);
		}

		public void OnTooltip(Boolean show)
		{
			if (show)
			{
				String p_tooltipText = String.Empty;
				if (m_currentAmount < m_maxAmount)
				{
					p_tooltipText = LocaManager.GetText("BESTIARY_PROGRESS_TT", m_currentAmount, m_maxAmount - m_currentAmount);
				}
				else
				{
					p_tooltipText = LocaManager.GetText("BESTIARY_PROGRESS_FULL_TT", m_maxAmount);
				}
				TooltipManager.Instance.Show(this, p_tooltipText, gameObject.transform.position, m_collider.transform.localScale * 0.5f);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
