using System;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	internal class MapTooltip : MonoBehaviour
	{
		[SerializeField]
		private TooltipBackground m_background;

		[SerializeField]
		private TooltipGroup m_objectNote;

		[SerializeField]
		private TooltipGroup m_userNote;

		[SerializeField]
		private Single m_outerPadding = 5f;

		[SerializeField]
		private Single m_innerPadding;

		public Vector3 Scale => m_background.GetScale();

	    public void Fill(String objectNote, String userNote)
		{
			Single num = m_outerPadding + m_innerPadding;
			Single num2 = 0f;
			Boolean flag = !String.IsNullOrEmpty(objectNote);
			m_objectNote.SetVisible(flag);
			if (flag)
			{
				m_objectNote.UpdatePositionY(-num);
				m_objectNote.UpdateText(objectNote);
				num += m_objectNote.Size.y + m_innerPadding;
				num2 = m_objectNote.Size.x;
			}
			Boolean flag2 = !String.IsNullOrEmpty(userNote);
			m_userNote.SetVisible(flag2);
			if (flag2)
			{
				m_userNote.UpdatePositionY(-num);
				m_userNote.UpdateText(userNote);
				num += m_userNote.Size.y + m_innerPadding;
				num2 = m_userNote.Size.x;
			}
			m_background.Scale(num2 + m_outerPadding * 2f, num - m_innerPadding + m_outerPadding);
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}
