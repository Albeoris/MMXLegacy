using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/SpellTooltip")]
	public class SpellTooltip : MonoBehaviour
	{
		private const String BEST_HEALTH_POTION_DEFAULT = "ITM_consumable_potion_health_1";

		private const String BEST_MANA_POTION_DEFAULT = "ITM_consumable_potion_mana_1";

		[SerializeField]
		private TooltipBackground m_background;

		[SerializeField]
		private TooltipGroup m_name;

		[SerializeField]
		private TooltipGroup m_description;

		[SerializeField]
		private TooltipGroup m_details;

		[SerializeField]
		private TooltipGroup m_requirements;

		[SerializeField]
		private TooltipItemSlot m_itemSlot;

		[SerializeField]
		private Single m_outerPadding = 5f;

		[SerializeField]
		private Single m_innerPadding = 3f;

		protected String[] m_descriptionValues;

		public Vector3 Scale => m_background.GetScale();

	    public void Hide()
		{
			NGUITools.SetActiveSelf(gameObject, false);
		}

		public void Fill(EQuickActionType p_action)
		{
			if (m_descriptionValues == null)
			{
				m_descriptionValues = new String[9];
				m_descriptionValues[0] = "[00FF00]";
				m_descriptionValues[1] = "[80FF80]";
				m_descriptionValues[2] = "[FF0000]";
				m_descriptionValues[3] = "[FFC080]";
				m_descriptionValues[4] = "[FFFF80]";
				m_descriptionValues[5] = "[80FFFF]";
				m_descriptionValues[6] = "[-]";
			}
			m_name.UpdateText(LocaManager.GetText("STANDARD_ACTION_" + p_action));
			Single num = m_outerPadding + m_name.Size.y + m_innerPadding;
			switch (p_action)
			{
			case EQuickActionType.ATTACK:
				m_itemSlot.SetSpell("SPL_action_melee");
				m_itemSlot.HideItem();
				break;
			case EQuickActionType.ATTACKRANGED:
				m_itemSlot.SetSpell("SPL_action_ranged");
				m_itemSlot.HideItem();
				break;
			case EQuickActionType.USE_BEST_MANAPOTION:
			{
				m_itemSlot.SetSpell("SPL_action_bestpotion");
				Potion bestPotion = LegacyLogic.Instance.WorldManager.Party.GetBestPotion(EPotionType.MANA_POTION, null);
				if (bestPotion != null)
				{
					m_itemSlot.SetItem(bestPotion.Icon);
				}
				else
				{
					m_itemSlot.SetItem("ITM_consumable_potion_mana_1");
				}
				break;
			}
			case EQuickActionType.USE_BEST_HEALTHPOTION:
			{
				m_itemSlot.SetSpell("SPL_action_bestpotion");
				Potion bestPotion2 = LegacyLogic.Instance.WorldManager.Party.GetBestPotion(EPotionType.HEALTH_POTION, null);
				if (bestPotion2 != null)
				{
					m_itemSlot.SetItem(bestPotion2.Icon);
				}
				else
				{
					m_itemSlot.SetItem("ITM_consumable_potion_health_1");
				}
				break;
			}
			case EQuickActionType.DEFEND:
				m_itemSlot.SetSpell("SPL_action_defend");
				m_itemSlot.HideItem();
				break;
			}
			m_itemSlot.UpdatePosition(new Vector3(-m_name.Size.x / 2f, -num, 0f));
			m_description.MinHeight = m_itemSlot.Size.y;
			m_description.VerticalAlign = TooltipGroup.Align.CENTER;
			m_description.UpdateText("-");
			m_description.UpdatePositionY(-num);
			num += m_description.Size.y + m_innerPadding;
			if (p_action == EQuickActionType.ATTACKRANGED)
			{
				m_descriptionValues[7] = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.FightValues.RangedAttackRange.ToString();
				m_descriptionValues[8] = (ConfigManager.Instance.Game.RangedAttackMeleeMalus * 100f).ToString();
			}
			else if (p_action == EQuickActionType.DEFEND)
			{
				m_descriptionValues[7] = (ConfigManager.Instance.Game.DefendEvadeBonus * 100f).ToString();
				m_descriptionValues[8] = (ConfigManager.Instance.Game.DefendBlockBonus * 100f).ToString();
			}
			else
			{
				m_descriptionValues[7] = String.Empty;
			}
			m_details.UpdateText(LocaManager.GetText("STANDARD_ACTION_" + p_action + "_TT", m_descriptionValues));
			m_details.UpdatePositionY(-num);
			num += m_details.Size.y + m_innerPadding;
			m_requirements.SetVisible(false);
			m_background.Scale(m_name.Size.x + m_outerPadding * 2f, num - m_innerPadding + m_outerPadding);
		}

		public void Fill(CharacterSpell p_spell)
		{
			m_name.UpdateText(LocaManager.GetText(p_spell.NameKey));
			Single num = m_outerPadding + m_name.Size.y + m_innerPadding;
			m_itemSlot.SetSpell(p_spell.StaticData.Icon);
			m_itemSlot.HideItem();
			m_itemSlot.UpdatePosition(new Vector3(-m_name.Size.x / 2f, -num, 0f));
			m_description.MinHeight = m_itemSlot.Size.y;
			m_description.VerticalAlign = TooltipGroup.Align.CENTER;
			if (p_spell.StaticData.ManaCost > 0)
			{
				m_description.UpdateText(LocaManager.GetText("SPELLBOOK_SPELL_MANA", p_spell.StaticData.ManaCost));
			}
			else
			{
				m_description.UpdateText(LocaManager.GetText("SPELL_DESCRIPTION_MANA_ALL"));
			}
			m_description.UpdatePositionY(-num);
			num += m_description.Size.y + m_innerPadding;
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			Single magicFactor = p_spell.GetMagicFactor(selectedCharacter, false, 0);
			m_details.UpdateText(p_spell.GetDescription(magicFactor));
			m_details.UpdatePositionY(-num);
			num += m_details.Size.y + m_innerPadding;
			m_requirements.SetVisible(true);
			SkillStaticData staticData = StaticDataHandler.GetStaticData<SkillStaticData>(EDataType.SKILL, (Int32)p_spell.StaticData.SkillID);
			String text = LocaManager.GetText(staticData.Name);
			String text2 = LocaManager.GetText("SKILL_TIER_" + (Int32)p_spell.StaticData.Tier);
			Boolean flag;
			if (p_spell.StaticData.ClassOnly == EClass.NONE)
			{
				flag = IsSkillRequirementFulfilled((Int32)p_spell.StaticData.SkillID, p_spell.StaticData.Tier);
				m_requirements.UpdateText(LocaManager.GetText("SKILL_TIER_REQUIREMENT_TT", text, text2));
			}
			else
			{
				flag = selectedCharacter.Class.IsAdvanced;
				String str = (selectedCharacter.Gender != EGender.MALE) ? "_F" : "_M";
				if (p_spell.StaticData.SkillID == ESkillID.SKILL_WARFARE)
				{
					m_requirements.UpdateText(LocaManager.GetText("ABILITY_REQUIREMENT_ADVANCED_CLASS" + str, selectedCharacter.Name, LocaManager.GetText(selectedCharacter.Class.AdvancedNameKey + str)));
				}
				else
				{
					m_requirements.UpdateText(LocaManager.GetText("SPELL_REQUIREMENT_ADVANCED_CLASS" + str, selectedCharacter.Name, LocaManager.GetText(selectedCharacter.Class.AdvancedNameKey + str)));
				}
			}
			m_requirements.UpdatePositionY(-num);
			m_requirements.Label.color = ((!flag) ? Color.red : Color.white);
			num += m_requirements.Size.y + m_innerPadding;
			m_background.Scale(m_name.Size.x + m_outerPadding * 2f, num - m_innerPadding + m_outerPadding);
		}

		public void Fill(BaseAbilityStaticData p_ability)
		{
			m_name.UpdateText(LocaManager.GetText(p_ability.NameKey));
			Single num = m_outerPadding + m_name.Size.y + m_innerPadding;
			m_itemSlot.SetSpell(p_ability.Icon);
			m_itemSlot.HideItem();
			m_itemSlot.UpdatePosition(new Vector3(-m_name.Size.x / 2f, -num, 0f));
			m_description.MinHeight = m_itemSlot.Size.y;
			m_description.VerticalAlign = TooltipGroup.Align.CENTER;
			m_description.UpdateText(LocaManager.GetText("SPELLBOOK_PASSIVE_ABILITY"));
			m_description.UpdatePositionY(-num);
			num += m_description.Size.y + m_innerPadding;
			String description;
			if (p_ability is RacialAbilitiesStaticData)
			{
				description = (p_ability as RacialAbilitiesStaticData).GetDescription();
			}
			else
			{
				description = (p_ability as ParagonAbilitiesStaticData).GetDescription();
			}
			m_details.UpdateText(description);
			m_details.UpdatePositionY(-num);
			num += m_details.Size.y + m_innerPadding;
			if (p_ability is ParagonAbilitiesStaticData)
			{
				Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
				String str = (selectedCharacter.Gender != EGender.MALE) ? "_F" : "_M";
				m_requirements.SetVisible(true);
				m_requirements.UpdateText(LocaManager.GetText("ABILITY_REQUIREMENT_ADVANCED_CLASS" + str, selectedCharacter.Name, LocaManager.GetText(selectedCharacter.Class.AdvancedNameKey + str)));
				m_requirements.UpdatePositionY(-num);
				m_requirements.Label.color = ((!selectedCharacter.Class.IsAdvanced) ? Color.red : Color.white);
				num += m_requirements.Size.y + m_innerPadding;
			}
			else
			{
				m_requirements.SetVisible(false);
			}
			m_background.Scale(m_name.Size.x + m_outerPadding * 2f, num - m_innerPadding + m_outerPadding);
		}

		public void Show()
		{
			NGUITools.SetActiveSelf(gameObject, true);
		}

		private Boolean IsSkillRequirementFulfilled(Int32 p_skillID, ETier p_tier)
		{
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			return selectedCharacter.SkillHandler.HasRequiredSkillTier(p_skillID, p_tier);
		}
	}
}
