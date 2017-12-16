using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityUndead : MonsterAbilityBase
	{
		public MonsterAbilityUndead() : base(EMonsterAbilityType.UNDEAD)
		{
			m_executionPhase = EExecutionPhase.ON_APPLY_MONSTER_BUFF;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			for (Int32 i = 0; i < m_staticData.GetValues(m_level).Length; i++)
			{
				p_monster.BuffHandler.RemoveBuffByID((Int32)m_staticData.GetValues(m_level)[i]);
			}
		}

		public override Boolean IsForbiddenBuff(Int32 p_buffID, Monster p_monster, Boolean p_silent)
		{
			foreach (Single num in m_staticData.GetValues(m_level))
			{
				if ((Int32)num == p_buffID)
				{
					return true;
				}
			}
			return false;
		}
	}
}
