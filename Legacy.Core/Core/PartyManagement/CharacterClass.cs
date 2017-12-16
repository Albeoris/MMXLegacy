using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Skills;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class CharacterClass
	{
		private EClass m_class;

		private String m_nameKey;

		private String m_advancedNameKey;

		private ERace m_race;

		private Attributes m_initialAttributes;

		private ResistanceCollection m_initialResistance;

		private CharacterClassStaticData m_data;

		private Int32[] m_expertSkills;

		private Int32[] m_masterSkills;

		private Int32[] m_grandMasterSkills;

		private Int32[] m_startSkills;

		private Boolean m_isAdvanced;

		public CharacterClass(EClass p_class)
		{
			m_class = p_class;
			m_data = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)p_class);
			m_race = m_data.Race;
			m_nameKey = m_data.NameKey;
			m_advancedNameKey = m_data.AdvancedNameKey;
			m_expertSkills = m_data.ExpertSkills;
			m_masterSkills = m_data.MasterSkills;
			m_grandMasterSkills = m_data.GrandMasterSkills;
			m_startSkills = m_data.StartSkills;
			m_initialResistance = new ResistanceCollection();
			m_initialResistance.Add(m_data.ResAir);
			m_initialResistance.Add(m_data.ResEarth);
			m_initialResistance.Add(m_data.ResFire);
			m_initialResistance.Add(m_data.ResWater);
			m_initialResistance.Add(m_data.ResDark);
			m_initialResistance.Add(m_data.ResLight);
			m_initialResistance.Add(m_data.ResPrimordial);
			m_initialAttributes = new Attributes(m_data.BaseMight, m_data.BaseMagic, m_data.BasePerception, m_data.BaseDestiny, m_data.BaseVitality, m_data.BaseSpirit, m_data.BaseHP, m_data.BaseMana);
		}

		public EClass Class => m_class;

	    public String NameKey
		{
			get
			{
				if (m_isAdvanced)
				{
					return m_advancedNameKey;
				}
				return m_nameKey;
			}
		}

		public String AdvancedNameKey => m_advancedNameKey;

	    public ERace Race => m_race;

	    public CharacterClassStaticData StaticData => m_data;

	    public String RaceKey
		{
			get
			{
				if (m_race == ERace.HUMAN)
				{
					return "RACE_HUMAN";
				}
				if (m_race == ERace.ELF)
				{
					return "RACE_ELF";
				}
				if (m_race == ERace.DWARF)
				{
					return "RACE_DWARF";
				}
				return "RACE_ORC";
			}
		}

		public Boolean IsAdvanced
		{
			get => m_isAdvanced;
		    set => m_isAdvanced = value;
		}

		public String GetPosingTexName()
		{
			return m_data.PosingBase;
		}

		public Attributes InitialAttributes => m_initialAttributes;

	    public ResistanceCollection InitialResistance => m_initialResistance;

	    public Int32[] StartSkills => m_startSkills;

	    public Int32[] GetSkillIDs(ETier p_tier)
		{
			switch (p_tier)
			{
			case ETier.NOVICE:
				return m_startSkills;
			case ETier.EXPERT:
				return m_expertSkills;
			case ETier.MASTER:
				return m_masterSkills;
			case ETier.GRAND_MASTER:
				return m_grandMasterSkills;
			default:
				return new Int32[0];
			}
		}

		public Boolean IsStartSkill(Int32 p_id)
		{
			for (Int32 i = 0; i < m_startSkills.Length; i++)
			{
				if (m_startSkills[i] == p_id)
				{
					return true;
				}
			}
			return false;
		}

		public String[] GetDefaultNames(EGender p_gender)
		{
			if (p_gender == EGender.MALE)
			{
				return m_data.DefaultMaleNames;
			}
			return m_data.DefaultFemaleNames;
		}

		public EClassType GetClassType()
		{
			if (m_class == EClass.MERCENARY || m_class == EClass.BLADEDANCER || m_class == EClass.DEFENDER || m_class == EClass.BARBARIAN)
			{
				return EClassType.MIGHT;
			}
			if (m_class == EClass.FREEMAGE || m_class == EClass.DRUID || m_class == EClass.RUNEPRIEST || m_class == EClass.SHAMAN)
			{
				return EClassType.MAGIC;
			}
			return EClassType.HYBRID;
		}

		public Single GetHPPerVitality()
		{
			Single num = ConfigManager.Instance.Game.HealthPerVitality;
			if (m_race == ERace.DWARF)
			{
				RacialAbilitiesStaticData staticData = StaticDataHandler.GetStaticData<RacialAbilitiesStaticData>(EDataType.RACIAL_ABILITIES, 6);
				num += staticData.Value;
			}
			return num;
		}

		public Int32 GetEvadeValue()
		{
			Int32 result = 0;
			if (m_race == ERace.ELF)
			{
				RacialAbilitiesStaticData staticData = StaticDataHandler.GetStaticData<RacialAbilitiesStaticData>(EDataType.RACIAL_ABILITIES, 4);
				result = staticData.Value;
			}
			return result;
		}

		public Boolean CanUnlock(ETokenID p_id)
		{
			return !m_isAdvanced && ((m_class == EClass.MERCENARY && p_id == ETokenID.TOKEN_CLASS_WINDSWORD) || (m_class == EClass.CRUSADER && p_id == ETokenID.TOKEN_CLASS_PALADIN) || (m_class == EClass.FREEMAGE && p_id == ETokenID.TOKEN_CLASS_ARCHMAGE) || (m_class == EClass.BLADEDANCER && p_id == ETokenID.TOKEN_CLASS_BLADEMASTER) || (m_class == EClass.RANGER && p_id == ETokenID.TOKEN_CLASS_WARDEN) || (m_class == EClass.DRUID && p_id == ETokenID.TOKEN_CLASS_DRUID_ELDER) || (m_class == EClass.DEFENDER && p_id == ETokenID.TOKEN_CLASS_SHIELD_GUARD) || (m_class == EClass.SCOUT && p_id == ETokenID.TOKEN_CLASS_PATHFINDER) || (m_class == EClass.RUNEPRIEST && p_id == ETokenID.TOKEN_CLASS_RUNELORD) || (m_class == EClass.BARBARIAN && p_id == ETokenID.TOKEN_CLASS_WARMONGER) || (m_class == EClass.HUNTER && p_id == ETokenID.TOKEN_CLASS_MARAUDER) || (m_class == EClass.SHAMAN && p_id == ETokenID.TOKEN_CLASS_BLOODCALLER));
		}

		public Boolean IsWindsword()
		{
			return m_isAdvanced && m_class == EClass.MERCENARY;
		}

		public Boolean IsBlademaster()
		{
			return m_isAdvanced && m_class == EClass.BLADEDANCER;
		}

		public Boolean IsShieldGuard()
		{
			return m_isAdvanced && m_class == EClass.DEFENDER;
		}

		public Boolean IsPathfinder()
		{
			return m_isAdvanced && m_class == EClass.SCOUT;
		}

		public Boolean IsWarmonger()
		{
			return m_isAdvanced && m_class == EClass.BARBARIAN;
		}

		public Boolean IsBloodcaller()
		{
			return m_isAdvanced && m_class == EClass.SHAMAN;
		}

		public List<RacialAbilitiesStaticData> GetRacialAbilities()
		{
			List<RacialAbilitiesStaticData> list = new List<RacialAbilitiesStaticData>();
			List<RacialAbilitiesStaticData> list2 = new List<RacialAbilitiesStaticData>(StaticDataHandler.GetIterator<RacialAbilitiesStaticData>(EDataType.RACIAL_ABILITIES));
			foreach (RacialAbilitiesStaticData racialAbilitiesStaticData in list2)
			{
				if (racialAbilitiesStaticData.Race == m_race)
				{
					list.Add(racialAbilitiesStaticData);
				}
			}
			return list;
		}

		public List<ParagonAbilitiesStaticData> GetParagonAbilities(Boolean p_hideUnlearned)
		{
			List<ParagonAbilitiesStaticData> list = new List<ParagonAbilitiesStaticData>();
			if (p_hideUnlearned && !IsAdvanced)
			{
				return list;
			}
			List<ParagonAbilitiesStaticData> list2 = new List<ParagonAbilitiesStaticData>(StaticDataHandler.GetIterator<ParagonAbilitiesStaticData>(EDataType.PARAGON_ABILITES));
			foreach (ParagonAbilitiesStaticData paragonAbilitiesStaticData in list2)
			{
				if (paragonAbilitiesStaticData.Class == m_class)
				{
					list.Add(paragonAbilitiesStaticData);
				}
			}
			return list;
		}
	}
}
