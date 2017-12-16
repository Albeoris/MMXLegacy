using System;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/XPBar")]
	public class XPBar : MonoBehaviour
	{
		private Character m_character;

		[SerializeField]
		private BoxCollider m_collider;

		public void SetNextXPAndLevel(Character p_character)
		{
			m_character = p_character;
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnTooltip(Boolean show)
		{
			if (show && !m_character.MaxLevelReached)
			{
				String text = LocaManager.GetText("CHARACTER_XP_TT", m_character.ExpNeededForNextLevel, m_character.Level + 1);
				TooltipManager.Instance.Show(this, text, transform.position, m_collider.transform.localScale * 0.5f);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
