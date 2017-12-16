using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Internationalization;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public abstract class Equipment : BaseItem
	{
		private const Single EQUIPMENT_IDENTIFIED_CHANCE_ONE_ENCHANTMENT = 0.5f;

		protected Boolean m_identified = true;

		protected Boolean m_broken;

		protected Int32 m_currentXP;

		protected String m_name = String.Empty;

		protected Boolean m_generated;

		protected Int32 m_prefixLevel = 1;

		protected Int32 m_suffixLevel = 1;

		protected Boolean m_isTracked;

		protected List<PrefixStaticData> m_prefixes;

		protected List<SuffixStaticData> m_suffixes;

		public Int32 PrefixLevel
		{
			get => m_prefixLevel;
		    set => m_prefixLevel = value;
		}

		public Int32 SuffixLevel
		{
			get => m_suffixLevel;
		    set => m_suffixLevel = value;
		}

		public Boolean IsTracked
		{
			get => m_isTracked;
		    set => m_isTracked = value;
		}

		protected abstract EquipmentStaticData BaseEquipmentData { get; }

		public abstract void InitFromModel(Int32 p_staticId, Int32 p_prefixId, Int32 p_suffixId);

		public virtual void InitFromModel(EquipmentStaticData p_staticData, Int32 p_prefixId, Int32 p_suffixId)
		{
			m_generated = true;
		}

		public override Int32 Price
		{
			get
			{
				if (m_broken || !m_identified)
				{
					return ConfigManager.Instance.Game.ItemPriceBrokenOrUnidentified;
				}
				Int32 num = BaseData.Price;
				for (Int32 i = 0; i < m_prefixes.Count; i++)
				{
					num += m_prefixes[i].GetPriceForLevel(PrefixLevel);
				}
				for (Int32 j = 0; j < m_suffixes.Count; j++)
				{
					num += m_suffixes[j].GetPriceForLevel(SuffixLevel, ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
				}
				if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
				{
					num = (Int32)(num * ConfigManager.Instance.Game.ItemEquipmentFactor);
				}
				return (Int32)Math.Ceiling(num * m_priceMultiplicator);
			}
		}

		public override String Name
		{
			get
			{
				if (m_name == String.Empty)
				{
					InitName();
				}
				return m_name;
			}
		}

		public Boolean Identified
		{
			get => m_identified;
		    set
			{
				m_identified = value;
				InitName();
			}
		}

		public Boolean Broken
		{
			get => m_broken;
		    set => m_broken = value;
		}

		public Int32 CurrentXP
		{
			get => m_currentXP;
		    set => m_currentXP = value;
		}

		public Int32 RequiredXP => BaseEquipmentData.RequiredXP;

	    public abstract EItemSlot ItemSlot { get; }

		public Int32 ModelLevel => BaseEquipmentData.ModelLevel;

	    public Int32 RelicLevel => BaseEquipmentData.RelicLevel;

	    public String Description => BaseEquipmentData.Description;

	    public Int32 NextLevelItemId => BaseEquipmentData.NextLevelItemID;

	    public List<PrefixStaticData> Prefixes => m_prefixes;

	    public List<SuffixStaticData> Suffixes => m_suffixes;

	    protected virtual void InitName()
		{
			if (IsRelic())
			{
				if (Identified)
				{
					m_name = Localization.Instance.GetText(BaseData.NameKey);
				}
				else
				{
					m_name = Localization.Instance.GetText("EQUIPMENT_UNIDENTIFIED_RELIC");
				}
			}
			else
			{
				m_name = Localization.Instance.GetText(BaseData.NameKey);
				if (!Identified)
				{
					m_name = Localization.Instance.GetText("EQUIPMENT_UNIDENTIFIED", m_name);
					m_name = m_name.Replace("  ", " ");
				}
				StringBuilder stringBuilder = new StringBuilder(m_name);
				if (m_prefixes.Count > 0 && Identified)
				{
					stringBuilder.Replace("{PMS}", Localization.Instance.GetText(m_prefixes[0].Name + "_MS"));
					stringBuilder.Replace("{PMP}", Localization.Instance.GetText(m_prefixes[0].Name + "_MP"));
					stringBuilder.Replace("{PFS}", Localization.Instance.GetText(m_prefixes[0].Name + "_FS"));
					stringBuilder.Replace("{PFP}", Localization.Instance.GetText(m_prefixes[0].Name + "_FP"));
					stringBuilder.Replace("{PNS}", Localization.Instance.GetText(m_prefixes[0].Name + "_NS"));
					stringBuilder.Replace("{PNP}", Localization.Instance.GetText(m_prefixes[0].Name + "_NP"));
				}
				else
				{
					stringBuilder.Replace("{PMS}", String.Empty);
					stringBuilder.Replace("{PMP}", String.Empty);
					stringBuilder.Replace("{PFS}", String.Empty);
					stringBuilder.Replace("{PFP}", String.Empty);
					stringBuilder.Replace("{PNS}", String.Empty);
					stringBuilder.Replace("{PNP}", String.Empty);
				}
				if (m_suffixes.Count > 0 && Identified)
				{
					stringBuilder.Replace("{SMS}", Localization.Instance.GetText(m_suffixes[0].Name + "_MS"));
					stringBuilder.Replace("{SMP}", Localization.Instance.GetText(m_suffixes[0].Name + "_MP"));
					stringBuilder.Replace("{SFS}", Localization.Instance.GetText(m_suffixes[0].Name + "_FS"));
					stringBuilder.Replace("{SFP}", Localization.Instance.GetText(m_suffixes[0].Name + "_FP"));
					stringBuilder.Replace("{SNS}", Localization.Instance.GetText(m_suffixes[0].Name + "_NS"));
					stringBuilder.Replace("{SNP}", Localization.Instance.GetText(m_suffixes[0].Name + "_NP"));
				}
				else
				{
					stringBuilder.Replace("{SMS}", String.Empty);
					stringBuilder.Replace("{SMP}", String.Empty);
					stringBuilder.Replace("{SFS}", String.Empty);
					stringBuilder.Replace("{SFP}", String.Empty);
					stringBuilder.Replace("{SNS}", String.Empty);
					stringBuilder.Replace("{SNP}", String.Empty);
				}
				m_name = stringBuilder.ToString().Trim();
			}
		}

		public virtual void InitIdentification()
		{
			if (m_prefixes.Count > 0 && m_suffixes.Count > 0)
			{
				Identified = false;
			}
			else if (m_prefixes.Count > 0 || m_suffixes.Count > 0)
			{
				Identified = (Random.Value < 0.5f);
			}
		}

		protected void InitPrefixes()
		{
			m_prefixLevel = BaseEquipmentData.PrefixLevel;
			m_prefixes = new List<PrefixStaticData>();
			Int32[] prefix = BaseEquipmentData.Prefix;
			Int32 num = prefix.Length;
			for (Int32 i = 0; i < num; i++)
			{
				Int32 num2 = prefix[i];
				if (num2 > 0)
				{
					PrefixStaticData staticData = StaticDataHandler.GetStaticData<PrefixStaticData>(EDataType.PREFIX, num2);
					if (staticData != null)
					{
						m_prefixes.Add(staticData);
					}
				}
			}
		}

		protected void InitSuffixes()
		{
			m_suffixLevel = BaseEquipmentData.SuffixLevel;
			m_suffixes = new List<SuffixStaticData>();
			Int32[] suffix = BaseEquipmentData.Suffix;
			Int32 num = suffix.Length;
			for (Int32 i = 0; i < num; i++)
			{
				Int32 num2 = suffix[i];
				if (num2 > 0)
				{
					SuffixStaticData staticData = StaticDataHandler.GetStaticData<SuffixStaticData>(EDataType.SUFFIX, num2);
					if (staticData != null)
					{
						m_suffixes.Add(staticData);
					}
				}
			}
		}

		protected void InitPrefix(Int32 p_prefixId)
		{
			m_prefixes = new List<PrefixStaticData>();
			if (p_prefixId > 0)
			{
				PrefixStaticData staticData = StaticDataHandler.GetStaticData<PrefixStaticData>(EDataType.PREFIX, p_prefixId);
				if (staticData != null)
				{
					m_prefixes.Add(staticData);
				}
			}
		}

		protected void InitSuffix(Int32 p_suffixId)
		{
			m_suffixes = new List<SuffixStaticData>();
			if (p_suffixId > 0)
			{
				SuffixStaticData staticData = StaticDataHandler.GetStaticData<SuffixStaticData>(EDataType.SUFFIX, p_suffixId);
				if (staticData != null)
				{
					m_suffixes.Add(staticData);
				}
			}
		}

		public Boolean IsRelic()
		{
			return BaseEquipmentData.RelicLevel >= 1;
		}

		public void AddExp(Int32 p_exp)
		{
			if (BaseEquipmentData.NextLevelItemID > 0)
			{
				m_currentXP += p_exp;
				if (m_currentXP > BaseEquipmentData.RequiredXP)
				{
					m_currentXP = BaseEquipmentData.RequiredXP;
				}
			}
		}

		public Boolean LevelUpConditionsMet()
		{
			return BaseEquipmentData.NextLevelItemID > 0 && m_currentXP >= BaseEquipmentData.RequiredXP;
		}

		public abstract void FillFightValues(Boolean p_offHand, FightValues p_fightValue);

		public abstract void ModifyProperties();

		public override void Load(SaveGameData p_data)
		{
			m_generated = p_data.Get<Boolean>("IsGenerated", false);
			if (m_generated)
			{
				Int32 p_staticId = p_data.Get<Int32>("StaticID", 0);
				Int32 p_prefixId = p_data.Get<Int32>("PrefixId", -1);
				Int32 p_suffixId = p_data.Get<Int32>("SuffixId", -1);
				InitFromModel(p_staticId, p_prefixId, p_suffixId);
				m_priceMultiplicator = p_data.Get<Single>("PriceMultiplicator", 1f);
			}
			else
			{
				base.Load(p_data);
			}
			m_isTracked = p_data.Get<Boolean>("IsTracked", false);
			m_prefixLevel = p_data.Get<Int32>("PrefixLevel", BaseEquipmentData.PrefixLevel);
			m_suffixLevel = p_data.Get<Int32>("SuffixLevel", BaseEquipmentData.SuffixLevel);
			m_identified = p_data.Get<Boolean>("Identified", true);
			m_broken = p_data.Get<Boolean>("Broken", false);
			m_currentXP = p_data.Get<Int32>("CurrentXP", 0);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			if (m_generated)
			{
				if (m_prefixes.Count > 0)
				{
					p_data.Set<Int32>("PrefixId", m_prefixes[0].StaticID);
				}
				if (m_suffixes.Count > 0)
				{
					p_data.Set<Int32>("SuffixId", m_suffixes[0].StaticID);
				}
			}
			p_data.Set<Boolean>("IsTracked", m_isTracked);
			p_data.Set<Int32>("PrefixLevel", m_prefixLevel);
			p_data.Set<Int32>("SuffixLevel", m_suffixLevel);
			p_data.Set<Boolean>("IsGenerated", m_generated);
			p_data.Set<Boolean>("Identified", m_identified);
			p_data.Set<Boolean>("Broken", m_broken);
			p_data.Set<Int32>("CurrentXP", m_currentXP);
		}
	}
}
