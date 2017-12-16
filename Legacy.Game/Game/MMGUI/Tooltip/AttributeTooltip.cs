using System;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/AttributeTooltip")]
	public class AttributeTooltip : MonoBehaviour
	{
		[SerializeField]
		private TooltipBackground m_background;

		[SerializeField]
		private TooltipGroup m_currentHeader;

		[SerializeField]
		private TooltipGroup m_currentText;

		[SerializeField]
		private TooltipGroup m_changeHeader;

		[SerializeField]
		private TooltipGroup m_changeText;

		[SerializeField]
		private Single m_outerPadding = 5f;

		[SerializeField]
		private Single m_innerPadding;

		[SerializeField]
		private Color m_attributetColor = Color.green;

		[SerializeField]
		private Color m_nextLevelAttributeColor = Color.green;

		private EPotionTarget m_atrribute;

		private Int32 m_currentIncrease;

		private String m_attributetColorHex;

		private String m_nextLevelAttributeColorHex;

		private Boolean m_initialized;

		public Vector3 Scale => m_background.GetScale();

	    private void Init()
		{
			if (!m_initialized)
			{
				m_attributetColorHex = "[" + NGUITools.EncodeColor(m_attributetColor) + "]";
				m_nextLevelAttributeColorHex = "[" + NGUITools.EncodeColor(m_nextLevelAttributeColor) + "]";
				m_initialized = true;
			}
		}

		public void Fill(TooltipType p_type, EPotionTarget p_attribute, Int32 p_currentIncrease, Character p_character, DummyCharacter p_dummy)
		{
			Init();
			m_atrribute = p_attribute;
			Single num = m_outerPadding + m_innerPadding;
			m_currentIncrease = p_currentIncrease;
			m_currentHeader.UpdatePositionY(-num);
			m_currentHeader.UpdateText(LocaManager.GetText("ATTRIBUTE_CURRENT"));
			num += m_currentHeader.Size.y + m_innerPadding;
			m_currentText.UpdatePositionY(-num);
			FillDescription(m_currentText, m_currentIncrease, p_character, p_dummy, false);
			num += m_currentText.Size.y + m_innerPadding;
			m_changeHeader.UpdatePositionY(-num);
			if (p_type == TooltipType.CURRENT_EFFECT_NEXT)
			{
				m_changeHeader.UpdateText(LocaManager.GetText("ATTRIBUTE_NEXT"));
			}
			else
			{
				m_changeHeader.UpdateText(LocaManager.GetText("ATTRIBUTE_PREV"));
			}
			num += m_changeHeader.Size.y + m_innerPadding;
			m_changeText.UpdatePositionY(-num);
			if (p_type == TooltipType.CURRENT_EFFECT_NEXT)
			{
				FillDescription(m_changeText, m_currentIncrease + 1, p_character, p_dummy, true);
			}
			else
			{
				FillDescription(m_changeText, m_currentIncrease - 1, p_character, p_dummy, true);
			}
			num += m_changeText.Size.y + m_innerPadding;
			m_background.Scale(m_currentHeader.Size.x + m_outerPadding * 2f, num - m_innerPadding + m_outerPadding);
		}

		private void FillDescription(TooltipGroup p_target, Int32 p_currentIncrease, Character p_character, DummyCharacter p_dummy, Boolean p_nextOrPrevious)
		{
			GameConfig game = ConfigManager.Instance.Game;
			String arg;
			if (p_nextOrPrevious)
			{
				arg = m_nextLevelAttributeColorHex;
			}
			else
			{
				arg = m_attributetColorHex;
			}
			Single hpperVitality;
			if (p_character != null)
			{
				hpperVitality = p_character.Class.GetHPPerVitality();
			}
			else
			{
				hpperVitality = p_dummy.GetHPPerVitality();
			}
			if (m_atrribute == EPotionTarget.MIGHT)
			{
				String str = String.Empty;
				if (game.HealthPerMight > 0f)
				{
					str = LocaManager.GetText("ATRRIBUTE_EFFECT_MIGHT_HEALTH", game.HealthPerMight * p_currentIncrease, arg, "[-]") + "\n\n";
				}
				String text = LocaManager.GetText("ATRRIBUTE_EFFECT_MIGHT_DAMAGE", Mathf.RoundToInt(ConfigManager.Instance.Game.MainHandDamage * p_currentIncrease * 100f), arg, "[-]");
				p_target.UpdateText(str + text);
			}
			else if (m_atrribute == EPotionTarget.MAGIC)
			{
				String str2 = String.Empty;
				if (game.ManaPerMagic > 0f)
				{
					str2 = LocaManager.GetText("ATRRIBUTE_EFFECT_MAGIC_MANA", game.ManaPerMagic * p_currentIncrease, arg, "[-]") + "\n\n";
				}
				String text2 = LocaManager.GetText("ATRRIBUTE_EFFECT_MAGIC_POWER", Mathf.RoundToInt(ConfigManager.Instance.Game.MagicDamage * p_currentIncrease * 100f), arg, "[-]");
				p_target.UpdateText(str2 + text2);
			}
			else if (m_atrribute == EPotionTarget.PERCEPTION)
			{
				String text3 = LocaManager.GetText("ATRRIBUTE_EFFECT_PERCEPTION_DAMAGE", Mathf.RoundToInt(ConfigManager.Instance.Game.RangedDamage * p_currentIncrease * 100f), arg, "[-]");
				String text4 = LocaManager.GetText("ATRRIBUTE_EFFECT_PERCEPTION_ATTACK", p_currentIncrease, arg, "[-]");
				p_target.UpdateText(text3 + "\n\n" + text4);
			}
			else if (m_atrribute == EPotionTarget.DESTINY)
			{
				String text5 = LocaManager.GetText("ATRRIBUTE_EFFECT_DESTINY_CRIT", game.MainHandCritChanceDestinyMod * p_currentIncrease, arg, "[-]");
				String text6 = LocaManager.GetText("ATRRIBUTE_EFFECT_DESTINY_EVADE", p_currentIncrease, arg, "[-]");
				p_target.UpdateText(text5 + "\n\n" + text6);
			}
			else if (m_atrribute == EPotionTarget.VITALITY)
			{
				String text7 = LocaManager.GetText("ATRRIBUTE_EFFECT_VITALITY_HEALTH", hpperVitality * p_currentIncrease, arg, "[-]");
				p_target.UpdateText(text7);
			}
			else if (m_atrribute == EPotionTarget.SPIRIT)
			{
				String text8 = LocaManager.GetText("ATRRIBUTE_EFFECT_SPIRIT_MANA", game.ManaPerSpirit * p_currentIncrease, arg, "[-]");
				p_target.UpdateText(text8);
			}
		}

		public void Show()
		{
			NGUITools.SetActiveSelf(gameObject, true);
		}

		public void Hide()
		{
			NGUITools.SetActiveSelf(gameObject, false);
		}

		public enum TooltipType
		{
			CURRENT_EFFECT_NEXT,
			CURRENT_EFFECT_PREV
		}
	}
}
