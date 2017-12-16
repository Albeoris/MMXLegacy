using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Skills;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public class RangedWeapon : Equipment, ISkillDependant, IDescribable
	{
		private RangedWeaponStaticData m_staticData;

		private Dictionary<String, String> m_properties;

		protected override BaseItemStaticData BaseData => m_staticData;

	    protected override EquipmentStaticData BaseEquipmentData => m_staticData;

	    public override EItemSlot ItemSlot => EItemSlot.ITEM_SLOT_RANGED;

	    public Int32 MinDamage => m_staticData.MinDamage;

	    public Int32 MaxDamage => m_staticData.MaxDamage;

	    public Int32 CriticalDamage => (Int32)Math.Round(100f * m_staticData.CritDamageFactor, MidpointRounding.AwayFromZero);

	    public override void Init(Int32 p_staticID)
		{
			m_staticData = StaticDataHandler.GetStaticData<RangedWeaponStaticData>(EDataType.RANGED_WEAPON, p_staticID);
			m_identified = m_staticData.Identified;
			InitProperties();
			InitPrefixes();
			InitSuffixes();
		}

		public override void InitFromModel(Int32 p_staticId, Int32 p_prefixId, Int32 p_suffixId)
		{
			EquipmentStaticData staticData = StaticDataHandler.GetStaticData<RangedWeaponStaticData>(EDataType.RANGED_WEAPON_MODEL, p_staticId);
			InitFromModel(staticData, p_prefixId, p_suffixId);
		}

		public override void InitFromModel(EquipmentStaticData p_staticData, Int32 p_prefixId, Int32 p_suffixId)
		{
			base.InitFromModel(p_staticData, p_prefixId, p_suffixId);
			m_staticData = (RangedWeaponStaticData)p_staticData;
			InitProperties();
			InitPrefix(p_prefixId);
			InitSuffix(p_suffixId);
		}

		private void InitProperties()
		{
			m_properties = new Dictionary<String, String>();
			Int32 minDamage = MinDamage;
			Int32 maxDamage = MaxDamage;
			String value = (minDamage != maxDamage) ? (minDamage.ToString() + "-" + maxDamage.ToString()) : maxDamage.ToString();
			m_properties["RANGED_EFFECT_DAMAGE"] = value;
			m_properties["RANGED_EFFECT_CRITICAL_DAMAGE"] = CriticalDamage.ToString() + "%";
		}

		public override EDataType GetItemType()
		{
			return EDataType.RANGED_WEAPON;
		}

		public EEquipmentType GetWeaponType()
		{
			return m_staticData.Type;
		}

		public override void FillFightValues(Boolean p_offHand, FightValues p_fightValue)
		{
			p_fightValue.RangeDamage.Add(new DamageData(EDamageType.PHYSICAL, m_staticData.MinDamage, m_staticData.MaxDamage));
			p_fightValue.RangeCriticalDamageMod += m_staticData.CritDamageFactor;
		}

		public override void ModifyProperties()
		{
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
