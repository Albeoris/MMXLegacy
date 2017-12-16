using System;
using Legacy.Core;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/SummaryCharacter")]
	public class SummaryCharacter : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_nameLabel;

		[SerializeField]
		private UILabel m_raceClassLabel;

		[SerializeField]
		private UISprite m_Portrait;

		[SerializeField]
		private UISprite m_body;

		[SerializeField]
		private UILabel m_healthCaption;

		[SerializeField]
		private UILabel m_manaCaption;

		[SerializeField]
		private UILabel m_mightCaption;

		[SerializeField]
		private UILabel m_magicCaption;

		[SerializeField]
		private UILabel m_perceptionCaption;

		[SerializeField]
		private UILabel m_destinyCaption;

		[SerializeField]
		private UILabel m_vitalityCaption;

		[SerializeField]
		private UILabel m_spiritCaption;

		[SerializeField]
		private UILabel m_healthtValue;

		[SerializeField]
		private UILabel m_manaValue;

		[SerializeField]
		private UILabel m_mightValue;

		[SerializeField]
		private UILabel m_magicValue;

		[SerializeField]
		private UILabel m_perceptionValue;

		[SerializeField]
		private UILabel m_destinyValue;

		[SerializeField]
		private UILabel m_vitalityValue;

		[SerializeField]
		private UILabel m_spiritValue;

		[SerializeField]
		private GameObject m_skillsRoot;

		[SerializeField]
		private SummarySkill[] m_selectedSkills;

		private DummyCharacter m_character;

		public void Init(DummyCharacter p_char)
		{
			m_character = p_char;
			m_nameLabel.text = m_character.Name;
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)m_character.Class);
			String text = staticData.NameKey;
			if (m_character.Gender == EGender.MALE)
			{
				text += "_M";
			}
			else
			{
				text += "_F";
			}
			m_raceClassLabel.text = LocaManager.GetText(m_character.GetRaceKey()) + " " + LocaManager.GetText(text);
			m_Portrait.spriteName = m_character.GetPortrait();
			m_body.spriteName = m_character.GetBodySprite();
			Attributes classAttributes = m_character.GetClassAttributes();
			m_healthCaption.text = LocaManager.GetText("CHARACTER_ATTRIBUTE_HEALTH");
			m_manaCaption.text = LocaManager.GetText("CHARACTER_ATTRIBUTE_MANA");
			m_mightCaption.text = LocaManager.GetText("CHARACTER_ATTRIBUTE_MIGHT");
			m_magicCaption.text = LocaManager.GetText("CHARACTER_ATTRIBUTE_MAGIC");
			m_perceptionCaption.text = LocaManager.GetText("CHARACTER_ATTRIBUTE_PERCEPTION");
			m_destinyCaption.text = LocaManager.GetText("CHARACTER_ATTRIBUTE_DESTINY");
			m_vitalityCaption.text = LocaManager.GetText("CHARACTER_ATTRIBUTE_VITALITY");
			m_spiritCaption.text = LocaManager.GetText("CHARACTER_ATTRIBUTE_SPIRIT");
			m_healthtValue.text = m_character.BaseAttributes.HealthPoints.ToString();
			m_manaValue.text = m_character.BaseAttributes.ManaPoints.ToString();
			m_mightValue.text = (classAttributes.Might + m_character.BaseAttributes.Might).ToString();
			m_magicValue.text = (classAttributes.Magic + m_character.BaseAttributes.Magic).ToString();
			m_perceptionValue.text = (classAttributes.Perception + m_character.BaseAttributes.Perception).ToString();
			m_destinyValue.text = (classAttributes.Destiny + m_character.BaseAttributes.Destiny).ToString();
			m_vitalityValue.text = (classAttributes.Vitality + m_character.BaseAttributes.Vitality).ToString();
			m_spiritValue.text = (classAttributes.Spirit + m_character.BaseAttributes.Spirit).ToString();
			UpdateLabelPositions();
			UpdateSkill(0, staticData.StartSkills[0]);
			UpdateSkill(1, staticData.StartSkills[1]);
			for (Int32 i = 0; i < 4; i++)
			{
				UpdateSkill(i + 2, m_character.SelectedSkills[i]);
			}
			if (m_character.Race == ERace.HUMAN)
			{
				m_skillsRoot.transform.localPosition = new Vector3(165f, -220f, 0f);
			}
			else
			{
				m_skillsRoot.transform.localPosition = new Vector3(270f, -220f, 0f);
			}
		}

		private void UpdateSkill(Int32 p_index, Int32 p_skill)
		{
			if (p_skill > 0)
			{
				SkillStaticData staticData = StaticDataHandler.GetStaticData<SkillStaticData>(EDataType.SKILL, p_skill);
				m_selectedSkills[p_index].Init(staticData, m_character);
			}
			else
			{
				m_selectedSkills[p_index].Init(null, m_character);
			}
		}

		private void UpdateLabelPositions()
		{
			Single num = 0f;
			Single num2 = 0f;
			num = Math.Max(num, GetLabelWidth(m_mightCaption));
			num = Math.Max(num, GetLabelWidth(m_magicCaption));
			num = Math.Max(num, GetLabelWidth(m_perceptionCaption));
			num = Math.Max(num, GetLabelWidth(m_healthCaption));
			num2 = Math.Max(num2, GetLabelWidth(m_mightValue));
			num2 = Math.Max(num2, GetLabelWidth(m_magicValue));
			num2 = Math.Max(num2, GetLabelWidth(m_perceptionValue));
			num2 = Math.Max(num2, GetLabelWidth(m_healthtValue));
			Single x = m_mightCaption.transform.localPosition.x + num + 5f + num2;
			m_mightValue.transform.localPosition = new Vector3(x, m_mightValue.transform.localPosition.y, m_mightValue.transform.localPosition.z);
			m_magicValue.transform.localPosition = new Vector3(x, m_magicValue.transform.localPosition.y, m_magicValue.transform.localPosition.z);
			m_perceptionValue.transform.localPosition = new Vector3(x, m_perceptionValue.transform.localPosition.y, m_perceptionValue.transform.localPosition.z);
			m_healthtValue.transform.localPosition = new Vector3(x, m_healthtValue.transform.localPosition.y, m_healthtValue.transform.localPosition.z);
			num = 0f;
			num2 = 0f;
			num = Math.Max(num, GetLabelWidth(m_destinyCaption));
			num = Math.Max(num, GetLabelWidth(m_vitalityCaption));
			num = Math.Max(num, GetLabelWidth(m_spiritCaption));
			num = Math.Max(num, GetLabelWidth(m_manaCaption));
			num2 = Math.Max(num2, GetLabelWidth(m_destinyValue));
			num2 = Math.Max(num2, GetLabelWidth(m_vitalityValue));
			num2 = Math.Max(num2, GetLabelWidth(m_spiritValue));
			num2 = Math.Max(num2, GetLabelWidth(m_manaValue));
			x = m_destinyValue.transform.localPosition.x - num2 - 5f - num;
			m_destinyCaption.transform.localPosition = new Vector3(x, m_destinyCaption.transform.localPosition.y, m_destinyCaption.transform.localPosition.z);
			m_vitalityCaption.transform.localPosition = new Vector3(x, m_vitalityCaption.transform.localPosition.y, m_vitalityCaption.transform.localPosition.z);
			m_spiritCaption.transform.localPosition = new Vector3(x, m_spiritCaption.transform.localPosition.y, m_spiritCaption.transform.localPosition.z);
			m_manaCaption.transform.localPosition = new Vector3(x, m_manaCaption.transform.localPosition.y, m_manaCaption.transform.localPosition.z);
		}

		private Single GetLabelWidth(UILabel p_label)
		{
			return p_label.relativeSize.x * p_label.transform.localScale.x;
		}
	}
}
