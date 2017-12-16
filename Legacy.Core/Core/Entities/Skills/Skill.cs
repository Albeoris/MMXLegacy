using System;
using System.Collections.Generic;
using Legacy.Core.Configuration;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Entities.Skills
{
	public class Skill : ISaveGameObject
	{
		private const String SET_TIER_EXCEPTION = "The tier cannot be increased!";

		private const String INCREASE_LEVEL_EXCEPTION = "The skill level cannot be increased!";

		internal Int32 m_requiredSkillLevelNovice;

		internal Int32 m_requiredSkillLevelExpert;

		internal Int32 m_requiredSkillLevelMaster;

		internal Int32 m_requiredSkillLevelGrandMaster;

		private SkillStaticData m_staticData;

		private Int32 m_level;

		private Int32 m_temporaryLevel;

		private Int32 m_virtualSkillLevel;

		private ETier m_tier;

		private ETier m_maxTier;

		private ETier m_maxUnlockedTier;

		internal List<SkillEffectStaticData> m_tier1Effects;

		internal List<SkillEffectStaticData> m_tier2Effects;

		internal List<SkillEffectStaticData> m_tier3Effects;

		internal List<SkillEffectStaticData> m_tier4Effects;

		private List<SkillEffectStaticData> m_currentlyAvailableEffects;

		private List<SkillEffectStaticData> m_availableScalingEffects;

		private List<SkillEffectStaticData> m_tier1ScalingEffects;

		public Skill(Int32 p_skillStaticID, ETier p_maxTier)
		{
			m_tier1Effects = new List<SkillEffectStaticData>();
			m_tier2Effects = new List<SkillEffectStaticData>();
			m_tier3Effects = new List<SkillEffectStaticData>();
			m_tier4Effects = new List<SkillEffectStaticData>();
			m_currentlyAvailableEffects = new List<SkillEffectStaticData>();
			m_availableScalingEffects = new List<SkillEffectStaticData>();
			m_tier1ScalingEffects = new List<SkillEffectStaticData>();
			m_requiredSkillLevelNovice = ConfigManager.Instance.Game.RequiredSkillLevelNovice;
			m_requiredSkillLevelExpert = ConfigManager.Instance.Game.RequiredSkillLevelExpert;
			m_requiredSkillLevelMaster = ConfigManager.Instance.Game.RequiredSkillLevelMaster;
			m_requiredSkillLevelGrandMaster = ConfigManager.Instance.Game.RequiredSkillLevelGrandMaster;
			m_maxTier = p_maxTier;
			m_staticData = StaticDataHandler.GetStaticData<SkillStaticData>(EDataType.SKILL, p_skillStaticID);
			if (m_maxTier >= ETier.NOVICE)
			{
				InitSkillEffectData(m_staticData.Tier1Effects, m_tier1Effects);
			}
			if (m_maxTier >= ETier.EXPERT)
			{
				InitSkillEffectData(m_staticData.Tier2Effects, m_tier2Effects);
			}
			if (m_maxTier >= ETier.MASTER)
			{
				InitSkillEffectData(m_staticData.Tier3Effects, m_tier3Effects);
			}
			if (m_maxTier >= ETier.GRAND_MASTER)
			{
				InitSkillEffectData(m_staticData.Tier4Effects, m_tier4Effects);
			}
		}

		public Int32 StaticID => m_staticData.StaticID;

	    public String Name => m_staticData.Name;

	    public String Description => m_staticData.Description;

	    public String Icon => m_staticData.Icon;

	    public ESkillCategory Category => m_staticData.Category;

	    public List<SkillEffectStaticData> Tier1Effects => m_tier1Effects;

	    public List<SkillEffectStaticData> Tier2Effects => m_tier2Effects;

	    public List<SkillEffectStaticData> Tier3Effects => m_tier3Effects;

	    public List<SkillEffectStaticData> Tier4Effects => m_tier4Effects;

	    public List<SkillEffectStaticData> CurrentlyAvailableEffects => m_currentlyAvailableEffects;

	    public List<SkillEffectStaticData> AvailableScalingEffects => m_availableScalingEffects;

	    public List<SkillEffectStaticData> Tier1ScalingEffects => m_tier1ScalingEffects;

	    public Int32 Level
		{
			get => m_level;
	        set
			{
				if (value > MaxLevel)
				{
					throw new ArgumentOutOfRangeException("value", value, String.Concat(new Object[]
					{
						"value < ",
						MinLevel,
						" || value > ",
						MaxLevel
					}));
				}
				m_level = value;
				if (value == 1)
				{
					Tier = ETier.NOVICE;
				}
				else if (value == 0)
				{
					Tier = ETier.NONE;
				}
			}
		}

		public Int32 TemporaryLevel
		{
			get => m_temporaryLevel;
		    set => m_temporaryLevel = value;
		}

		public Int32 VirtualSkillLevel
		{
			get => m_virtualSkillLevel;
		    set => m_virtualSkillLevel = value;
		}

		public Int32 MinLevel => (m_tier != ETier.NONE) ? GetMaxLevelByTier(m_tier) : 0;

	    public Int32 MaxLevel => GetMaxLevelByTier((m_tier + 1 <= ETier.GRAND_MASTER) ? (m_tier + 1) : ETier.GRAND_MASTER);

	    public Int32 TotalMaxLevel => GetMaxLevelByTier(m_maxTier);

	    public ETier Tier
		{
			get => m_tier;
	        set
			{
				if (value < ETier.NONE || value > m_maxTier)
				{
					throw new ArgumentOutOfRangeException("value", value, String.Concat(new Object[]
					{
						"value < ",
						ETier.NONE,
						" || value > ",
						m_maxTier
					}));
				}
				if (m_tier != value)
				{
					m_tier = value;
					Int32 maxLevel = MaxLevel;
					Int32 minLevel = MinLevel;
					if (m_level > maxLevel)
					{
						m_level = maxLevel;
					}
					if (m_level < minLevel)
					{
						m_level = minLevel;
					}
					if (m_tier > m_maxUnlockedTier)
					{
						m_maxUnlockedTier = m_tier;
					}
					UpdateSkillEffects();
				}
			}
		}

		public ETier MaxTier => m_maxTier;

	    public Boolean IsTierMaxLevel => Level == MaxLevel;

	    public static Int32 GetMaxLevelByTier(ETier p_tier)
		{
			switch (p_tier)
			{
			case ETier.NONE:
				return 1;
			case ETier.NOVICE:
				return ConfigManager.Instance.Game.RequiredSkillLevelNovice;
			case ETier.EXPERT:
				return ConfigManager.Instance.Game.RequiredSkillLevelExpert;
			case ETier.MASTER:
				return ConfigManager.Instance.Game.RequiredSkillLevelMaster;
			case ETier.GRAND_MASTER:
				return ConfigManager.Instance.Game.RequiredSkillLevelGrandMaster;
			default:
				return 0;
			}
		}

		public static ETier GetTierByLevel(Int32 p_level)
		{
			if (p_level >= ConfigManager.Instance.Game.RequiredSkillLevelGrandMaster)
			{
				return ETier.GRAND_MASTER;
			}
			if (p_level >= ConfigManager.Instance.Game.RequiredSkillLevelMaster)
			{
				return ETier.MASTER;
			}
			if (p_level >= ConfigManager.Instance.Game.RequiredSkillLevelExpert)
			{
				return ETier.EXPERT;
			}
			if (p_level >= 1)
			{
				return ETier.NOVICE;
			}
			return ETier.NONE;
		}

		private void UpdateSkillEffects()
		{
			m_currentlyAvailableEffects.Clear();
			m_availableScalingEffects.Clear();
			if (m_tier == ETier.NONE)
			{
				return;
			}
			if (m_tier >= ETier.NOVICE)
			{
				m_currentlyAvailableEffects.AddRange(m_tier1Effects);
			}
			if (m_tier >= ETier.EXPERT)
			{
				m_currentlyAvailableEffects.AddRange(m_tier2Effects);
			}
			if (m_tier >= ETier.MASTER)
			{
				m_currentlyAvailableEffects.AddRange(m_tier3Effects);
			}
			if (m_tier >= ETier.GRAND_MASTER)
			{
				m_currentlyAvailableEffects.AddRange(m_tier4Effects);
			}
			foreach (SkillEffectStaticData skillEffectStaticData in m_currentlyAvailableEffects)
			{
				if (skillEffectStaticData.Mode == ESkillEffectMode.PER_SKILL_LEVEL)
				{
					m_availableScalingEffects.Add(skillEffectStaticData);
				}
			}
		}

		private void InitSkillEffectData(Int32[] p_tierEffectIDs, List<SkillEffectStaticData> p_tierEffects)
		{
			for (Int32 i = 0; i < p_tierEffectIDs.Length; i++)
			{
				SkillEffectStaticData staticData = StaticDataHandler.GetStaticData<SkillEffectStaticData>(EDataType.SKILL_EFFECT, p_tierEffectIDs[i]);
				p_tierEffects.Add(staticData);
				if (p_tierEffects == m_tier1Effects && staticData.Mode == ESkillEffectMode.PER_SKILL_LEVEL)
				{
					m_tier1ScalingEffects.Add(staticData);
				}
			}
		}

		internal Int32 GetRequiredSkillLevel(ETier p_tier)
		{
			switch (p_tier)
			{
			case ETier.NONE:
				return 0;
			case ETier.NOVICE:
				return m_requiredSkillLevelNovice;
			case ETier.EXPERT:
				return m_requiredSkillLevelExpert;
			case ETier.MASTER:
				return m_requiredSkillLevelMaster;
			case ETier.GRAND_MASTER:
				return m_requiredSkillLevelGrandMaster;
			default:
				return 0;
			}
		}

		public void Load(SaveGameData p_data)
		{
			m_maxUnlockedTier = (ETier)p_data.Get<Int32>("Tier", 0);
			Tier = (ETier)p_data.Get<Int32>("Tier", 0);
			Level = p_data.Get<Int32>("Level", 0);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("Level", m_level);
			p_data.Set<Int32>("Tier", (Int32)m_tier);
			p_data.Set<Int32>("MaxUnlockedTier", (Int32)m_maxUnlockedTier);
		}
	}
}
