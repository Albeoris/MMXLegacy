using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public class MeleeWeapon : Equipment, ISkillDependant, IDescribable
	{
		private MeleeWeaponStaticData m_staticData;

		private Dictionary<String, String> m_properties;

		protected override BaseItemStaticData BaseData => m_staticData;

	    protected override EquipmentStaticData BaseEquipmentData => m_staticData;

	    public override EItemSlot ItemSlot
		{
			get
			{
				EEquipmentType subtype = m_staticData.Subtype;
				if (subtype != EEquipmentType.TWOHANDED)
				{
					return EItemSlot.ITEM_SLOT_1_HAND;
				}
				return EItemSlot.ITEM_SLOT_2_HAND;
			}
		}

		public Int32 MinDamage => (Int32)Math.Round(m_staticData.MinDamage * BrokenMultiplier, MidpointRounding.AwayFromZero);

	    public Int32 MaxDamage => (Int32)Math.Round(m_staticData.MaxDamage * BrokenMultiplier, MidpointRounding.AwayFromZero);

	    public Int32 CriticalDamage => (Int32)Math.Round(100f * m_staticData.CritDamageFactor, MidpointRounding.AwayFromZero);

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
			m_staticData = StaticDataHandler.GetStaticData<MeleeWeaponStaticData>(EDataType.MELEE_WEAPON, p_staticID);
			m_identified = m_staticData.Identified;
			InitProperties();
			InitPrefixes();
			InitSuffixes();
		}

		public override void InitFromModel(Int32 p_staticId, Int32 p_prefixId, Int32 p_suffixId)
		{
			EquipmentStaticData staticData = StaticDataHandler.GetStaticData<MeleeWeaponStaticData>(EDataType.MELEE_WEAPON_MODEL, p_staticId);
			InitFromModel(staticData, p_prefixId, p_suffixId);
		}

		public override void InitFromModel(EquipmentStaticData p_staticData, Int32 p_prefixId, Int32 p_suffixId)
		{
			base.InitFromModel(p_staticData, p_prefixId, p_suffixId);
			m_staticData = (MeleeWeaponStaticData)p_staticData;
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
			m_properties["MELEE_EFFECT_DAMAGE"] = value;
			m_properties["MELEE_EFFECT_CRITICAL_DAMAGE"] = CriticalDamage.ToString() + "%";
		}

		public override EDataType GetItemType()
		{
			return EDataType.MELEE_WEAPON;
		}

		public EEquipmentType GetSubType()
		{
			return m_staticData.Subtype;
		}

		public EEquipmentType GetWeaponType()
		{
			return m_staticData.Type;
		}

		public override void FillFightValues(Boolean p_offHand, FightValues p_fightValue)
		{
			Int32 minDamage = MinDamage;
			Int32 maxDamage = MaxDamage;
			String value = (minDamage != maxDamage) ? (minDamage.ToString() + "-" + maxDamage.ToString()) : maxDamage.ToString();
			m_properties["MELEE_EFFECT_DAMAGE"] = value;
			DamageData p_data = new DamageData(EDamageType.PHYSICAL, minDamage, maxDamage);
			if (p_offHand)
			{
				p_fightValue.OffHandDamage.Add(p_data);
				p_fightValue.OffHandCriticalDamageMod = m_staticData.CritDamageFactor;
			}
			else
			{
				p_fightValue.MainHandDamage.Add(p_data);
				p_fightValue.MainHandCriticalDamageMod = m_staticData.CritDamageFactor;
			}
		}

		public override void ModifyProperties()
		{
			Int32 minDamage = MinDamage;
			Int32 maxDamage = MaxDamage;
			String value = (minDamage != maxDamage) ? (minDamage.ToString() + "-" + maxDamage.ToString()) : maxDamage.ToString();
			m_properties["MELEE_EFFECT_DAMAGE"] = value;
		}

		public Boolean BreakCheck()
		{
			if (!m_broken)
			{
				if (Random.Value < m_staticData.BreakChance)
				{
					m_broken = true;
				}
				if (m_broken)
				{
					Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
					selectedCharacter.BarkHandler.TriggerBark(EBarks.ATTACK_CRIT_BREAKING, selectedCharacter);
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
			String text = String.Empty;
			if (m_staticData.Type == EEquipmentType.MACE)
			{
				text = "MELEE_WEAPON_TYPE_MACE";
			}
			else if (m_staticData.Type == EEquipmentType.SWORD)
			{
				text = "MELEE_WEAPON_TYPE_SWORD";
			}
			else if (m_staticData.Type == EEquipmentType.AXE)
			{
				text = "MELEE_WEAPON_TYPE_AXE";
			}
			else if (m_staticData.Type == EEquipmentType.DAGGER)
			{
				text = "MELEE_WEAPON_TYPE_DAGGER";
			}
			else if (m_staticData.Type == EEquipmentType.SPEAR)
			{
				text = "MELEE_WEAPON_TYPE_SPEAR";
			}
			if (m_staticData.Subtype == EEquipmentType.TWOHANDED && m_staticData.Type != EEquipmentType.SPEAR)
			{
				text += "_GREAT";
			}
			return text;
		}

		public Dictionary<String, String> GetPropertiesDescription()
		{
			return m_properties;
		}
	}
}
