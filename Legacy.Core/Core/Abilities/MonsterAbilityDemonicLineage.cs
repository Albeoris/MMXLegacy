using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityDemonicLineage : MonsterAbilityBase
	{
		public MonsterAbilityDemonicLineage() : base(EMonsterAbilityType.DEMONIC_LINEAGE)
		{
			m_executionPhase = (EExecutionPhase.BEFORE_TARGET_SELECTION | EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_BEFORE_DAMAGE_REDUCTION);
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			if (p_attackList == null)
			{
				return;
			}
			if (m_currentExecutionPhase == EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_BEFORE_DAMAGE_REDUCTION)
			{
				foreach (Attack attack in p_attackList)
				{
					List<Damage> damages = attack.Damages;
					for (Int32 i = 0; i < damages.Count; i++)
					{
						if (damages[i].Type == EDamageType.LIGHT)
						{
							Damage value = damages[i];
							value.Value = (Int32)(value.Value * (1f + m_staticData.GetValues(m_level)[0] * 0.01f));
							damages[i] = value;
						}
					}
				}
			}
			else if (m_currentExecutionPhase == EExecutionPhase.BEFORE_TARGET_SELECTION)
			{
				Single num = 1f + m_staticData.GetValues(m_level)[1] * (1f - p_monster.CurrentHealth / (Single)p_monster.MaxHealth);
				foreach (Attack attack2 in p_attackList)
				{
					List<Damage> damages2 = attack2.Damages;
					for (Int32 j = 0; j < damages2.Count; j++)
					{
						Damage value2 = damages2[j];
						value2.Value = (Int32)(value2.Value * num);
						damages2[j] = value2;
					}
				}
			}
		}

		public override String GetDescription()
		{
			return Localization.Instance.GetText("MONSTER_ABILITY_DEMONIC_LINEAG_INFO", m_staticData.GetValues(m_level)[0], m_staticData.GetValues(m_level)[1]);
		}
	}
}
