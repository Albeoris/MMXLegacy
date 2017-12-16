using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class PartyCreator
	{
		private Party m_party;

		private DummyCharacter[] m_dummyChars;

		public Int32 m_selectedCharacter;

		public PartyCreator()
		{
			m_dummyChars = new DummyCharacter[4];
		}

		public Party CurrentParty
		{
			get
			{
				if (m_party == null)
				{
					m_party = new Party(0);
				}
				return m_party;
			}
		}

		public void ClearAndDestroy()
		{
			if (m_party != null)
			{
				m_party.Destroy();
				m_party = null;
			}
		}

		public void SelectCharacter(Int32 i)
		{
			m_selectedCharacter = i;
		}

		public void SelectPreviousCharacter()
		{
			m_selectedCharacter--;
			if (m_selectedCharacter < 0)
			{
				m_selectedCharacter = 3;
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(GetSelectedDummyCharacter(), EEventType.DUMMY_CHARACTER_SELECTED, EventArgs.Empty);
		}

		public void SelectNextCharacter()
		{
			m_selectedCharacter++;
			if (m_selectedCharacter >= 4)
			{
				m_selectedCharacter = 0;
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(GetSelectedDummyCharacter(), EEventType.DUMMY_CHARACTER_SELECTED, EventArgs.Empty);
		}

		public DummyCharacter GetSelectedDummyCharacter()
		{
			return m_dummyChars[m_selectedCharacter];
		}

		public Int32 GetSelectedCharacterIndex()
		{
			return m_selectedCharacter;
		}

		public DummyCharacter GetDummyCharacter(Int32 p_index)
		{
			return m_dummyChars[p_index];
		}

		public void InitManualCreation()
		{
			for (Int32 i = 0; i < 4; i++)
			{
				m_dummyChars[i] = new DummyCharacter();
			}
		}

		public void RandomizeParty()
		{
			for (Int32 i = 0; i < 4; i++)
			{
				m_selectedCharacter = i;
				m_dummyChars[i].Race = GetUnusedRace();
				m_dummyChars[i].Class = GetUnusedClass();
				m_dummyChars[i].Gender = ((Random.Range(0, 100) < 50) ? EGender.FEMALE : EGender.MALE);
				m_dummyChars[i].PortraitID = ((Random.Range(0, 100) < 50) ? 2 : 1);
				m_dummyChars[i].Name = GetRandomName(m_dummyChars[i]);
				m_dummyChars[i].LastConfirmedStep = 4;
				AssignRandomSkills(m_dummyChars[i]);
				AssignRandomAttributes(m_dummyChars[i]);
			}
		}

		public void FinalizePartyCreation()
		{
			m_party = CurrentParty;
			for (Int32 i = 0; i < 4; i++)
			{
				Character p_member = m_dummyChars[i].CreateCharacter(m_party);
				m_party.AddMember(i, p_member);
			}
		}

		public void CreateDefaultParty()
		{
			m_party = CurrentParty;
			Int32[] partyMembers = ConfigManager.Instance.Game.PartyMembers;
			String[] array = new String[]
			{
				"DWARF_MALE_NAME_1",
				"ORC_MALE_NAME_1",
				"ELF_FEMALE_NAME_1",
				"HUM_FEMALE_NAME_1"
			};
			for (Int32 i = 0; i < 4; i++)
			{
				m_selectedCharacter = i;
			    m_dummyChars[i].Race = (ERace)((partyMembers[i] + (Int32)ERace.ELF) / (Int32)ERace.DWARF);
				m_dummyChars[i].Class = (EClass)partyMembers[i];
				m_dummyChars[i].Gender = ((i >= 2) ? EGender.FEMALE : EGender.MALE);
				m_dummyChars[i].PortraitID = ((i != 1 && i != 3) ? 1 : 2);
				m_dummyChars[i].Name = Localization.Instance.GetText(array[i]);
				m_dummyChars[i].LastConfirmedStep = 4;
				m_dummyChars[i].SetStartSkills();
				m_dummyChars[i].SetDefaultAttributes();
			}
		}

		public void GiveStartSetup()
		{
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = m_party.GetMember(i);
				member.GiveStartSetup();
			}
			for (Int32 j = 0; j < 5; j++)
			{
				m_party.Inventory.AddItem(ItemFactory.CreateItem<Potion>(1));
			}
			for (Int32 k = 0; k < 5; k++)
			{
				m_party.Inventory.AddItem(ItemFactory.CreatePotion(EPotionType.MANA_POTION, 1));
			}
			m_party.ChangeGold(200);
			m_party.CheckUnlockUPlayPrivilegesRewards();
			ItemFactory.InitItemProbabilities();
			m_party.SetSupplies(ConfigManager.Instance.Game.StartSupplies);
			m_party.TokenHandler.AddToken(528);
			LegacyLogic.Instance.WorldManager.QuestHandler.ActivateQuest(15);
			for (Int32 l = 0; l < 4; l++)
			{
				Character member2 = m_party.GetMember(l);
				if (member2.Class.Race == ERace.HUMAN)
				{
					m_party.TokenHandler.AddToken(736);
				}
				else if (member2.Class.Race == ERace.DWARF)
				{
					m_party.TokenHandler.AddToken(739);
				}
				else if (member2.Class.Race == ERace.ELF)
				{
					m_party.TokenHandler.AddToken(738);
				}
				else if (member2.Class.Race == ERace.ORC)
				{
					m_party.TokenHandler.AddToken(737);
				}
				if (member2.Class.Class == EClass.RANGER)
				{
					m_party.TokenHandler.AddToken(757);
				}
			}
		}

		public void AssignRandomSkills(DummyCharacter p_character)
		{
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)p_character.Class);
			List<Int32> list = new List<Int32>();
			List<Int32> list2 = new List<Int32>(staticData.StartSkills);
			p_character.ResetPickedSkills();
			for (Int32 i = 0; i < staticData.GrandMasterSkills.Length; i++)
			{
				Int32 num = staticData.GrandMasterSkills[i];
				if (!p_character.IsSkillPicked(num) && !list2.Contains(num))
				{
					list.Add(num);
				}
			}
			for (Int32 j = 0; j < staticData.MasterSkills.Length; j++)
			{
				Int32 num2 = staticData.MasterSkills[j];
				if (!p_character.IsSkillPicked(num2) && !list2.Contains(num2))
				{
					list.Add(num2);
				}
			}
			for (Int32 k = 0; k < staticData.ExpertSkills.Length; k++)
			{
				Int32 num3 = staticData.ExpertSkills[k];
				if (!p_character.IsSkillPicked(num3) && !list2.Contains(num3))
				{
					list.Add(num3);
				}
			}
			Int32 num4 = (staticData.Race != ERace.HUMAN) ? 2 : 4;
			for (Int32 l = 0; l < num4; l++)
			{
				Int32 index = Random.Range(0, list.Count);
				p_character.PickSkill(list[index]);
				list.RemoveAt(index);
			}
		}

		public void AssignRandomAttributes(DummyCharacter p_character)
		{
			p_character.ResetAttributes();
			for (Int32 i = 0; i < 5; i++)
			{
				switch (Random.Range(0, 6))
				{
				case 0:
					p_character.IncreaseMight();
					break;
				case 1:
					p_character.IncreaseMagic();
					break;
				case 2:
					p_character.IncreasePerception();
					break;
				case 3:
					p_character.IncreaseDestiny();
					break;
				case 4:
					p_character.IncreaseVitality();
					break;
				case 5:
					p_character.IncreaseSpirit();
					break;
				}
			}
		}

		public ERace GetUnusedRace()
		{
			List<ERace> list = new List<ERace>(4);
			list.Add(ERace.HUMAN);
			list.Add(ERace.ELF);
			list.Add(ERace.DWARF);
			list.Add(ERace.ORC);
			for (Int32 i = 0; i < 4; i++)
			{
				if (m_dummyChars[i].Race != ERace.NONE)
				{
					list.Remove(m_dummyChars[i].Race);
				}
			}
			return list[Random.Range(0, list.Count)];
		}

		public EClass GetUnusedClass()
		{
			DummyCharacter selectedDummyCharacter = GetSelectedDummyCharacter();
			List<EClass> classListForRace = GetClassListForRace(selectedDummyCharacter.Race);
			for (Int32 i = 0; i < 4; i++)
			{
				if (m_dummyChars[i].Class != EClass.NONE)
				{
					classListForRace.Remove(m_dummyChars[i].Class);
				}
			}
			EClass result;
			if (classListForRace.Count > 0)
			{
				result = classListForRace[Random.Range(0, classListForRace.Count)];
			}
			else
			{
				classListForRace = GetClassListForRace(selectedDummyCharacter.Race);
				result = classListForRace[Random.Range(0, classListForRace.Count)];
			}
			return result;
		}

		private List<EClass> GetClassListForRace(ERace p_race)
		{
			List<EClass> list = new List<EClass>();
			if (p_race == ERace.HUMAN)
			{
				AddClassIfAvailable(list, EClass.MERCENARY);
				AddClassIfAvailable(list, EClass.CRUSADER);
				AddClassIfAvailable(list, EClass.FREEMAGE);
			}
			if (p_race == ERace.ELF)
			{
				AddClassIfAvailable(list, EClass.BLADEDANCER);
				AddClassIfAvailable(list, EClass.RANGER);
				AddClassIfAvailable(list, EClass.DRUID);
			}
			if (p_race == ERace.DWARF)
			{
				AddClassIfAvailable(list, EClass.DEFENDER);
				AddClassIfAvailable(list, EClass.SCOUT);
				AddClassIfAvailable(list, EClass.RUNEPRIEST);
			}
			if (p_race == ERace.ORC)
			{
				AddClassIfAvailable(list, EClass.BARBARIAN);
				AddClassIfAvailable(list, EClass.HUNTER);
				AddClassIfAvailable(list, EClass.SHAMAN);
			}
			return list;
		}

		private void AddClassIfAvailable(List<EClass> p_list, EClass p_class)
		{
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)p_class);
			if (staticData != null)
			{
				p_list.Add(p_class);
			}
		}

		public String GetRandomName(DummyCharacter c)
		{
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)c.Class);
			String[] array;
			if (c.Gender == EGender.MALE)
			{
				array = staticData.DefaultMaleNames;
			}
			else
			{
				array = staticData.DefaultFemaleNames;
			}
			return Localization.Instance.GetText(array[Random.Range(0, array.Length)]);
		}

		public Boolean CheckCustomFinished()
		{
			for (Int32 i = 0; i < 4; i++)
			{
				if (!m_dummyChars[i].CheckCustomFinished())
				{
					return false;
				}
			}
			return true;
		}

		public Boolean CheckSkillsFinished()
		{
			return m_dummyChars[m_selectedCharacter].GetSkillsToPickLeft() == 0;
		}

		public Boolean CheckAttributesFinished()
		{
			return m_dummyChars[m_selectedCharacter].GetAttributesToPickLeft() == 0;
		}

		public Boolean CheckSkillsFinished(Int32 p_index)
		{
			return m_dummyChars[p_index].GetSkillsToPickLeft() == 0;
		}

		public Boolean CheckAttributesFinished(Int32 p_index)
		{
			return m_dummyChars[p_index].GetAttributesToPickLeft() == 0;
		}
	}
}
