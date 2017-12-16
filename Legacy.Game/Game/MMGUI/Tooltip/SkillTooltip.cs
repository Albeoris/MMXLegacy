using System;
using System.Collections.Generic;
using Legacy.Core.Entities.Skills;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/SkillTooltip")]
	public class SkillTooltip : MonoBehaviour
	{
		[SerializeField]
		private TooltipBackground m_background;

		[SerializeField]
		private TooltipItemSlot m_itemSlot;

		[SerializeField]
		private TooltipItemSlot[] m_spellSlot;

		[SerializeField]
		private TooltipGroup m_name;

		[SerializeField]
		private TooltipGroup m_subHeadLine;

		[SerializeField]
		private TooltipGroup m_level;

		[SerializeField]
		private TooltipGroup m_description;

		[SerializeField]
		private TooltipGroup m_effect;

		[SerializeField]
		private TooltipGroup[] m_spellName;

		[SerializeField]
		private Single m_outerPadding = 5f;

		[SerializeField]
		private Single m_innerPadding;

		[SerializeField]
		private Color m_normalColor = Color.white;

		[SerializeField]
		private Color m_unlearnedColor = Color.grey;

		[SerializeField]
		private Color m_effectColor = Color.green;

		[SerializeField]
		private Color m_unlearnedEffectColor = Color.green;

		private String m_effectColorHex;

		private String m_unlearnedEffectColorHex;

		private Boolean m_initialized;

		public Vector3 Scale => m_background.GetScale();

	    private void Init()
		{
			if (!m_initialized)
			{
				m_effectColorHex = "[" + NGUITools.EncodeColor(m_effectColor) + "]";
				m_unlearnedEffectColorHex = "[" + NGUITools.EncodeColor(m_unlearnedEffectColor) + "]";
				m_initialized = true;
			}
		}

		public void Fill(Skill p_skill, TooltipType p_type, Boolean p_isDefault)
		{
			Init();
			Single num = m_outerPadding + m_innerPadding;
			Boolean flag = p_type != TooltipType.CURRENT_EFFECT_NEXT || p_skill.Tier >= ETier.NOVICE;
			m_name.SetVisible(flag);
			if (flag)
			{
				m_name.UpdatePositionY(-num);
				FillSkillName(p_skill, p_type);
				num += m_name.Size.y + m_innerPadding;
			}
			Boolean flag2 = p_type == TooltipType.BASE || p_type == TooltipType.PARTY_CREATION;
			m_itemSlot.SetVisible(flag2);
			m_level.SetVisible(flag2);
			if (flag2)
			{
				m_itemSlot.UpdatePosition(new Vector3(-m_name.Size.x / 2f, -num, 0f));
				m_itemSlot.SetItem(p_skill.Icon);
				m_level.VerticalAlign = TooltipGroup.Align.CENTER;
				m_level.MinHeight = m_itemSlot.Size.y;
				m_level.UpdatePositionY(-num);
				FillSkillLevel(p_skill, p_type, p_isDefault);
				num += m_level.Size.y + m_innerPadding;
			}
			Boolean flag3 = p_type == TooltipType.BASE || p_type == TooltipType.PARTY_CREATION || p_type == TooltipType.CURRENT_EFFECT_PREV || (p_type == TooltipType.CURRENT_EFFECT_NEXT && p_skill.Tier >= ETier.NOVICE);
			m_description.SetVisible(flag3);
			if (flag3)
			{
				m_description.UpdatePositionY(-num);
				FillSkillDescription(p_skill, p_type, p_isDefault);
				num += m_description.Size.y + m_innerPadding;
			}
			Boolean flag4 = p_type == TooltipType.CURRENT_EFFECT_NEXT || (p_type == TooltipType.CURRENT_EFFECT_PREV && p_skill.Level > 1);
			m_subHeadLine.SetVisible(flag4);
			if (flag4)
			{
				m_subHeadLine.UpdatePositionY(-num);
				if (p_type == TooltipType.CURRENT_EFFECT_NEXT)
				{
					m_subHeadLine.UpdateText(LocaManager.GetText("SKILL_NEXT_INCREASE"));
				}
				else
				{
					m_subHeadLine.UpdateText(LocaManager.GetText("SKILL_PREV_INCREASE"));
				}
				num += m_subHeadLine.Size.y + m_innerPadding;
			}
			Boolean flag5 = (p_type != TooltipType.CURRENT_EFFECT_PREV || p_skill.Level > 1) && (p_type != TooltipType.PARTY_CREATION || IsMagicSchoolSkill(p_skill));
			m_effect.SetVisible(flag5);
			if (flag5)
			{
				m_effect.UpdatePositionY(-num);
				FillSkillEffect(p_skill, p_type);
				num += m_effect.Size.y + m_innerPadding;
			}
			Boolean flag6 = p_type == TooltipType.PARTY_CREATION && IsMagicSchoolSkill(p_skill);
			Int32 num2 = 0;
			for (Int32 i = 0; i < m_spellSlot.Length; i++)
			{
				m_spellSlot[i].SetVisible(false);
				m_spellName[i].SetVisible(false);
			}
			if (flag6)
			{
				foreach (SkillEffectStaticData skillEffectStaticData in p_skill.Tier1Effects)
				{
					if (skillEffectStaticData.Type == ESkillEffectType.LEARN_SPELL)
					{
						Int32 p_spellType = (Int32)skillEffectStaticData.Value;
						CharacterSpell characterSpell = SpellFactory.CreateCharacterSpell((ECharacterSpell)p_spellType);
						m_spellSlot[num2].SetVisible(true);
						m_spellSlot[num2].UpdatePosition(new Vector3(-m_name.Size.x / 2f, -num, 0f));
						m_spellSlot[num2].SetItem(characterSpell.StaticData.Icon);
						m_spellName[num2].SetVisible(true);
						m_spellName[num2].VerticalAlign = TooltipGroup.Align.CENTER;
						m_spellName[num2].MinHeight = m_spellSlot[num2].Size.y;
						m_spellName[num2].UpdatePositionY(-num);
						m_spellName[num2].UpdateText(LocaManager.GetText(characterSpell.NameKey));
						num += m_spellName[num2].Size.y + m_innerPadding;
						num2++;
					}
				}
			}
			m_background.Scale(m_name.Size.x + m_outerPadding * 2f, num - m_innerPadding + m_outerPadding);
		}

		private void FillSkillName(Skill p_skill, TooltipType p_type)
		{
			if (p_type == TooltipType.CURRENT_EFFECT_NEXT || p_type == TooltipType.CURRENT_EFFECT_PREV)
			{
				m_name.Label.color = m_normalColor;
				m_name.UpdateText(LocaManager.GetText("SKILL_CURRENT_INCREASE"));
			}
			if (p_type == TooltipType.BASE || p_type == TooltipType.PARTY_CREATION)
			{
				m_name.Label.color = m_normalColor;
				m_name.UpdateText(LocaManager.GetText(p_skill.Name));
			}
			else if (p_type == TooltipType.EXPERT)
			{
				m_name.Label.color = ((p_skill.Tier < ETier.EXPERT) ? m_unlearnedColor : m_normalColor);
				m_name.UpdateText(LocaManager.GetText("SKILL_TIER_BONUS_2"));
			}
			else if (p_type == TooltipType.MASTER)
			{
				m_name.Label.color = ((p_skill.Tier < ETier.MASTER) ? m_unlearnedColor : m_normalColor);
				m_name.UpdateText(LocaManager.GetText("SKILL_TIER_BONUS_3"));
			}
			else if (p_type == TooltipType.GRAND_MASTER)
			{
				m_name.Label.color = ((p_skill.Tier < ETier.GRAND_MASTER) ? m_unlearnedColor : m_normalColor);
				m_name.UpdateText(LocaManager.GetText("SKILL_TIER_BONUS_4"));
			}
		}

		private void FillSkillLevel(Skill p_skill, TooltipType p_type, Boolean p_isDefault)
		{
			if (p_type == TooltipType.PARTY_CREATION)
			{
				if (p_skill.MaxTier == ETier.NONE)
				{
					m_level.UpdateText(LocaManager.GetText("TOOLTIP_SKILL_CANNOT_LEARN"));
				}
				else
				{
					String p_text;
					if (p_skill.Tier == ETier.NONE)
					{
						p_text = LocaManager.GetText("TOOLTIP_SKILL_UNSELECTED");
					}
					else
					{
						p_text = "[00ff00]" + LocaManager.GetText("TOOLTIP_SKILL_SELECTED") + "[-]";
					}
					m_level.UpdateText(p_text);
				}
			}
			else if (p_skill.Tier == ETier.NONE)
			{
				m_level.UpdateText(LocaManager.GetText("GUI_SKILLS_LEVEL_DESCRIPTION_UNLEARNED"));
			}
			else
			{
				String arg;
				if (p_skill.VirtualSkillLevel > 0)
				{
					arg = m_effectColorHex + (p_skill.Level + p_skill.VirtualSkillLevel) + "[-]";
				}
				else
				{
					arg = p_skill.Level.ToString();
				}
				m_level.UpdateText(LocaManager.GetText("TT_SKILL_LEVEL", SkillView.GetTierText(p_skill.Tier), arg));
			}
		}

		private void FillSkillDescription(Skill p_skill, TooltipType p_type, Boolean p_isDefault)
		{
			if (p_type == TooltipType.BASE || p_type == TooltipType.PARTY_CREATION)
			{
				String text = LocaManager.GetText(p_skill.Description);
				if (p_type == TooltipType.PARTY_CREATION)
				{
					if (p_skill.MaxTier == ETier.GRAND_MASTER)
					{
						text = text + "\n\n[00ff00]" + LocaManager.GetText("TOOLTIP_SKILL_MAX_GRANDMASTER") + "[-]";
					}
					else if (p_skill.MaxTier == ETier.MASTER)
					{
						text = text + "\n\n[00ff00]" + LocaManager.GetText("TOOLTIP_SKILL_MAX_MASTER") + "[-]";
					}
					else if (p_skill.MaxTier == ETier.EXPERT)
					{
						text = text + "\n\n[00ff00]" + LocaManager.GetText("TOOLTIP_SKILL_MAX_EXPERT") + "[-]";
					}
					if (p_isDefault)
					{
						text = text + "\n\n[ff0000]" + LocaManager.GetText("TOOLTIP_SKILL_IS_DEFAULT") + "[-]";
					}
				}
				m_description.UpdateText(text);
			}
			else
			{
				String increaseDescription = GetIncreaseDescription(p_skill, p_skill.Level, true);
				m_description.UpdateText(increaseDescription);
			}
		}

		private void FillSkillEffect(Skill p_skill, TooltipType p_type)
		{
			if (p_type == TooltipType.CURRENT_EFFECT_NEXT)
			{
				String increaseDescription = GetIncreaseDescription(p_skill, p_skill.Level + 1, false);
				m_effect.UpdateText(increaseDescription);
				m_effect.Label.color = m_unlearnedColor;
			}
			else if (p_type == TooltipType.CURRENT_EFFECT_PREV)
			{
				String increaseDescription2 = GetIncreaseDescription(p_skill, p_skill.Level - 1, false);
				m_effect.UpdateText(increaseDescription2);
				m_effect.Label.color = m_unlearnedColor;
			}
			else if (p_type == TooltipType.BASE)
			{
				String tierBoni = GetTierBoni(p_skill.Tier1Effects, p_skill.Tier >= ETier.NOVICE);
				m_effect.UpdateText(tierBoni);
				m_effect.Label.color = m_normalColor;
			}
			else if (p_type == TooltipType.EXPERT)
			{
				Boolean flag = p_skill.Tier >= ETier.EXPERT;
				String tierBoni2 = GetTierBoni(p_skill.Tier2Effects, flag);
				m_effect.UpdateText(tierBoni2);
				m_effect.Label.color = ((!flag) ? m_unlearnedColor : m_normalColor);
			}
			else if (p_type == TooltipType.MASTER)
			{
				Boolean flag2 = p_skill.Tier >= ETier.MASTER;
				String tierBoni3 = GetTierBoni(p_skill.Tier3Effects, flag2);
				m_effect.UpdateText(tierBoni3);
				m_effect.Label.color = ((!flag2) ? m_unlearnedColor : m_normalColor);
			}
			else if (p_type == TooltipType.GRAND_MASTER)
			{
				Boolean flag3 = p_skill.Tier >= ETier.GRAND_MASTER;
				String tierBoni4 = GetTierBoni(p_skill.Tier4Effects, flag3);
				m_effect.UpdateText(tierBoni4);
				m_effect.Label.color = ((!flag3) ? m_unlearnedColor : m_normalColor);
			}
			else
			{
				if (IsMagicSchoolSkill(p_skill))
				{
					if (p_skill.StaticID == 11)
					{
						m_effect.UpdateText(LocaManager.GetText("TOOLTIP_SKILL_START_WARFARE"));
					}
					else
					{
						m_effect.UpdateText(LocaManager.GetText("TOOLTIP_SKILL_START_SPELL"));
					}
				}
				m_effect.Label.color = m_normalColor;
			}
		}

		private String GetTierBoni(List<SkillEffectStaticData> p_skillEffects, Boolean p_tierAvailable)
		{
			String text = String.Empty;
			for (Int32 i = 0; i < p_skillEffects.Count; i++)
			{
				SkillEffectStaticData skillEffectStaticData = p_skillEffects[i];
				if (skillEffectStaticData.ShowInTooltip)
				{
					String text2 = (!p_tierAvailable) ? m_unlearnedEffectColorHex : m_effectColorHex;
					String str = String.Empty;
					if (skillEffectStaticData.Mode == ESkillEffectMode.NONE)
					{
						str = LocaManager.GetText(skillEffectStaticData.GeneralDescription, text2, "[-]");
					}
					else
					{
						Single num = skillEffectStaticData.Value;
						if (skillEffectStaticData.ContainsPercent)
						{
							num *= 100f;
						}
						String arg = text2 + num.ToString() + "[-]";
						str = LocaManager.GetText(skillEffectStaticData.GeneralDescription, text2, "[-]", arg);
					}
					text += str;
					if (i < p_skillEffects.Count - 1 && p_skillEffects[i + 1].ShowInTooltip)
					{
						text += "\n\n";
					}
				}
			}
			return text;
		}

		private void AdjustLabelPosition(UILabel p_label, Single p_offset)
		{
			Vector2 v = p_label.transform.localPosition;
			v.y -= p_offset;
			p_label.transform.localPosition = v;
		}

		private String GetIncreaseDescription(Skill p_skill, Int32 p_level, Boolean p_skillAvailable)
		{
			String text = (!p_skillAvailable) ? m_unlearnedEffectColorHex : m_effectColorHex;
			String text2 = String.Empty;
			List<SkillEffectStaticData> list = (p_skill.Tier < ETier.NOVICE) ? p_skill.Tier1ScalingEffects : p_skill.AvailableScalingEffects;
			foreach (SkillEffectStaticData skillEffectStaticData in list)
			{
				String dynamicDescription = skillEffectStaticData.DynamicDescription;
				if (dynamicDescription != String.Empty)
				{
					if (text2 != String.Empty)
					{
						text2 += "\n\n";
					}
					Single num = skillEffectStaticData.Value;
					if (skillEffectStaticData.ContainsPercent)
					{
						num *= 100f;
					}
					String arg = text + (num * p_level).ToString() + "[-]";
					text2 = text2 + LocaManager.GetText(dynamicDescription, text, "[-]", arg) + " ";
				}
			}
			if (text2 != String.Empty)
			{
				return text2;
			}
			return String.Empty;
		}

		public void Show()
		{
			NGUITools.SetActiveSelf(gameObject, true);
		}

		public void Hide()
		{
			NGUITools.SetActiveSelf(gameObject, false);
		}

		private Boolean IsMagicSchoolSkill(Skill p_skill)
		{
			return p_skill.StaticID == 21 || p_skill.StaticID == 22 || p_skill.StaticID == 19 || p_skill.StaticID == 20 || p_skill.StaticID == 23 || p_skill.StaticID == 24 || p_skill.StaticID == 25 || p_skill.StaticID == 11;
		}

		public enum TooltipType
		{
			BASE,
			EXPERT,
			MASTER,
			GRAND_MASTER,
			CURRENT_EFFECT_NEXT,
			CURRENT_EFFECT_PREV,
			PARTY_CREATION
		}
	}
}
