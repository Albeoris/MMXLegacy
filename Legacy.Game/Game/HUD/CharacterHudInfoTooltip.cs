using System;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/CharacterHudInfoTooltip")]
	public class CharacterHudInfoTooltip : MonoBehaviour
	{
		private ETooltipObject m_type;

		private Character m_character;

		public void Init(ETooltipObject p_type, Character p_character)
		{
			m_type = p_type;
			m_character = p_character;
		}

		public void ChangeCharacter(Character p_character)
		{
			m_character = p_character;
		}

		public void OnTooltip(Boolean p_isOver)
		{
			if (p_isOver)
			{
				String text;
				String text2;
				if (m_type == ETooltipObject.HP_BAR)
				{
					text = LocaManager.GetText("CHARACTER_PORTRAIT_TOOLTIP_HEALTH");
					text2 = LocaManager.GetText("GUI_STATS_X_OF_Y", m_character.HealthPoints, m_character.MaximumHealthPoints);
				}
				else
				{
					text = LocaManager.GetText("CHARACTER_PORTRAIT_TOOLTIP_MANA");
					text2 = LocaManager.GetText("GUI_STATS_X_OF_Y", m_character.ManaPoints, m_character.MaximumManaPoints);
				}
				Vector3 position = transform.position;
				Vector3 p_offset = transform.localScale * 0.5f;
				TooltipManager.Instance.Show(this, text, text2, TextTooltip.ESize.SMALL, position, p_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		public enum ETooltipObject
		{
			HP_BAR,
			MP_BAR
		}
	}
}
