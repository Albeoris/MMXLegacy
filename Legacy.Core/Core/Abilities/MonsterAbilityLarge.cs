using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityLarge : MonsterAbilityBase
	{
		public MonsterAbilityLarge() : base(EMonsterAbilityType.LARGE)
		{
			m_executionPhase = EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_BEFORE_DAMAGE_REDUCTION;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			if (p_attackList == null)
			{
				return;
			}
			BaseItem itemAt = p_character.Equipment.GetItemAt(EEquipSlots.MAIN_HAND);
			if (itemAt is MeleeWeapon)
			{
				MeleeWeapon meleeWeapon = (MeleeWeapon)itemAt;
				if (meleeWeapon.GetWeaponType() == EEquipmentType.SPEAR)
				{
					p_monster.CombatHandler.EvadeValue = (Int32)(p_monster.CombatHandler.EvadeValue * (m_staticData.GetValues(m_level)[0] * 0.01f));
					for (Int32 i = 0; i < p_attackList.Count; i++)
					{
						for (Int32 j = 0; j < p_attackList[i].Damages.Count; j++)
						{
							if (p_attackList[i].Damages[j].Type == EDamageType.PHYSICAL)
							{
								Damage value = new Damage(EDamageType.PHYSICAL, p_attackList[i].Damages[j].Value, p_attackList[i].Damages[j].CriticalBonusMod, p_attackList[i].Damages[j].percentage);
								value.Value = (Int32)(value.Value * (1f + m_staticData.GetValues(m_level)[1] * 0.01f));
								p_attackList[i].Damages[j] = value;
							}
						}
					}
				}
			}
		}

		public override String GetDescription()
		{
			return Localization.Instance.GetText("MONSTER_ABILITY_LARGE_INFO", m_staticData.GetValues(m_level)[0], m_staticData.GetValues(m_level)[1]);
		}
	}
}
