using System;
using System.Collections.Generic;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityBoss : MonsterAbilityBase
	{
		public MonsterAbilityBoss() : base(EMonsterAbilityType.BOSS)
		{
			m_executionPhase = EExecutionPhase.ON_APPLY_MONSTER_BUFF;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			for (Int32 i = 0; i < m_staticData.GetValues(m_level).Length; i++)
			{
				if (p_monster.BuffHandler.HasBuff((EMonsterBuffType)m_staticData.GetValues(m_level)[i]))
				{
					p_monster.BuffHandler.RemoveBuffByID((Int32)m_staticData.GetValues(m_level)[i]);
				}
			}
		}

		public override Boolean IsForbiddenBuff(Int32 p_buffID, Monster p_monster, Boolean p_silent)
		{
			for (Int32 i = 0; i < m_staticData.GetValues(m_level).Length; i++)
			{
				if (p_buffID == (Int32)m_staticData.GetValues(m_level)[i])
				{
					return true;
				}
			}
			return false;
		}
	}
}
