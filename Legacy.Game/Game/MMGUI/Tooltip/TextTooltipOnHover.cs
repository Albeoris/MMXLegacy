using System;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/TextTooltipOnHover")]
	public class TextTooltipOnHover : MonoBehaviour
	{
		[SerializeField]
		protected String m_caption = String.Empty;

		[SerializeField]
		protected String m_locaKey = String.Empty;

		[SerializeField]
		protected TextTooltip.ESize m_size = TextTooltip.ESize.BIG;

		[SerializeField]
		protected Vector3 m_offset;

		protected virtual void OnDisable()
		{
			if (TooltipManager.Instance != null)
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		protected virtual void OnTooltip(Boolean p_isHovered)
		{
			if (p_isHovered)
			{
				String p_captionText = String.Empty;
				String p_tooltipText = String.Empty;
				if (m_caption != String.Empty)
				{
					p_captionText = LocaManager.GetText(m_caption);
				}
				if (m_locaKey != String.Empty)
				{
					p_tooltipText = LocaManager.GetText(m_locaKey);
				}
				TooltipManager.Instance.Show(this, p_captionText, p_tooltipText, m_size, transform.position, m_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
