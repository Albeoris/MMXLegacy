using System;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/MonsterStatEntry")]
	public class MonsterStatEntry : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_icon;

		private String m_tt;

		public void Init()
		{
			m_tt = String.Empty;
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void ShowEntry(Boolean p_show)
		{
			NGUITools.SetActive(gameObject, p_show);
		}

		public void UpdateEntry(String p_iconName)
		{
			m_icon.spriteName = p_iconName;
		}

		public void SetTooltip(String p_tt)
		{
			m_tt = p_tt;
		}

		public void OnTooltip(Boolean isOver)
		{
			if (isOver && m_tt != String.Empty)
			{
				Vector3 position = m_icon.transform.position;
				Vector3 p_offset = m_icon.transform.localScale * 0.5f;
				TooltipManager.Instance.Show(this, m_tt, position, p_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
