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
	public class Armor : Equipment, ISkillDependant, IDescribable
	{
		private ArmorStaticData m_staticData;

		private Dictionary<String, String> m_properties;

		public EEquipmentType ArmorType => m_staticData.Type;

	    public EEquipmentType GetSubType()
		{
			return m_staticData.Subtype;
		}

		protected override BaseItemStaticData BaseData => m_staticData;

	    protected override EquipmentStaticData BaseEquipmentData => m_staticData;

	    public override EItemSlot ItemSlot
		{
			get
			{
				switch (m_staticData.Type)
				{
				case EEquipmentType.GARMENT:
					return EItemSlot.ITEM_SLOT_TORSO;
				case EEquipmentType.GLOVE:
					return EItemSlot.ITEM_SLOT_HAND;
				case EEquipmentType.BOOTS:
					return EItemSlot.ITEM_SLOT_FEET;
				default:
					return EItemSlot.ITEM_SLOT_HEAD;
				}
			}
		}

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
			m_staticData = StaticDataHandler.GetStaticData<ArmorStaticData>(EDataType.ARMOR, p_staticID);
			m_identified = m_staticData.Identified;
			InitProperties();
			InitPrefixes();
			InitSuffixes();
		}

		public override void InitFromModel(Int32 p_staticId, Int32 p_prefixId, Int32 p_suffixId)
		{
			EquipmentStaticData staticData = StaticDataHandler.GetStaticData<ArmorStaticData>(EDataType.ARMOR_MODEL, p_staticId);
			InitFromModel(staticData, p_prefixId, p_suffixId);
		}

		public override void InitFromModel(EquipmentStaticData p_staticData, Int32 p_prefixId, Int32 p_suffixId)
		{
			base.InitFromModel(p_staticData, p_prefixId, p_suffixId);
			m_staticData = (ArmorStaticData)p_staticData;
			InitProperties();
			InitPrefix(p_prefixId);
			InitSuffix(p_suffixId);
		}

		private void InitProperties()
		{
			m_properties = new Dictionary<String, String>();
			ModifyProperties();
		}

		public override EDataType GetItemType()
		{
			return EDataType.ARMOR;
		}

		public override void FillFightValues(Boolean p_offHand, FightValues p_fightValues)
		{
			p_fightValues.ArmorValue += ArmorValue;
			ModifyProperties();
		}

		public override void ModifyProperties()
		{
			if (m_staticData.AC > 0)
			{
				m_properties["ARMOR_EFFECT_AC"] = ArmorValue.ToString();
			}
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
			if (m_staticData.Subtype == EEquipmentType.CLOTHING)
			{
				return 0;
			}
			return m_staticData.RequiredSkillID;
		}

		public ETier GetRequiredSkillTier()
		{
			return m_staticData.RequiredSkillTier;
		}

		public String GetTypeDescription()
		{
			if (m_staticData.Subtype == EEquipmentType.LIGHT_ARMOR || m_staticData.Subtype == EEquipmentType.HEAVY_ARMOR)
			{
				return m_staticData.Subtype.ToString();
			}
			return m_staticData.Type.ToString();
		}

		public Dictionary<String, String> GetPropertiesDescription()
		{
			return m_properties;
		}
	}
}
