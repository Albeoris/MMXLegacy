using System;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	public class TextTooltip : MonoBehaviour
	{
		[SerializeField]
		private TooltipBackground m_background;

		[SerializeField]
		private TooltipGroup m_description;

		[SerializeField]
		private TooltipGroup m_caption;

		[SerializeField]
		private Single m_outerPadding = 5f;

		[SerializeField]
		private Single m_innerPadding;

		public Vector3 Scale => m_background.GetScale();

	    public void Fill(String p_caption, String p_description, ESize p_size)
		{
			Single num = m_outerPadding + m_innerPadding;
			Single num2 = 0f;
			Single horizontalScaleFactor;
			if (p_size == ESize.SMALL)
			{
				horizontalScaleFactor = 0.5f;
			}
			else if (p_size == ESize.MEDIUM)
			{
				horizontalScaleFactor = 0.75f;
			}
			else
			{
				horizontalScaleFactor = 1f;
			}
			Boolean flag = p_caption != String.Empty;
			m_caption.SetVisible(flag);
			if (flag)
			{
				m_caption.UpdatePositionY(-num);
				m_caption.HorizontalScaleFactor = horizontalScaleFactor;
				m_caption.UpdateText(p_caption);
				num += m_caption.Size.y + m_innerPadding;
				num2 = m_caption.Size.x;
			}
			Boolean flag2 = p_description != String.Empty;
			m_description.SetVisible(flag2);
			if (flag2)
			{
				m_description.UpdatePositionY(-num);
				m_description.HorizontalScaleFactor = horizontalScaleFactor;
				m_description.UpdateText(p_description);
				num += m_description.Size.y + m_innerPadding;
				num2 = m_description.Size.x;
			}
			m_background.Scale(num2 + m_outerPadding * 2f, num - m_innerPadding + m_outerPadding);
		}

		public void Show()
		{
			NGUITools.SetActiveSelf(gameObject, true);
		}

		public void Hide()
		{
			NGUITools.SetActiveSelf(gameObject, false);
		}

		public enum ESize
		{
			SMALL,
			MEDIUM,
			BIG
		}
	}
}
