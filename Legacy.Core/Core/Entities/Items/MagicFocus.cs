using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Skills;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public class MagicFocus : Equipment, ISkillDependant, IDescribable
	{
		private MagicFocusStaticData m_staticData;

		private Dictionary<String, String> m_properties;

		protected override BaseItemStaticData BaseData => m_staticData;

	    protected override EquipmentStaticData BaseEquipmentData => m_staticData;

	    public override EItemSlot ItemSlot
		{
			get
			{
				EEquipmentType type = m_staticData.Type;
				if (type != EEquipmentType.MAGIC_FOCUS_TWOHANDED)
				{
					return EItemSlot.ITEM_SLOT_1_HAND;
				}
				return EItemSlot.ITEM_SLOT_2_HAND;
			}
		}

		public Int32 MinDamage => m_staticData.MinDamage;

	    public Int32 MaxDamage => m_staticData.MaxDamage;

	    public Int32 MagicalCriticalDamage => (Int32)Math.Round(100f * m_staticData.AddCritDamage, MidpointRounding.AwayFromZero);

	    public EEquipmentType GetMagicfocusType()
		{
			return m_staticData.Type;
		}

		public override void Init(Int32 p_staticID)
		{
			m_staticData = StaticDataHandler.GetStaticData<MagicFocusStaticData>(EDataType.MAGIC_FOCUS, p_staticID);
			m_identified = m_staticData.Identified;
			InitProperties();
			InitPrefixes();
			InitSuffixes();
		}

		public override void InitFromModel(Int32 p_staticId, Int32 p_prefixId, Int32 p_suffixId)
		{
			EquipmentStaticData staticData = StaticDataHandler.GetStaticData<MagicFocusStaticData>(EDataType.MAGIC_FOCUS_MODEL, p_staticId);
			InitFromModel(staticData, p_prefixId, p_suffixId);
		}

		public override void InitFromModel(EquipmentStaticData p_staticData, Int32 p_prefixId, Int32 p_suffixId)
		{
			base.InitFromModel(p_staticData, p_prefixId, p_suffixId);
			m_staticData = (MagicFocusStaticData)p_staticData;
			InitProperties();
			InitPrefix(p_prefixId);
			InitSuffix(p_suffixId);
		}

		private void InitProperties()
		{
			m_properties = new Dictionary<String, String>();
			DamageData baseDamage = GetBaseDamage();
			String value = (baseDamage.Minimum != baseDamage.Maximum) ? (baseDamage.Minimum.ToString() + "-" + baseDamage.Maximum.ToString()) : baseDamage.Maximum.ToString();
			m_properties["MELEE_EFFECT_DAMAGE"] = value;
			m_properties["MAGIC_FOCUS_EFFECT_INCREASE_CRITICAL_DAMAGE"] = MagicalCriticalDamage.ToString() + "%";
		}

		public override EDataType GetItemType()
		{
			return EDataType.MAGIC_FOCUS;
		}

		public override void FillFightValues(Boolean p_offHand, FightValues p_fightValue)
		{
			DamageData baseDamage = GetBaseDamage();
			if (p_offHand)
			{
				p_fightValue.OffHandDamage.Add(baseDamage);
				p_fightValue.OffHandCriticalDamageMod = 0f;
			}
			else
			{
				p_fightValue.MainHandDamage.Add(baseDamage);
				p_fightValue.MainHandCriticalDamageMod = 0f;
			}
			p_fightValue.MagicalCriticalDamageMod += m_staticData.AddCritDamage;
		}

		public override void ModifyProperties()
		{
		}

		public DamageData GetBaseDamage()
		{
			return new DamageData(EDamageType.PRIMORDIAL, MinDamage, MaxDamage);
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
