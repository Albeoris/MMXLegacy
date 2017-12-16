using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.Cheats
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsCharacters")]
	public class CheatsCharacters : MonoBehaviour
	{
		private const String CHARACTER_IDENTIFIER_ALL = "All";

		[SerializeField]
		private UIPopupList m_characterList;

		[SerializeField]
		private UIInput m_skillPointsInput;

		[SerializeField]
		private UIPopupList m_skillList;

		[SerializeField]
		private UIPopupList m_conditionList;

		private List<Character> m_characters;

		private List<Character> m_charactersSelected;

		private List<Int32> m_skillIDMapping;

		private void Awake()
		{
			m_characters = new List<Character>();
			m_charactersSelected = new List<Character>();
			m_skillIDMapping = new List<Int32>();
			FillConditionList();
		}

		private void OnEnable()
		{
			if (m_characterList != null)
			{
				m_characterList.items.Clear();
				m_characterList.items.Add("All");
				Party party = LegacyLogic.Instance.WorldManager.Party;
				for (Int32 i = 0; i < 4; i++)
				{
					Character member = party.GetMember(i);
					if (member != null)
					{
						m_characters.Add(member);
						m_characterList.items.Add(member.Name);
					}
				}
				m_characterList.selection = m_characterList.items[0];
			}
			UpdateCharacterSelection();
		}

		private void OnDisable()
		{
			m_charactersSelected.Clear();
			m_characters.Clear();
		}

		private void OnSelectionChange()
		{
			UpdateCharacterSelection();
			FillSkillList();
			m_skillList.selection = m_skillList.items[0];
		}

		private void FillSkillList()
		{
			m_skillList.items.Clear();
			m_skillIDMapping.Clear();
			foreach (Character character in m_charactersSelected)
			{
				List<Skill> availableSkills = character.SkillHandler.AvailableSkills;
				foreach (Skill skill in availableSkills)
				{
					if (!m_skillIDMapping.Contains(skill.StaticID))
					{
						m_skillIDMapping.Add(skill.StaticID);
					}
				}
			}
			foreach (Int32 p_skillStaticID in m_skillIDMapping)
			{
				String text = String.Empty;
				String str = String.Empty;
				foreach (Character character2 in m_charactersSelected)
				{
					Skill skill2 = character2.SkillHandler.FindSkill(p_skillStaticID);
					if (text != String.Empty)
					{
						text += " ";
					}
					if (skill2 != null)
					{
						str = LocaManager.GetText(skill2.Name);
						text += (Int32)skill2.Tier;
					}
					else
					{
						text += "-";
					}
				}
				m_skillList.items.Add(str + " [4477CC](" + text + ")[-]");
			}
		}

		private void FillConditionList()
		{
			String[] names = Enum.GetNames(typeof(ECondition));
			for (Int32 i = 1; i < names.Length; i++)
			{
				m_conditionList.items.Add(names[i]);
			}
			m_conditionList.items.Sort();
			m_conditionList.selection = m_conditionList.items[0];
		}

		public void OnLevelUpButtonClick()
		{
			foreach (Character character in m_charactersSelected)
			{
				character.AddExp(character.ExpNeededForNextLevel);
			}
		}

		public void OnAddConditionButtonClick()
		{
			ECondition econdition = (ECondition)Enum.Parse(typeof(ECondition), m_conditionList.selection, true);
			foreach (Character character in m_charactersSelected)
			{
				character.ConditionHandler.AddCondition(econdition);
				if ((econdition == ECondition.UNCONSCIOUS || econdition == ECondition.DEAD) && character.HealthPoints > 0)
				{
					character.ChangeHP((econdition != ECondition.UNCONSCIOUS) ? (-(character.HealthPoints * 2)) : (-character.HealthPoints));
				}
			}
		}

		public void OnConditionResetButtonClick()
		{
			foreach (Character character in m_charactersSelected)
			{
				for (Int32 i = 0; i < EnumUtil<ECondition>.Length; i++)
				{
					ECondition p_condition = (ECondition)(1 << i);
					if (character.ConditionHandler.HasCondition(p_condition))
					{
						character.ConditionHandler.RemoveCondition(p_condition);
					}
				}
				if (character.HealthPoints <= 0)
				{
					character.ChangeHP(character.MaximumHealthPoints - character.HealthPoints);
				}
			}
		}

		public void OnAddSkillPointsButtonClick()
		{
			Int32 num = 0;
			try
			{
				num = Int32.Parse(m_skillPointsInput.text);
			}
			catch (FormatException ex)
			{
				Debug.Log(ex.Message);
			}
			foreach (Character character in m_charactersSelected)
			{
				character.SkillPoints += num;
			}
		}

		private void OnNextTierButtonClick()
		{
			Int32 index = m_skillList.items.IndexOf(m_skillList.selection);
			foreach (Character character in m_charactersSelected)
			{
				Skill skill = character.SkillHandler.FindSkill(m_skillIDMapping[index]);
				if (skill != null && skill.Tier != skill.MaxTier)
				{
					ETier etier = skill.Tier + 1;
					character.SkillHandler.SetSkillTier(skill.StaticID, etier);
					AddSpells(character, skill.StaticID, etier);
				}
			}
			FillSkillList();
			m_skillList.selection = m_skillList.items[index];
		}

		private void OnAllTierButtonClick()
		{
			foreach (Character character in m_charactersSelected)
			{
				List<Skill> availableSkills = character.SkillHandler.AvailableSkills;
				foreach (Skill skill in availableSkills)
				{
					while (skill.Tier < skill.MaxTier)
					{
						ETier etier = skill.Tier + 1;
						character.SkillHandler.SetSkillTier(skill.StaticID, etier);
						AddSpells(character, skill.StaticID, etier);
					}
				}
			}
			FillSkillList();
			m_skillList.selection = m_skillList.items[0];
		}

		private void OnAllMaxButtonClick()
		{
			OnConditionResetButtonClick();
			OnAllTierButtonClick();
			foreach (Character character in m_charactersSelected)
			{
				while (character.Level < 50)
				{
					character.AddExp(character.ExpNeededForNextLevel);
				}
				Attributes baseAttributes = character.BaseAttributes;
				baseAttributes.Might = 160;
				baseAttributes.Magic = 160;
				baseAttributes.Perception = 160;
				baseAttributes.Destiny = 160;
				baseAttributes.Spirit = 160;
				baseAttributes.Vitality = 160;
				character.BaseAttributes = baseAttributes;
				character.CalculateCurrentAttributes();
				character.ChangeHP(character.MaximumHealthPoints - character.HealthPoints);
				character.ChangeMP(character.MaximumManaPoints - character.ManaPoints);
			}
		}

		private void AddSpells(Character p_char, Int32 p_skillID, ETier p_tier)
		{
			if (p_skillID == 19 || p_skillID == 20 || p_skillID == 21 || p_skillID == 22 || p_skillID == 23 || p_skillID == 24 || p_skillID == 25)
			{
				ECharacterSpell[] values = EnumUtil<ECharacterSpell>.Values;
				for (Int32 i = 0; i < values.Length; i++)
				{
					if (values[i] > 0)
					{
						CharacterSpellStaticData staticData = StaticDataHandler.GetStaticData<CharacterSpellStaticData>(EDataType.CHARACTER_SPELLS, (Int32)values[i]);
						if (staticData != null && staticData.SkillID == (ESkillID)p_skillID && staticData.Tier <= p_tier)
						{
							p_char.SpellHandler.AddSpell(values[i]);
						}
					}
				}
			}
		}

		private void UpdateCharacterSelection()
		{
			m_charactersSelected.Clear();
			if (m_characterList != null)
			{
				if (m_characterList.selection == "All")
				{
					m_charactersSelected.AddRange(m_characters);
				}
				else
				{
					Int32 index = m_characterList.items.IndexOf(m_characterList.selection) - 1;
					m_charactersSelected.Add(m_characters[index]);
				}
			}
		}
	}
}
