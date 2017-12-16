using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Skills;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public class Shield : Equipment, ISkillDependant, IDescribable
	{
		private ShieldStaticData m_staticData;

		private Dictionary<String, String> m_properties;

		protected override BaseItemStaticData BaseData => m_staticData;

	    protected override EquipmentStaticData BaseEquipmentData => m_staticData;

	    public override EItemSlot ItemSlot => EItemSlot.ITEM_SLOT_OFFHAND;

	    public Int32 ArmorValue => (Int32)Math.Round(m_staticData.AC * BrokenMultiplier, MidpointRounding.AwayFromZero);

	    private Single BrokenMultiplier
		{
			get
			{
				if (!m_broken)
				{
					return 1f;
				}
				if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.NORMAL)
				{
					return ConfigManager.Instance.Game.BrokenItemMalusNormal;
				}
				return ConfigManager.Instance.Game.BrokenItemMalusHard;
			}
		}

		public override void Init(Int32 p_staticID)
		{
			m_staticData = StaticDataHandler.GetStaticData<ShieldStaticData>(EDataType.SHIELD, p_staticID);
			m_identified = m_staticData.Identified;
			InitProperties();
			InitPrefixes();
			InitSuffixes();
		}

		public override void InitFromModel(Int32 p_staticId, Int32 p_prefixId, Int32 p_suffixId)
		{
			EquipmentStaticData staticData = StaticDataHandler.GetStaticData<ShieldStaticData>(EDataType.SHIELD_MODEL, p_staticId);
			InitFromModel(staticData, p_prefixId, p_suffixId);
		}

		public override void InitFromModel(EquipmentStaticData p_staticData, Int32 p_prefixId, Int32 p_suffixId)
		{
			base.InitFromModel(p_staticData, p_prefixId, p_suffixId);
			m_staticData = (ShieldStaticData)p_staticData;
			InitProperties();
			InitPrefix(p_prefixId);
			InitSuffix(p_suffixId);
		}

		private void InitProperties()
		{
			m_properties = new Dictionary<String, String>();
			m_properties["SHIELD_EFFECT_AC"] = ArmorValue.ToString();
		}

		public override EDataType GetItemType()
		{
			return EDataType.SHIELD;
		}

		public override void FillFightValues(Boolean p_offHand, FightValues p_fightValues)
		{
			p_fightValues.ArmorValue += ArmorValue;
			m_properties["SHIELD_EFFECT_AC"] = ArmorValue.ToString();
		}

		public override void ModifyProperties()
		{
			m_properties["SHIELD_EFFECT_AC"] = ArmorValue.ToString();
		}

		public Boolean BreakCheck()
		{
			if (!m_broken)
			{
				if (Random.Value < m_staticData.BreakChance)
				{
					m_broken = true;
				}
				return m_broken;
			}
			return false;
		}

		public Int32 GetRequiredSkillID()
		{
			return m_staticData.RequiredSkillID;
		}

		public ETier GetRequiredSkillTier()
		{
			return m_staticData.RequiredSkillTier;
		}

		public String GetTypeDescription()
		{
			return m_staticData.Type.ToString();
		}

		public Dictionary<String, String> GetPropertiesDescription()
		{
			return m_properties;
		}
	}
}
