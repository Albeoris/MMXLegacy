using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class DummyCharacter
	{
		public const Int32 SKILLS_TO_PICK = 2;

		public const Int32 SKILLS_TO_PICK_HUMAN = 4;

		public const Int32 ATTRIBUTE_POINT_AMOUNT = 5;

		private String m_name;

		private ERace m_race;

		private EClass m_class;

		private Int32 m_portraitID;

		private EGender m_gender;

		private EVoice m_voice;

		private Int32[] m_selectedSkills;

		private Attributes m_baseAttributes;

		private Int32 m_lastConfirmedStep;

		private Boolean m_customName;

		public DummyCharacter()
		{
			m_name = String.Empty;
			m_race = ERace.NONE;
			m_class = EClass.NONE;
			m_portraitID = 0;
			m_gender = EGender.NOT_SELECTED;
			m_voice = ((Random.Range(0, 100) <= 50) ? EVoice.CYNICAL : EVoice.HEROIC);
			m_selectedSkills = new Int32[4];
			for (Int32 i = 0; i < 4; i++)
			{
				m_selectedSkills[i] = -1;
			}
			ResetAttributes();
			m_lastConfirmedStep = -1;
			m_customName = false;
		}

		public String Name
		{
			get => m_name;
		    set => m_name = value;
		}

		public Boolean CustomName
		{
			get => m_customName;
		    set => m_customName = value;
		}

		public ERace Race
		{
			get => m_race;
		    set
			{
				if (m_race != value)
				{
					ResetPickedSkills();
					m_lastConfirmedStep = -1;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_TABS_NEEED_UPDATE, EventArgs.Empty);
				}
				m_race = value;
				StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STATUS);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_STATUS_CHANGED, p_eventArgs);
			}
		}

		public EClass Class
		{
			get => m_class;
		    set
			{
				if (m_class != value)
				{
					ResetPickedSkills();
					m_lastConfirmedStep = 0;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_TABS_NEEED_UPDATE, EventArgs.Empty);
					m_class = value;
					ResetAttributes();
					StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STATUS);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_STATUS_CHANGED, p_eventArgs);
				}
			}
		}

		public Int32 PortraitID
		{
			get => m_portraitID;
		    set
			{
				m_portraitID = value;
				StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STATUS);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_STATUS_CHANGED, p_eventArgs);
			}
		}

		public EGender Gender
		{
			get => m_gender;
		    set
			{
				m_gender = value;
				StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STATUS);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_STATUS_CHANGED, p_eventArgs);
			}
		}

		public EVoice Voice
		{
			get => m_voice;
		    set => m_voice = value;
		}

		public Int32[] SelectedSkills => m_selectedSkills;

	    public Attributes BaseAttributes => m_baseAttributes;

	    public Character CreateCharacter(Party p_party)
		{
			Character character = new Character(p_party.Buffs);
			CharacterClass p_class = new CharacterClass(m_class);
			character.InitStartValues(p_class, m_portraitID, m_name, m_gender, (EVoiceSetting)m_voice, m_baseAttributes);
			Int32[] startSkills = character.Class.StartSkills;
			for (Int32 i = 0; i < startSkills.Length; i++)
			{
				character.SkillHandler.IncreaseSkillLevel(startSkills[i]);
			}
			for (Int32 i = 0; i < m_selectedSkills.Length; i++)
			{
				if (m_selectedSkills[i] >= 0)
				{
					character.SkillHandler.IncreaseSkillLevel(m_selectedSkills[i]);
				}
			}
			character.SetStandardQuickActions();
			LegacyLogic.Instance.TrackingManager.TrackCharacterCreated(character, m_selectedSkills, m_baseAttributes);
			return character;
		}

		public Int32 GetSkillsToPickLeft()
		{
			Int32 num = 0;
			Int32 num2 = 2;
			if (m_race == ERace.HUMAN)
			{
				num2 = 4;
			}
			for (Int32 i = 0; i < num2; i++)
			{
				if (m_selectedSkills[i] == -1)
				{
					num++;
				}
			}
			return num;
		}

		public Int32 GetAttributesToPickLeft()
		{
			Int32 num = m_baseAttributes.Might + m_baseAttributes.Magic + m_baseAttributes.Destiny + m_baseAttributes.Perception + m_baseAttributes.Vitality + m_baseAttributes.Spirit;
			return 5 - num;
		}

		public void ResetPickedSkills()
		{
			for (Int32 i = 0; i < 4; i++)
			{
				m_selectedSkills[i] = -1;
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void ResetAttributes()
		{
			m_baseAttributes = default(Attributes);
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void IncreaseMight()
		{
			m_baseAttributes.Might = m_baseAttributes.Might + 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void IncreaseMagic()
		{
			m_baseAttributes.Magic = m_baseAttributes.Magic + 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void IncreasePerception()
		{
			m_baseAttributes.Perception = m_baseAttributes.Perception + 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void IncreaseDestiny()
		{
			m_baseAttributes.Destiny = m_baseAttributes.Destiny + 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void IncreaseVitality()
		{
			m_baseAttributes.Vitality = m_baseAttributes.Vitality + 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void IncreaseSpirit()
		{
			m_baseAttributes.Spirit = m_baseAttributes.Spirit + 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void DecreaseMight()
		{
			m_baseAttributes.Might = m_baseAttributes.Might - 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void DecreaseMagic()
		{
			m_baseAttributes.Magic = m_baseAttributes.Magic - 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void DecreasePerception()
		{
			m_baseAttributes.Perception = m_baseAttributes.Perception - 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void DecreaseDestiny()
		{
			m_baseAttributes.Destiny = m_baseAttributes.Destiny - 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void DecreaseVitality()
		{
			m_baseAttributes.Vitality = m_baseAttributes.Vitality - 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void DecreaseSpirit()
		{
			m_baseAttributes.Spirit = m_baseAttributes.Spirit - 1;
			UpdateHPMana();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
		}

		public void SetStartSkills()
		{
			if (m_class != EClass.NONE)
			{
				ResetPickedSkills();
				CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)m_class);
				Int32[] defaultSkills = staticData.DefaultSkills;
				Int32 num = 2;
				if (m_race == ERace.HUMAN)
				{
					num = 4;
				}
				for (Int32 i = 0; i < num; i++)
				{
					PickSkill(defaultSkills[i]);
				}
			}
		}

		public void SetDefaultAttributes()
		{
			if (m_class != EClass.NONE)
			{
				ResetAttributes();
				CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)m_class);
				m_baseAttributes.Might = staticData.DefaultMight;
				m_baseAttributes.Magic = staticData.DefaultMagic;
				m_baseAttributes.Perception = staticData.DefaultPercetpion;
				m_baseAttributes.Destiny = staticData.DefaultDestiny;
				m_baseAttributes.Vitality = staticData.DefaultVitality;
				m_baseAttributes.Spirit = staticData.DefaultSpirit;
				UpdateHPMana();
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
			}
		}

		private void UpdateHPMana()
		{
			GameConfig game = ConfigManager.Instance.Game;
			Attributes classAttributes = GetClassAttributes();
			Single hpperVitality = GetHPPerVitality();
			m_baseAttributes.HealthPoints = classAttributes.HealthPoints + (Int32)(game.HealthPerMight * (m_baseAttributes.Might + classAttributes.Might) + hpperVitality * (m_baseAttributes.Vitality + classAttributes.Vitality));
			m_baseAttributes.ManaPoints = classAttributes.ManaPoints + (Int32)(game.ManaPerMagic * (m_baseAttributes.Magic + classAttributes.Magic) + game.ManaPerSpirit * (m_baseAttributes.Spirit + classAttributes.Spirit));
		}

		public Single GetHPPerVitality()
		{
			GameConfig game = ConfigManager.Instance.Game;
			Single num = game.HealthPerVitality;
			if (Class != EClass.NONE && (m_class == EClass.DEFENDER || m_class == EClass.RUNEPRIEST || m_class == EClass.SCOUT))
			{
				RacialAbilitiesStaticData staticData = StaticDataHandler.GetStaticData<RacialAbilitiesStaticData>(EDataType.RACIAL_ABILITIES, 6);
				num += staticData.Value;
			}
			return num;
		}

		public Attributes GetClassAttributes()
		{
			if (Class != EClass.NONE)
			{
				CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)Class);
				Attributes result = new Attributes(staticData.BaseMight, staticData.BaseMagic, staticData.BasePerception, staticData.BaseDestiny, staticData.BaseVitality, staticData.BaseSpirit, staticData.BaseHP, staticData.BaseMana);
				return result;
			}
			return default(Attributes);
		}

		public void PickSkill(Int32 p_skill)
		{
			if (IsSkillPicked(p_skill))
			{
				return;
			}
			Int32 num = 2;
			if (m_race == ERace.HUMAN)
			{
				num = 4;
			}
			for (Int32 i = 0; i < num; i++)
			{
				if (m_selectedSkills[i] == -1)
				{
					m_selectedSkills[i] = p_skill;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
					return;
				}
			}
		}

		public void UnPickSkill(Int32 selSkill)
		{
			Int32 num = 2;
			if (m_race == ERace.HUMAN)
			{
				num = 4;
			}
			for (Int32 i = 0; i < num; i++)
			{
				if (m_selectedSkills[i] == selSkill)
				{
					m_selectedSkills[i] = -1;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DUMMY_CHARACTER_POINTS_CHANGED, EventArgs.Empty);
					return;
				}
			}
		}

		public Boolean IsSkillPicked(Int32 p_skill)
		{
			Int32 num = 2;
			if (m_race == ERace.HUMAN)
			{
				num = 4;
			}
			for (Int32 i = 0; i < num; i++)
			{
				if (m_selectedSkills[i] == p_skill)
				{
					return true;
				}
			}
			return false;
		}

		public Int32 LastConfirmedStep
		{
			get => m_lastConfirmedStep;
		    set
			{
				m_lastConfirmedStep = value;
				if (m_lastConfirmedStep < -1)
				{
					m_lastConfirmedStep = -1;
				}
			}
		}

		public Int32 GetStepToShow()
		{
			return Math.Min(m_lastConfirmedStep + 1, 4);
		}

		public String GetPortrait()
		{
			if (m_race == ERace.NONE || m_gender == EGender.NOT_SELECTED)
			{
				return "PIC_character_dummy";
			}
			String text = "PIC_head_";
			if (m_race == ERace.HUMAN)
			{
				text += "human";
			}
			else if (m_race == ERace.ELF)
			{
				text += "elf";
			}
			else if (m_race == ERace.DWARF)
			{
				text += "dwarf";
			}
			else if (m_race == ERace.ORC)
			{
				text += "orc";
			}
			if (m_gender == EGender.MALE)
			{
				String text2 = text;
				text = String.Concat(new Object[]
				{
					text2,
					"_male_",
					m_portraitID,
					"_idle"
				});
			}
			else
			{
				String text2 = text;
				text = String.Concat(new Object[]
				{
					text2,
					"_female_",
					m_portraitID,
					"_idle"
				});
			}
			return text;
		}

		public String GetBodySprite()
		{
			if (m_race == ERace.NONE || m_gender == EGender.NOT_SELECTED || m_class == EClass.NONE)
			{
				return String.Empty;
			}
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)m_class);
			String text = staticData.BodyBase;
			if (m_gender == EGender.MALE)
			{
				text += "_male";
			}
			else
			{
				text += "_female";
			}
			return text;
		}

		public String GetRaceKey()
		{
			String result = "RACE_ORC";
			if (m_race == ERace.HUMAN)
			{
				result = "RACE_HUMAN";
			}
			else if (m_race == ERace.ELF)
			{
				result = "RACE_ELF";
			}
			else if (m_race == ERace.DWARF)
			{
				result = "RACE_DWARF";
			}
			return result;
		}

		public Boolean CheckCustomFinished()
		{
			return GetAttributesToPickLeft() == 0 && GetSkillsToPickLeft() == 0;
		}
	}
}
